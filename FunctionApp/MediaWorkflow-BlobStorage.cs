using System;
using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaWorkflowBlobStorage
    {
        [FunctionName("MediaWorkflow-BlobStorage")]
        public static void Run([EventGridTrigger] EventGridEvent eventTrigger, [Blob("{data.url}", FileAccess.Read)] Stream blobInput,
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
                            Transform transform = mediaClient.CreateTransform(workflowManifest.TransformPresets);
                            switch (workflowManifest.JobInputMode)
                            {
                                case MediaJobInputMode.InputFile:
                                    string contentType = eventData["contentType"].ToString();
                                    string inputFileUrl = CreateBlob(mediaClient, workflowManifest, blobInput, fileName, contentType);
                                    CreateJob(mediaClient, workflowManifest, transform, inputFileUrl, logger);
                                    break;
                                case MediaJobInputMode.AssetFile:
                                    Asset inputAsset = mediaClient.CreateAsset(workflowManifest.MediaStorage, fileName, fileName, blobInput);
                                    string assetFileUrl = MediaClient.GetAssetFileUrl(mediaClient, inputAsset);
                                    CreateJob(mediaClient, workflowManifest, transform, assetFileUrl, logger);
                                    break;
                                case MediaJobInputMode.Asset:
                                    inputAsset = mediaClient.CreateAsset(workflowManifest.MediaStorage, fileName, fileName, blobInput);
                                    CreateJob(mediaClient, workflowManifest, transform, inputAsset, logger);
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

        private static string CreateBlob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Stream blobStream, string fileName, string contentType)
        {
            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, workflowManifest.MediaStorage);
            CloudBlockBlob blob = blobClient.GetBlockBlob(Constant.Storage.Blob.WorkflowContainerName, null, fileName);
            blob.UploadFromStreamAsync(blobStream).Wait();
            blob.Properties.ContentType = contentType;
            blob.SetPropertiesAsync();
            return blobClient.GetDownloadUrl(Constant.Storage.Blob.WorkflowContainerName, fileName, false);
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, string inputFileUrl, ILogger logger)
        {
            Uri inputFileUri = new Uri(inputFileUrl);
            string assetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? assetName : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, inputFileUrl, null, jobOutputMode, streamingPolicyName);
        }

        private static void CreateJob(MediaClient mediaClient, MediaWorkflowManifest workflowManifest, Transform transform, Asset inputAsset, ILogger logger)
        {
            string jobName = string.IsNullOrEmpty(workflowManifest.JobName) ? inputAsset.Name : workflowManifest.JobName;
            Priority jobPriority = workflowManifest.JobPriority;
            MediaJobOutputMode jobOutputMode = workflowManifest.JobOutputMode;
            string streamingPolicyName = workflowManifest.StreamingPolicyName;
            mediaClient.CreateJob(transform.Name, jobName, null, jobPriority, null, null, inputAsset.Name, jobOutputMode, streamingPolicyName);
        }
    }
}