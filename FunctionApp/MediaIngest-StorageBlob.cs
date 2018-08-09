using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

using Newtonsoft.Json;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaIngestStorageBlob
    {
        [FunctionName("MediaIngest-StorageBlob")]
        public static void Run([BlobTrigger("ams/{name}")] Stream blob, string name, TraceWriter log)
        {
            bool createLog = true;
            BlobClient blobClient = new BlobClient();
            try
            {
                if (!name.EndsWith(Constant.Media.IngestManifest.FileExtensionLog, StringComparison.OrdinalIgnoreCase))
                {
                    log.Info($"Media File: {name}");
                    if (name.StartsWith(Constant.Media.IngestManifest.TriggerPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        ProcessManifest(blobClient, blob, name, log, createLog);
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
                                    ProcessManifest(blobClient, blob, ingestManifest.Name, log, createLog);
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
            catch (Exception ex)
            {
                string logData = ex.ToString();
                createLog = UpdateLog(blobClient, name, logData, createLog);
                log.Info(logData);
            }
        }

        private static MediaIngestManifest GetManifest(BlobClient blobClient, Stream manifestStream)
        {
            StreamReader manifestReader = new StreamReader(manifestStream);
            string ingestManifestData = manifestReader.ReadToEnd();
            MediaIngestManifest ingestManifest = JsonConvert.DeserializeObject<MediaIngestManifest>(ingestManifestData);
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

        private static void ProcessManifest(BlobClient blobClient, Stream manifestStream, string manifestName, TraceWriter log, bool createLog)
        {
            MediaIngestManifest ingestManifest = GetManifest(blobClient, manifestStream);
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
                MediaClient mediaClient = new MediaClient(null, ingestManifest.MediaAccount);
                if (ingestManifest.AssetFiles.Length > 0)
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
                    createLog = UpdateLog(blobClient, manifestName, logData, createLog);
                    log.Info(logData);
                    logData = string.Concat("Asset Files: ", string.Join(",", assetFileNames));
                    createLog = UpdateLog(blobClient, manifestName, logData, createLog);
                    log.Info(logData);
                }
                if (ingestManifest.TransformPresets.Length > 0)
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
                    Transform transform = mediaClient.CreateTransform(standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
                    Job job = mediaClient.CreateJob(transform.Name, ingestManifest);
                    string logData = string.Concat("Transform Name: ", transform.Name);
                    createLog = UpdateLog(blobClient, manifestName, logData, createLog);
                    log.Info(logData);
                    logData = string.Concat("Job Name: ", job.Name);
                    createLog = UpdateLog(blobClient, manifestName, logData, createLog);
                    log.Info(logData);
                }
            }
        }

        private static bool UpdateLog(BlobClient blobClient, string manifestName, string logData, bool createLog)
        {
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string logName = manifestName.Replace(Constant.Media.IngestManifest.FileExtension, Constant.Media.IngestManifest.FileExtensionLog);
            CloudAppendBlob logBlob = blobClient.GetAppendBlob(containerName, logName);
            CloudBlobStream logStream = logBlob.OpenWriteAsync(createLog).Result;
            using (StreamWriter logWriter = new StreamWriter(logStream))
            {
                logWriter.WriteLine(logData);
            }
            return false;
        }
    }
}