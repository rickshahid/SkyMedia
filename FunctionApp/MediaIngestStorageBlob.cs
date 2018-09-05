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
        public static void Run([BlobTrigger(Constant.Storage.BlobContainer.MediaServices + "/{blobName}", Connection = "%Storage%")] Stream blobStream, string blobName, ILogger logger)
        {
            StorageBlobClient blobClient = new StorageBlobClient();
            try
            {
                if (!blobName.EndsWith(Constant.Media.IngestManifest.FileExtensionLog, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogInformation("Media File: {0}", blobName);
                    if (blobName.StartsWith(Constant.Media.IngestManifest.TriggerPrefix, StringComparison.OrdinalIgnoreCase))
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
                                if (!string.Equals(missingFile, blobName, StringComparison.OrdinalIgnoreCase))
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
                string logData = ex.Response.ToString();
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
                CloudBlockBlob assetFile = blobClient.GetBlockBlob(containerName, fileName);
                if (!assetFile.ExistsAsync().Result)
                {
                    missingFiles.Add(fileName);
                }
            }
            ingestManifest.MissingFiles = missingFiles.ToArray();
            return ingestManifest;
        }

        private static void ProcessManifest(StorageBlobClient blobClient, Stream blobStream, string manifestName, ILogger log)
        {
            WriteLog(blobClient, manifestName, null);
            MediaIngestManifest ingestManifest = GetManifest(blobClient, blobStream, manifestName);
            string logData = JsonConvert.SerializeObject(ingestManifest);
            WriteLog(blobClient, manifestName, logData);
            log.LogInformation(logData);
            if (ingestManifest.MissingFiles.Length > 0)
            {
                string collectionId = Constant.Database.Collection.MediaIngest;
                _databaseClient.UpsertDocument(collectionId, ingestManifest);
            }
            else
            {
                using (MediaClient mediaClient = new MediaClient(null, ingestManifest.MediaAccount))
                {
                    if (ingestManifest.BlobFiles.Length > 0)
                    {
                        ingestManifest = CreateAsset(blobClient, mediaClient, ingestManifest, log);
                    }
                    if (ingestManifest.MediaProcessors.Length > 0)
                    {
                        CreateJob(blobClient, mediaClient, ingestManifest, log);
                    }
                }
            }
        }

        private static MediaIngestManifest CreateAsset(StorageBlobClient blobClient, MediaClient mediaClient, MediaIngestManifest ingestManifest, ILogger logger)
        {
            string storageAccount = ingestManifest.StorageAccount;
            string assetName = ingestManifest.AssetName;
            string assetDescription = ingestManifest.AssetDescription;
            string assetAlternateId = ingestManifest.AssetAlternateId;
            string sourceContainer = Constant.Storage.BlobContainer.MediaServices;
            string[] blobFileNames = ingestManifest.BlobFiles;
            StorageBlobClient assetBlobClient = new StorageBlobClient(ingestManifest.MediaAccount, storageAccount);
            Asset asset = mediaClient.CreateAsset(blobClient, assetBlobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, blobFileNames);
            string logData = string.Concat("Asset Name: ", asset.Name);
            WriteLog(blobClient, ingestManifest.Name, logData);
            logger.LogInformation(logData);
            logData = string.Concat("Asset Files: ", string.Join(",", blobFileNames));
            WriteLog(blobClient, ingestManifest.Name, logData);
            logger.LogInformation(logData);
            ingestManifest.AssetName = asset.Name;
            return ingestManifest;
        }

        private static void CreateJob(StorageBlobClient blobClient, MediaClient mediaClient, MediaIngestManifest ingestManifest, ILogger logger)
        {
            bool adaptiveStreaming = false;
            bool thumbnailSprite = false;
            bool videoAnalyzer = false;
            bool audioAnalyzer = false;
            bool videoIndexer = false;
            bool audioIndexer = false;
            foreach (MediaProcessor mediaProcessor in ingestManifest.MediaProcessors)
            {
                switch (mediaProcessor)
                {
                    case MediaProcessor.StandardEncoder:
                        adaptiveStreaming = true;
                        //thumbnailSprite
                        break;
                    case MediaProcessor.VideoAnalyzer:
                        videoAnalyzer = true;
                        break;
                    case MediaProcessor.AudioAnalyzer:
                        audioAnalyzer = true;
                        break;
                    case MediaProcessor.VideoIndexer:
                        videoIndexer = true;
                        break;
                    case MediaProcessor.AudioIndexer:
                        audioIndexer = true;
                        break;
                }
            }
            string indexId = null;
            if (mediaClient.IndexerIsEnabled() && (videoIndexer || audioIndexer))
            {
                string videoUrl = ingestManifest.JobInputFileUrl;
                if (string.IsNullOrEmpty(videoUrl))
                {
                    Asset inputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, ingestManifest.AssetName);
                    string fileName = Path.GetFileNameWithoutExtension(ingestManifest.BlobFiles[0]);
                    videoUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                }
                bool audioOnly = !videoIndexer && audioIndexer;
                indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, videoUrl, ingestManifest.AssetName, ingestManifest.AssetDescription, string.Empty, audioOnly);
                string logData = string.Concat("Index Id: ", indexId);
                WriteLog(blobClient, ingestManifest.Name, logData);
                logger.LogInformation(logData);
            }
            Transform transform = mediaClient.CreateTransform(adaptiveStreaming, thumbnailSprite, videoAnalyzer, audioAnalyzer);
            if (transform != null)
            {
                Job job = mediaClient.CreateJob(null, transform.Name, ingestManifest.JobName, ingestManifest.JobDescription, ingestManifest.JobPriority, ingestManifest.JobData, ingestManifest.AssetName, ingestManifest.JobInputFileUrl, ingestManifest.StorageAccount, ingestManifest.JobOutputAssetDescription, indexId, ingestManifest.StreamingPolicyName);
                string logData = string.Concat("Transform Name: ", transform.Name);
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