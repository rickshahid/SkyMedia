using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaIngestStorageBlob
    {
        private static DatabaseClient _databaseClient = new DatabaseClient();

        [FunctionName("MediaIngestStorageBlob")]
        public static void Run([BlobTrigger(Constant.Storage.BlobContainer.MediaServices + "/{blobName}", Connection = "Storage")] Stream blobStream, string blobName, ILogger logger)
        {
            StorageBlobClient blobClient = new StorageBlobClient();
            try
            {
                StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
                if (!blobName.EndsWith(Constant.Media.IngestManifest.FileExtensionLog, stringComparison))
                {
                    logger.LogInformation("Media File: {0}", blobName);
                    if (blobName.StartsWith(Constant.Media.IngestManifest.TriggerPrefix, stringComparison))
                    {
                        ProcessManifest(blobClient, blobStream, blobName, logger);
                    }
                    else
                    {
                        string collectionId = Constant.Database.Collection.MediaIngest;
                        MediaIngestManifest[] ingestManifests = _databaseClient.GetDocuments<MediaIngestManifest>(collectionId);
                        foreach (MediaIngestManifest ingestManifest in ingestManifests)
                        {
                            List<string> missingFiles = new List<string>();
                            foreach (string missingFile in ingestManifest.MissingFiles)
                            {
                                if (!string.Equals(missingFile, blobName, stringComparison))
                                {
                                    missingFiles.Add(blobName);
                                }
                            }
                            if (missingFiles.Count == 0)
                            {
                                _databaseClient.DeleteDocument(collectionId, ingestManifest.Name);
                                string containerName = Constant.Storage.BlobContainer.MediaServices;
                                CloudBlockBlob manifestBlob = blobClient.GetBlockBlob(containerName, ingestManifest.Name);
                                blobStream = manifestBlob.OpenReadAsync().Result;
                                ProcessManifest(blobClient, blobStream, ingestManifest.Name, logger);
                            }
                            else if (missingFiles.Count != ingestManifest.MissingFiles.Length)
                            {
                                ingestManifest.MissingFiles = missingFiles.ToArray();
                                _databaseClient.UpsertDocument(collectionId, ingestManifest);
                            }
                        }
                    }
                }
            }
            catch (ApiErrorException ex)
            {
                string logData = ex.ToString();
                WriteLog(blobClient, blobName, logData);
                logger.LogError(ex, logData);
                logData = ex.Response.Content;
                WriteLog(blobClient, blobName, logData);
                logger.LogError(ex, logData);
            }
            catch (Exception ex)
            {
                string logData = ex.ToString();
                WriteLog(blobClient, blobName, logData);
                logger.LogError(ex, logData);
            }
        }

        private static MediaIngestManifest GetManifest(StorageBlobClient blobClient, Stream manifestStream, string manifestName)
        {
            MediaIngestManifest ingestManifest;
            using (StreamReader manifestReader = new StreamReader(manifestStream))
            {
                string manifestData = manifestReader.ReadToEnd();
                ingestManifest = JsonConvert.DeserializeObject<MediaIngestManifest>(manifestData);
                ingestManifest.Name = manifestName;
            }
            List<string> missingFiles = new List<string>();
            foreach (string fileName in ingestManifest.BlobFiles)
            {
                string containerName = Constant.Storage.BlobContainer.MediaServices;
                CloudBlockBlob blobFile = blobClient.GetBlockBlob(containerName, fileName);
                if (!blobFile.ExistsAsync().Result)
                {
                    missingFiles.Add(fileName);
                }
            }
            ingestManifest.MissingFiles = missingFiles.ToArray();
            return ingestManifest;
        }

        private static void ProcessManifest(StorageBlobClient blobClient, Stream blobStream, string manifestName, ILogger logger)
        {
            WriteLog(blobClient, manifestName, null);
            MediaIngestManifest ingestManifest = GetManifest(blobClient, blobStream, manifestName);
            if (ingestManifest.MissingFiles.Length > 0)
            {
                string collectionId = Constant.Database.Collection.MediaIngest;
                _databaseClient.UpsertDocument(collectionId, ingestManifest);
            }
            else
            {
                if (string.IsNullOrEmpty(ingestManifest.JobInputFileUrl) && ingestManifest.JobInputType == MediaJobInputType.UploadFile)
                {
                    string sourceContainer = Constant.Storage.BlobContainer.MediaServices;
                    string fileName = ingestManifest.BlobFiles[0];
                    ingestManifest.JobInputFileUrl = blobClient.GetDownloadUrl(sourceContainer, fileName, false);
                }
                using (MediaClient mediaClient = new MediaClient(null, ingestManifest.MediaAccount))
                {
                    Asset inputAsset = null;
                    if (ingestManifest.BlobFiles.Length > 0)
                    {
                        ingestManifest = CreateAsset(blobClient, mediaClient, ingestManifest, out inputAsset, logger);
                    }
                    if (ingestManifest.TransformPresets.Length > 0)
                    {
                        CreateJob(blobClient, mediaClient, ingestManifest, inputAsset, logger);
                    }
                }
            }
        }

        private static MediaIngestManifest CreateAsset(StorageBlobClient blobClient, MediaClient mediaClient, MediaIngestManifest ingestManifest, out Asset inputAsset, ILogger logger)
        {
            string storageAccount = mediaClient.PrimaryStorageAccount;
            string assetName = ingestManifest.AssetName;
            string assetDescription = ingestManifest.AssetDescription;
            string assetAlternateId = ingestManifest.AssetAlternateId;
            string sourceContainer = Constant.Storage.BlobContainer.MediaServices;
            string[] blobFileNames = ingestManifest.BlobFiles;
            StorageBlobClient assetBlobClient = new StorageBlobClient(ingestManifest.MediaAccount, storageAccount);
            inputAsset = mediaClient.CreateAsset(blobClient, assetBlobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, blobFileNames);
            string logData = string.Concat("New Asset: ", JsonConvert.SerializeObject(inputAsset));
            WriteLog(blobClient, ingestManifest.Name, logData);
            logger.LogInformation(logData);
            ingestManifest.AssetName = inputAsset.Name;
            if (string.IsNullOrEmpty(ingestManifest.JobInputFileUrl) && ingestManifest.JobInputType == MediaJobInputType.AssetFile)
            {
                string fileName = blobFileNames[0];
                ingestManifest.JobInputFileUrl = assetBlobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
            }
            return ingestManifest;
        }

        private static void CreateJob(StorageBlobClient blobClient, MediaClient mediaClient, MediaIngestManifest ingestManifest, Asset inputAsset, ILogger logger)
        {
            string logData = string.Concat("Input File Url: ", ingestManifest.JobInputFileUrl);
            WriteLog(blobClient, ingestManifest.Name, logData);
            logger.LogInformation(logData);
            MediaTransformPresets transformPresets = mediaClient.GetTransformPresets(ingestManifest.TransformPresets);
            if (mediaClient.IndexerEnabled() && (transformPresets.VideoIndexer || transformPresets.AudioIndexer))
            {
                bool audioOnly = !transformPresets.VideoIndexer && transformPresets.AudioIndexer;
                string indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, inputAsset, ingestManifest.JobInputFileUrl, audioOnly);
                logData = string.Concat("Index Id: ", indexId);
                WriteLog(blobClient, ingestManifest.Name, logData);
                logger.LogInformation(logData);
            }
            Transform transform = mediaClient.CreateTransform(transformPresets);
            if (transform != null)
            {
                Job job = mediaClient.CreateJob(null, transform.Name, ingestManifest.JobName, ingestManifest.JobDescription, ingestManifest.JobPriority, ingestManifest.JobData, ingestManifest.AssetName, ingestManifest.JobInputFileUrl, ingestManifest.JobOutputAssetFilesMerge, ingestManifest.JobOutputAssetDescriptions, ingestManifest.JobOutputAssetAlternateIds, ingestManifest.StreamingPolicyName);
                logData = string.Concat("Transform Name: ", transform.Name);
                WriteLog(blobClient, ingestManifest.Name, logData);
                logger.LogInformation(logData);
                logData = string.Concat("Job Name: ", job.Name);
                WriteLog(blobClient, ingestManifest.Name, logData);
                logger.LogInformation(logData);
            }
        }

        private static void WriteLog(StorageBlobClient blobClient, string manifestName, string logData)
        {
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string logName = manifestName.Replace(Constant.Media.IngestManifest.FileExtension, Constant.Media.IngestManifest.FileExtensionLog);
            CloudAppendBlob logBlob = blobClient.GetAppendBlob(containerName, logName);
            if (string.IsNullOrEmpty(logData))
            {
                logBlob.CreateOrReplaceAsync().Wait();
            }
            else
            {
                CloudBlobStream logStream = logBlob.OpenWriteAsync(false).Result;
                using (StreamWriter logWriter = new StreamWriter(logStream))
                {
                    logWriter.WriteLine(logData);
                }
            }
        }
    }
}