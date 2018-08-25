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
        [FunctionName("MediaIngest-StorageBlob")]
        public static void Run([BlobTrigger("ams/{name}")] Stream blob, string name, ILogger logger)
        {
            BlobClient blobClient = new BlobClient();
            try
            {
                if (!name.EndsWith(Constant.Media.IngestManifest.FileExtensionLog, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogInformation("Media File: {0}", name);
                    if (name.StartsWith(Constant.Media.IngestManifest.TriggerPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        ProcessManifest(blobClient, blob, name, logger);
                    }
                    else
                    {
                        using (DatabaseClient databaseClient = new DatabaseClient())
                        {
                            string collectionId = Constant.Database.Collection.IngestManifest;
                            MediaIngestManifest[] ingestManifests = databaseClient.GetDocuments<MediaIngestManifest>(collectionId);
                            foreach (MediaIngestManifest ingestManifest in ingestManifests)
                            {
                                List<string> missingFiles = new List<string>();
                                foreach (string missingFile in ingestManifest.MissingFiles)
                                {
                                    if (!string.Equals(missingFile, name, StringComparison.OrdinalIgnoreCase))
                                    {
                                        missingFiles.Add(name);
                                    }
                                }
                                if (missingFiles.Count == 0)
                                {
                                    databaseClient.DeleteDocument(collectionId, ingestManifest.Name);
                                    string containerName = Constant.Storage.BlobContainer.MediaServices;
                                    CloudBlockBlob manifestBlob = blobClient.GetBlockBlob(containerName, ingestManifest.Name);
                                    blob = manifestBlob.OpenReadAsync().Result;
                                    ProcessManifest(blobClient, blob, ingestManifest.Name, logger);
                                }
                                else if (missingFiles.Count != ingestManifest.MissingFiles.Length)
                                {
                                    ingestManifest.MissingFiles = missingFiles.ToArray();
                                    databaseClient.UpsertDocument(collectionId, ingestManifest);
                                }
                            }
                        }
                    }
                }
            }
            catch (ApiErrorException ex)
            {
                string logData = ex.Response.ToString();
                WriteLog(blobClient, name, logData);
                logger.LogError(ex, logData);
            }
            catch (Exception ex)
            {
                string logData = ex.ToString();
                WriteLog(blobClient, name, logData);
                logger.LogError(ex, logData);
            }
        }

        private static MediaIngestManifest GetManifest(BlobClient blobClient, Stream manifestStream, string manifestName)
        {
            MediaIngestManifest ingestManifest;
            using (StreamReader manifestReader = new StreamReader(manifestStream))
            {
                string manifestData = manifestReader.ReadToEnd();
                ingestManifest = JsonConvert.DeserializeObject<MediaIngestManifest>(manifestData);
                ingestManifest.Name = manifestName;
            }
            List<string> missingFiles = new List<string>();
            foreach (string fileName in ingestManifest.AssetFiles)
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

        private static void ProcessManifest(BlobClient blobClient, Stream manifestStream, string manifestName, ILogger log)
        {
            WriteLog(blobClient, manifestName, null);
            MediaIngestManifest ingestManifest = GetManifest(blobClient, manifestStream, manifestName);
            string logData = JsonConvert.SerializeObject(ingestManifest);
            WriteLog(blobClient, manifestName, logData);
            log.LogInformation(logData);
            if (ingestManifest.MissingFiles.Length > 0)
            {
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    string collectionId = Constant.Database.Collection.IngestManifest;
                    databaseClient.UpsertDocument(collectionId, ingestManifest);
                }
            }
            else
            {
                using (MediaClient mediaClient = new MediaClient(null, ingestManifest.MediaAccount))
                {
                    if (ingestManifest.AssetFiles.Length > 0)
                    {
                        ingestManifest = CreateAsset(blobClient, mediaClient, ingestManifest, log);
                    }
                    if (ingestManifest.TransformPresets.Length > 0)
                    {
                        CreateJob(blobClient, mediaClient, ingestManifest, log);
                    }
                }
            }
        }

        private static MediaIngestManifest CreateAsset(BlobClient blobClient, MediaClient mediaClient, MediaIngestManifest ingestManifest, ILogger logger)
        {
            string storageAccount = ingestManifest.StorageAccount;
            string assetName = ingestManifest.AssetName;
            string assetDescription = ingestManifest.AssetDescription;
            string assetAlternateId = ingestManifest.AssetAlternateId;
            string sourceContainer = Constant.Storage.BlobContainer.MediaServices;
            string[] assetFileNames = ingestManifest.AssetFiles;
            BlobClient assetBlobClient = new BlobClient(ingestManifest.MediaAccount, storageAccount);
            Asset asset = mediaClient.CreateAsset(blobClient, assetBlobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, assetFileNames);
            string logData = string.Concat("Asset Name: ", asset.Name);
            WriteLog(blobClient, ingestManifest.Name, logData);
            logger.LogInformation(logData);
            logData = string.Concat("Asset Files: ", string.Join(",", assetFileNames));
            WriteLog(blobClient, ingestManifest.Name, logData);
            logger.LogInformation(logData);
            ingestManifest.AssetName = asset.Name;
            return ingestManifest;
        }

        private static void CreateJob(BlobClient blobClient, MediaClient mediaClient, MediaIngestManifest ingestManifest, ILogger logger)
        {
            bool standardEncoderPreset = false;
            bool videoAnalyzerPreset = false;
            bool audioAnalyzerPreset = false;
            foreach (MediaTransformPreset transformPreset in ingestManifest.TransformPresets)
            {
                switch (transformPreset)
                {
                    case MediaTransformPreset.AdaptiveStreaming:
                        standardEncoderPreset = true;
                        break;
                    case MediaTransformPreset.VideoAnalyzer:
                        videoAnalyzerPreset = true;
                        break;
                    case MediaTransformPreset.AudioAnalyzer:
                        audioAnalyzerPreset = true;
                        break;
                }
            }
            string indexId = null;
            if (mediaClient.IndexerIsEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
            {
                string videoUrl = ingestManifest.JobInputFileUrl;
                if (string.IsNullOrEmpty(videoUrl))
                {
                    Asset inputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, ingestManifest.AssetName);
                    string fileName = Path.GetFileNameWithoutExtension(ingestManifest.AssetFiles[0]);
                    videoUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                }
                bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, videoUrl, ingestManifest.AssetName, ingestManifest.AssetDescription, string.Empty, audioOnly);
                string logData = string.Concat("Index Id: ", indexId);
                WriteLog(blobClient, ingestManifest.Name, logData);
                logger.LogInformation(logData);
            }
            Transform transform = mediaClient.CreateTransform(standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
            if (transform != null)
            {
                Job job = mediaClient.CreateJob(transform.Name, ingestManifest.JobName, ingestManifest.JobDescription, ingestManifest.JobPriority, ingestManifest.AssetName, ingestManifest.JobInputFileUrl, ingestManifest.StorageAccount, ingestManifest.JobOutputAssetDescription, indexId, ingestManifest.StreamingPolicyName);
                string logData = string.Concat("Transform Name: ", transform.Name);
                WriteLog(blobClient, ingestManifest.Name, logData);
                logger.LogInformation(logData);
                logData = string.Concat("Job Name: ", job.Name);
                WriteLog(blobClient, ingestManifest.Name, logData);
                logger.LogInformation(logData);
            }
        }

        private static void WriteLog(BlobClient blobClient, string manifestName, string logData)
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