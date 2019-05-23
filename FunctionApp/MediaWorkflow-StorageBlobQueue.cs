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
        private static StringComparison _stringComparison = StringComparison.OrdinalIgnoreCase;

        [FunctionName("MediaWorkflow-StorageBlobQueue")]
        public async static Task Run([QueueTrigger(Constant.Storage.Blob.WorkflowContainerName)] EventGridEvent queueMessage,
                                     [Blob(Constant.Storage.Blob.WorkflowManifestPath, FileAccess.Read)] Stream manifestInput,
                                     [Blob("{data.url}", FileAccess.Read)] Stream workflowInput, ILogger logger)
        {
            try
            {
                logger.LogInformation(JsonConvert.SerializeObject(queueMessage, Formatting.Indented));
                if (InputComplete(queueMessage, manifestInput, logger, out MediaWorkflowManifest workflowManifest))
                {
                    using (workflowInput)
                    using (MediaClient mediaClient = new MediaClient(workflowManifest))
                    {
                        StorageBlobClient blobClient = new StorageBlobClient();
                        string containerName = Constant.Storage.Blob.WorkflowContainerName;
                        string inputFileName = workflowManifest.InputFileName;
                        if (string.IsNullOrEmpty(inputFileName))
                        {
                            inputFileName = Path.GetFileName(queueMessage.Subject);
                        }
                        if (inputFileName.Equals(Constant.Storage.Blob.WorkflowContainerFiles, _stringComparison))
                        {
                            CloudBlockBlob[] blobs = blobClient.ListBlobContainer(containerName, null);
                            foreach (CloudBlockBlob blob in blobs)
                            {
                                if (!blob.Name.Equals(Constant.Storage.Blob.WorkflowManifestFile, _stringComparison))
                                {
                                    if (workflowManifest.JobInputMode == MediaJobInputMode.Asset)
                                    {
                                        string assetName = Path.GetFileNameWithoutExtension(blob.Name);
                                        Asset inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, assetName, blob.Name, workflowInput);
                                        logger.LogInformation(Constant.Message.AssetCreated, inputAsset.Name);
                                        ProcessInput(mediaClient, workflowManifest, null, inputAsset, logger);
                                    }
                                    else
                                    {
                                        string inputFileUrl = blobClient.GetDownloadUrl(containerName, blob.Name);
                                        logger.LogInformation(inputFileUrl);
                                        ProcessInput(mediaClient, workflowManifest, inputFileUrl, null, logger);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (workflowManifest.JobInputMode == MediaJobInputMode.Asset)
                            {
                                string assetName = Path.GetFileNameWithoutExtension(inputFileName);
                                Asset inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, assetName, inputFileName, workflowInput);
                                logger.LogInformation(Constant.Message.AssetCreated, inputAsset.Name);
                                ProcessInput(mediaClient, workflowManifest, null, inputAsset, logger);
                            }
                            else
                            {
                                string inputFileUrl = blobClient.GetDownloadUrl(containerName, inputFileName);
                                logger.LogInformation(inputFileUrl);
                                ProcessInput(mediaClient, workflowManifest, inputFileUrl, null, logger);
                            }
                        }
                    }
                }
            }
            catch (ApiErrorException ex)
            {
                logger.LogError(ex.Response.Content);
                logger.LogError(ex.ToString());
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
                    inputComplete = !queueMessage.Subject.EndsWith(Constant.Storage.Blob.WorkflowManifestFile, _stringComparison);
                }
                else if (workflowManifest.InputFileName.Equals(Constant.Storage.Blob.WorkflowContainerFiles))
                {
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

        private static void ProcessInput(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, string inputFileUrl, Asset inputAsset, ILogger logger)
        {
            string insightId = null;
            bool videoIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoIndexer);
            bool audioIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioIndexer);
            bool indexerEnabled = mediaClient.IndexerEnabled() && (videoIndexer || audioIndexer);
            if (indexerEnabled)
            {
                insightId = mediaClient.IndexerUploadVideo(inputFileUrl, inputAsset, workflowManifest.JobPriority, videoIndexer, audioIndexer);
                logger.LogInformation(Constant.Message.InsightCreated, insightId);
            }
            Transform transform = mediaClient.GetTransform(workflowManifest.TransformPresets);
            if (transform != null)
            {
                MediaJobOutputInsight outputInsight = new MediaJobOutputInsight()
                {
                    Id = insightId,
                    VideoIndexer = videoIndexer,
                    AudioIndexer = audioIndexer
                };
                string inputAssetName = inputAsset == null ? null : inputAsset.Name;
                Job job = mediaClient.CreateJob(transform.Name, workflowManifest.JobName, null, workflowManifest.JobPriority, inputFileUrl, inputAssetName, workflowManifest.OutputAssetStorage, workflowManifest.JobOutputPublish, outputInsight);
                logger.LogInformation(Constant.Message.JobCreated, job.Name);
            }
        }
    }
}