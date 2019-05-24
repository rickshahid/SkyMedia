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
        private static readonly StorageBlobClient _blobClient = new StorageBlobClient();
        private const string _containerName = Constant.Storage.Blob.WorkflowContainerName;
        private const StringComparison _stringComparison = StringComparison.OrdinalIgnoreCase;

        [FunctionName("MediaWorkflow-StorageBlobQueue")]
        public async static Task Run([QueueTrigger(Constant.Storage.Blob.WorkflowContainerName)] EventGridEvent queueMessage,
                                     [Blob(Constant.Storage.Blob.WorkflowManifestPath, FileAccess.Read)] Stream manifestInput, ILogger logger)
        {
            try
            {
                logger.LogInformation(JsonConvert.SerializeObject(queueMessage, Formatting.Indented));
                if (InputComplete(queueMessage, manifestInput, logger, out MediaWorkflowManifest workflowManifest))
                {
                    using (MediaClient mediaClient = new MediaClient(workflowManifest))
                    {
                        string inputFileName = workflowManifest.InputFileName;
                        if (string.IsNullOrEmpty(inputFileName))
                        {
                            inputFileName = Path.GetFileName(queueMessage.Subject);
                        }
                        if (inputFileName.Equals(Constant.Storage.Blob.WorkflowContainerFiles, _stringComparison))
                        {
                            CloudBlockBlob[] blobs = _blobClient.ListBlobContainer(_containerName, null);
                            foreach (CloudBlockBlob blob in blobs)
                            {
                                if (!blob.Name.Equals(Constant.Storage.Blob.WorkflowManifestFile, _stringComparison))
                                {
                                    if (workflowManifest.JobInputMode == MediaJobInputMode.Asset)
                                    {
                                        await CreateAssetJob(mediaClient, workflowManifest, blob, logger);
                                    }
                                    else
                                    {
                                        CreateFileJob(mediaClient, workflowManifest, blob.Name, logger);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (workflowManifest.JobInputMode == MediaJobInputMode.Asset)
                            {
                                CloudBlockBlob blob = _blobClient.GetBlockBlob(_containerName, null, inputFileName);
                                await CreateAssetJob(mediaClient, workflowManifest, blob, logger);
                            }
                            else
                            {
                                CreateFileJob(mediaClient, workflowManifest, inputFileName, logger);
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

        private async static Task CreateAssetJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, CloudBlockBlob blob, ILogger logger)
        {
            Asset inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, workflowManifest.AssetName, blob);
            logger.LogInformation(Constant.Message.AssetCreated, inputAsset.Name);
            CreateJob(mediaClient, workflowManifest, null, inputAsset, logger);
        }

        private static void CreateFileJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, string fileName, ILogger logger)
        {
            string inputFileUrl = _blobClient.GetDownloadUrl(_containerName, fileName);
            logger.LogInformation(inputFileUrl);
            CreateJob(mediaClient, workflowManifest, inputFileUrl, null, logger);
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, string inputFileUrl, Asset inputAsset, ILogger logger)
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