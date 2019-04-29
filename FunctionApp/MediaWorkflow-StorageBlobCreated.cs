using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaWorkflowStorageBlobCreated
    {
        [FunctionName("MediaWorkflow-StorageBlobCreated")]
        public async static Task Run([EventGridTrigger] EventGridEvent eventTrigger, [Blob("{data.url}", FileAccess.Read)] Stream workflowInput,
                                     [Blob(Constant.Storage.Blob.WorkflowManifestPath, FileAccess.Read)] Stream manifestInput, ILogger logger)
        {
            try
            {
                logger.LogInformation(JsonConvert.SerializeObject(eventTrigger, Formatting.Indented));
                if (ValidInput(eventTrigger, manifestInput, logger, out MediaWorkflowManifest workflowManifest))
                {
                    using (workflowInput)
                    {
                        JObject eventData = JObject.FromObject(eventTrigger.Data);
                        MediaAccount mediaAccount = workflowManifest.MediaAccounts[0];
                        using (MediaClient mediaClient = new MediaClient(mediaAccount))
                        {
                            Asset inputAsset = null;
                            string inputFileUrl = null;
                            string inputFileName = workflowManifest.InputFileName;
                            if (string.IsNullOrEmpty(inputFileName))
                            {
                                inputFileName = Path.GetFileName(eventTrigger.Subject);
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
                                        CreateJob(mediaClient, workflowManifest, transform, inputFileUrl);
                                    }
                                    break;
                                case MediaJobInputMode.AssetFile:
                                    inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, inputFileName, inputFileName, workflowInput);
                                    blobClient = new StorageBlobClient(mediaClient.MediaAccount, inputAsset.StorageAccountName);
                                    inputFileUrl = blobClient.GetDownloadUrl(inputAsset.Container, inputFileName);
                                    logger.LogInformation(inputFileUrl);
                                    if (transform != null)
                                    {
                                        CreateJob(mediaClient, workflowManifest, transform, inputFileUrl);
                                    }
                                    break;
                                case MediaJobInputMode.Asset:
                                    inputAsset = await mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, inputFileName, inputFileName, workflowInput);
                                    logger.LogInformation(inputAsset.Name);
                                    if (transform != null)
                                    {
                                        CreateJob(mediaClient, workflowManifest, transform, inputAsset);
                                    }
                                    break;
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

        private static bool ValidInput(EventGridEvent eventTrigger, Stream manifestInput, ILogger logger, out MediaWorkflowManifest workflowManifest)
        {
            bool validInput = false;
            workflowManifest = null;
            if (manifestInput != null)
            {
                string workflowManifestJson;
                using (StreamReader manifestReader = new StreamReader(manifestInput))
                {
                    workflowManifestJson = manifestReader.ReadToEnd();
                }
                logger.LogInformation(workflowManifestJson);
                workflowManifest = JsonConvert.DeserializeObject<MediaWorkflowManifest>(workflowManifestJson);
                if (eventTrigger.Subject.EndsWith(Constant.Storage.Blob.WorkflowManifestFile, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(workflowManifest.InputFileName))
                    {
                        StorageBlobClient blobClient = new StorageBlobClient();
                        string containerName = Constant.Storage.Blob.WorkflowContainerName;
                        CloudBlockBlob blob = blobClient.GetBlockBlob(containerName, null, workflowManifest.InputFileName);
                        validInput = blob.Exists();
                    }
                }
                else
                {
                    validInput = true;
                }
            }
            return validInput;
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, string inputFileUrl)
        {
            Uri inputFileUri = new Uri(inputFileUrl);
            string assetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? assetName : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string outputAssetStorage = workflowManifest.OutputAssetStorage;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, inputFileUrl, null, jobOutputMode, outputAssetStorage, streamingPolicyName);
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, Asset inputAsset)
        {
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? inputAsset.Name : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string outputAssetStorage = workflowManifest.OutputAssetStorage;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, null, inputAsset.Name, jobOutputMode, outputAssetStorage, streamingPolicyName);
        }
    }
}