using System;
using System.IO;
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
        public async static void Run([EventGridTrigger] EventGridEvent eventTrigger, [Blob("{data.url}", FileAccess.Read)] Stream blobInput,
                                     [Blob(Constant.Storage.Blob.WorkflowManifestFile, FileAccess.Read)] Stream blobWorkflow, ILogger logger)
        {
            try
            {
                logger.LogInformation(JsonConvert.SerializeObject(eventTrigger, Formatting.Indented));
                if (blobWorkflow != null && !eventTrigger.Subject.EndsWith(Constant.FileExtension.WorkflowManifest, StringComparison.OrdinalIgnoreCase))
                {
                    string workflowJson;
                    using (StreamReader workflowReader = new StreamReader(blobWorkflow))
                    {
                        workflowJson = workflowReader.ReadToEnd();
                    }
                    logger.LogInformation(workflowJson);
                    MediaWorkflowManifest workflowManifest = JsonConvert.DeserializeObject<MediaWorkflowManifest>(workflowJson);
                    using (blobInput)
                    {
                        string fileName = Path.GetFileName(eventTrigger.Subject);
                        JObject eventData = JObject.FromObject(eventTrigger.Data);
                        MediaAccount mediaAccount = workflowManifest.MediaAccounts[0];
                        using (MediaClient mediaClient = new MediaClient(mediaAccount))
                        {
                            Transform transform = mediaClient.GetTransform(workflowManifest.TransformPresets);
                            switch (workflowManifest.JobInputMode)
                            {
                                case MediaJobInputMode.InputFile:
                                    string contentType = eventData["contentType"].ToString();
                                    string inputFileUrl = await CopyBlob(mediaClient, workflowManifest, blobInput, fileName, contentType);
                                    CreateJob(mediaClient, workflowManifest, transform, inputFileUrl);
                                    break;
                                case MediaJobInputMode.AssetFile:
                                    Asset inputAsset = await mediaClient.CreateAsset(workflowManifest.MediaStorage, fileName, fileName, blobInput);
                                    string assetFileUrl = MediaClient.GetAssetFileUrl(mediaClient, inputAsset);
                                    CreateJob(mediaClient, workflowManifest, transform, assetFileUrl);
                                    break;
                                case MediaJobInputMode.Asset:
                                    inputAsset = await mediaClient.CreateAsset(workflowManifest.MediaStorage, fileName, fileName, blobInput);
                                    CreateJob(mediaClient, workflowManifest, transform, inputAsset);
                                    break;
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

        private async static Task<string> CopyBlob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Stream blobStream, string fileName, string contentType)
        {
            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, workflowManifest.MediaStorage);
            CloudBlockBlob blob = blobClient.GetBlockBlob(Constant.Storage.Blob.WorkflowContainerName, null, fileName);
            blob.Properties.ContentType = contentType;
            await blob.UploadFromStreamAsync(blobStream);
            //await blob.SetPropertiesAsync();
            return blobClient.GetDownloadUrl(Constant.Storage.Blob.WorkflowContainerName, fileName);
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, string inputFileUrl)
        {
            Uri inputFileUri = new Uri(inputFileUrl);
            string assetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? assetName : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, inputFileUrl, null, jobOutputMode, streamingPolicyName);
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, Asset inputAsset)
        {
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? inputAsset.Name : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, null, inputAsset.Name, jobOutputMode, streamingPolicyName);
        }
    }
}