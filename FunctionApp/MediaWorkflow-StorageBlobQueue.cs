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
                        UserAccount userAccount = workflowManifest.UserAccount;
                        MediaAccount mediaAccount = workflowManifest.MediaAccounts[0];
                        using (MediaClient mediaClient = new MediaClient(userAccount, mediaAccount))
                        {
                            Job job = null;
                            Asset inputAsset = null;
                            string inputFileUrl = null;
                            string inputFileName = workflowManifest.InputFileName;
                            if (string.IsNullOrEmpty(inputFileName))
                            {
                                inputFileName = Path.GetFileName(queueMessage.Subject);
                            }
                            Transform transform = mediaClient.GetTransform(workflowManifest.TransformPresets);
                            switch (workflowManifest.JobInputMode)
                            {
                                case MediaJobInputMode.InputFile:
                                    StorageBlobClient blobClient = new StorageBlobClient();
                                    string containerName = Constant.Storage.Blob.WorkflowContainerName;
                                    inputFileUrl = blobClient.GetDownloadUrl(containerName, inputFileName);
                                    logger.LogInformation(inputFileUrl);
                                    if (transform != null)
                                    {
                                        job = await CreateJob(mediaClient, workflowManifest, transform, inputFileUrl);
                                    }
                                    break;
                                case MediaJobInputMode.AssetFile:
                                    inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, inputFileName, inputFileName, workflowInput);
                                    blobClient = new StorageBlobClient(mediaClient.MediaAccount, inputAsset.StorageAccountName);
                                    inputFileUrl = blobClient.GetDownloadUrl(inputAsset.Container, inputFileName);
                                    logger.LogInformation(inputFileUrl);
                                    if (transform != null)
                                    {
                                        job = await CreateJob(mediaClient, workflowManifest, transform, inputFileUrl);
                                    }
                                    break;
                                case MediaJobInputMode.Asset:
                                    inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, inputFileName, inputFileName, workflowInput);
                                    logger.LogInformation(inputAsset.Name);
                                    if (transform != null)
                                    {
                                        job = await CreateJob(mediaClient, workflowManifest, transform, inputAsset);
                                    }
                                    break;
                            }
                            if (job != null)
                            {
                                logger.LogInformation(Constant.Message.JobCreated, job.Name);
                            }
                            bool videoIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoIndexer);
                            bool audioIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioIndexer);
                            bool indexerEnabled = mediaClient.IndexerEnabled() && (videoIndexer || audioIndexer);
                            bool indexOnly = false;
                            bool audioOnly = !videoIndexer && audioIndexer;
                            bool videoOnly = false;
                            if (indexerEnabled)
                            {
                                string insightId = mediaClient.IndexerUploadVideo(inputFileUrl, inputAsset, workflowManifest.JobPriority, indexOnly, audioOnly, videoOnly);
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
                if (queueMessage.Subject.EndsWith(Constant.Storage.Blob.WorkflowManifestFile, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(workflowManifest.InputFileName))
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
                else
                {
                    inputComplete = true;
                }
            }
            return inputComplete;
        }

        private static async Task<Job> CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, string inputFileUrl)
        {
            Uri inputFileUri = new Uri(inputFileUrl);
            string assetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? assetName : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string outputAssetStorage = workflowManifest.OutputAssetStorage;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            return await mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, inputFileUrl, null, jobOutputMode, outputAssetStorage, streamingPolicyName);
        }

        private static async Task<Job> CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, Asset inputAsset)
        {
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? inputAsset.Name : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string outputAssetStorage = workflowManifest.OutputAssetStorage;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            return await mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, null, inputAsset.Name, jobOutputMode, outputAssetStorage, streamingPolicyName);
        }
    }
}