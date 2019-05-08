using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaWorkflowStorageBlobQueue
    {
        [FunctionName("MediaWorkflow-StorageBlobQueue")]
        public static async Task Run([QueueTrigger(Constant.Storage.Blob.WorkflowContainerName)] EventGridEvent queueMessage,
                                     [Blob(Constant.Storage.Blob.WorkflowManifestPath, FileAccess.Read)] Stream manifestInput,
                                     [Blob("{data.url}", FileAccess.Read)] Stream workflowInput, ILogger logger)
        {
            try
            {
                logger.LogInformation(JsonConvert.SerializeObject(queueMessage, Formatting.Indented));
                if (InputComplete(queueMessage, manifestInput, logger, out MediaWorkflowManifest workflowManifest))
                {
                    using (workflowInput)
                    {
                        using (MediaClient mediaClient = new MediaClient(workflowManifest))
                        {
                            Asset inputAsset = null;
                            string inputFileUrl = null;
                            string inputFileName = workflowManifest.InputFileName;
                            if (string.IsNullOrEmpty(inputFileName))
                            {
                                inputFileName = Path.GetFileName(queueMessage.Subject);
                            }
                            string containerName = Constant.Storage.Blob.WorkflowContainerName;
                            Transform transform = mediaClient.GetTransform(workflowManifest.TransformPresets);
                            switch (workflowManifest.JobInputMode)
                            {
                                case MediaJobInputMode.Container:
                                    StorageBlobClient blobClient = new StorageBlobClient();
                                    CloudBlockBlob[] blobs = blobClient.ListBlobContainer(containerName, null);
                                    foreach (CloudBlockBlob blob in blobs)
                                    {
                                        if (!blob.Name.Equals(Constant.Storage.Blob.WorkflowManifestFile, StringComparison.OrdinalIgnoreCase))
                                        {
                                            inputFileUrl = blobClient.GetDownloadUrl(containerName, blob.Name);
                                            logger.LogInformation(inputFileUrl);
                                            if (transform != null)
                                            {
                                                CreateJob(mediaClient, workflowManifest, transform, inputFileUrl, inputAsset, logger);
                                            }
                                            CreateInsight(mediaClient, workflowManifest, inputFileUrl, inputAsset, logger);
                                        }
                                    }
                                    break;
                                case MediaJobInputMode.InputFile:
                                    blobClient = new StorageBlobClient();
                                    inputFileUrl = blobClient.GetDownloadUrl(containerName, inputFileName);
                                    logger.LogInformation(inputFileUrl);
                                    if (transform != null)
                                    {
                                        CreateJob(mediaClient, workflowManifest, transform, inputFileUrl, inputAsset, logger);
                                    }
                                    break;
                                case MediaJobInputMode.AssetFile:
                                    inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, inputFileName, inputFileName, workflowInput);
                                    blobClient = new StorageBlobClient(mediaClient.MediaAccount, inputAsset.StorageAccountName);
                                    inputFileUrl = blobClient.GetDownloadUrl(inputAsset.Container, inputFileName);
                                    logger.LogInformation(inputFileUrl);
                                    if (transform != null)
                                    {
                                        CreateJob(mediaClient, workflowManifest, transform, inputFileUrl, inputAsset, logger);
                                    }
                                    break;
                                case MediaJobInputMode.Asset:
                                    inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, inputFileName, inputFileName, workflowInput);
                                    logger.LogInformation(inputAsset.Name);
                                    if (transform != null)
                                    {
                                        CreateJob(mediaClient, workflowManifest, transform, inputFileUrl, inputAsset, logger);
                                    }
                                    break;
                            }
                            if (workflowManifest.JobInputMode != MediaJobInputMode.Container)
                            {
                                CreateInsight(mediaClient, workflowManifest, inputFileUrl, inputAsset, logger);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        private static bool InputComplete(EventGridEvent queueMessage, Stream manifestInput, ILogger logger, out MediaWorkflowManifest workflowManifest)
        {
            workflowManifest = null;
            bool inputComplete = false;
            if (manifestInput == null)
            {
                logger.LogInformation(string.Format(Constant.Message.WorkflowInputNotComplete, Constant.Storage.Blob.WorkflowManifestFile));
            }
            else
            {
                string workflowManifestJson;
                using (StreamReader manifestReader = new StreamReader(manifestInput))
                {
                    workflowManifestJson = manifestReader.ReadToEnd();
                }
                logger.LogInformation(workflowManifestJson);
                workflowManifest = JsonConvert.DeserializeObject<MediaWorkflowManifest>(workflowManifestJson);
                if (string.IsNullOrEmpty(workflowManifest.InputFileName))
                {
                    inputComplete = !queueMessage.Subject.EndsWith(Constant.Storage.Blob.WorkflowManifestFile, StringComparison.OrdinalIgnoreCase);
                }
                else if (workflowManifest.InputFileName.Equals(Constant.Storage.Blob.WorkflowContainerFiles))
                {
                    workflowManifest.JobInputMode = MediaJobInputMode.Container;
                    inputComplete = true;
                }
                else
                {
                    StorageBlobClient blobClient = new StorageBlobClient();
                    string containerName = Constant.Storage.Blob.WorkflowContainerName;
                    CloudBlockBlob blob = blobClient.GetBlockBlob(containerName, null, workflowManifest.InputFileName);
                    inputComplete = blob.Exists();
                    if (!inputComplete)
                    {
                        logger.LogInformation(string.Format(Constant.Message.WorkflowInputNotComplete, workflowManifest.InputFileName));
                    }
                }
            }
            return inputComplete;
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, string inputFileUrl, Asset inputAsset, ILogger logger)
        {
            string inputAssetName;
            if (inputAsset != null)
            {
                inputAssetName = inputAsset.Name;
            }
            else
            {
                Uri inputFileUri = new Uri(inputFileUrl);
                inputAssetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            }
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? inputAssetName : workflowManifest.JobName;
            Job job = mediaClient.CreateJob(transform.Name, jobName, null, workflowManifest.JobPriority, inputFileUrl, inputAssetName, workflowManifest.OutputAssetStorage, workflowManifest.JobOutputPublish);
            logger.LogInformation(Constant.Message.JobCreated, job.Name);
        }

        private static void CreateInsight(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, string inputFileUrl, Asset inputAsset, ILogger logger)
        {
            bool videoIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoIndexer);
            bool audioIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioIndexer);
            bool indexerEnabled = mediaClient.IndexerEnabled() && (videoIndexer || audioIndexer);
            if (indexerEnabled)
            {
                bool audioOnly = !videoIndexer && audioIndexer;
                bool videoOnly = videoIndexer && !audioIndexer;
                string insightId = mediaClient.IndexerUploadVideo(inputFileUrl, inputAsset, workflowManifest.JobPriority, audioOnly, videoOnly);
                logger.LogInformation(Constant.Message.InsightCreated, insightId);
            }
        }
    }
}