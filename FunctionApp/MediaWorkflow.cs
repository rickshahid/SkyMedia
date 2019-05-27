using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        private static ILogger _logger;
        private static readonly StorageBlobClient _blobClient = new StorageBlobClient();
        private const string _containerName = Constant.Storage.Blob.WorkflowContainerName;
        private const StringComparison _stringComparison = StringComparison.OrdinalIgnoreCase;

        [FunctionName("MediaWorkflow")]
        public static async Task Orchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            ValueTuple<EventGridEvent, MediaWorkflowManifest> workflowInput = context.GetInput<(EventGridEvent, MediaWorkflowManifest)>();
            EventGridEvent queueMessage = workflowInput.Item1;
            MediaWorkflowManifest workflowManifest = workflowInput.Item2;
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
                            ValueTuple<MediaWorkflowManifest, string> assetInput = (workflowManifest, blob.Name);
                            context.CallSubOrchestratorAsync("MediaWorkflow-Create", assetInput);
                        }
                        else
                        {
                            string inputFileUrl = _blobClient.GetDownloadUrl(_containerName, blob.Name);
                            ValueTuple<MediaWorkflowManifest, string, string> jobInput = (workflowManifest, inputFileUrl, null);
                            Job job = await context.CallActivityAsync<Job>("MediaWorkflow-CreateJob", jobInput);
                            _logger.LogInformation(Constant.Message.JobCreated, job.Name);
                        }
                    }
                }
            }
            else
            {
                if (workflowManifest.JobInputMode == MediaJobInputMode.Asset)
                {
                    ValueTuple<MediaWorkflowManifest, string> assetInput = (workflowManifest, inputFileName);
                    context.CallSubOrchestratorAsync("MediaWorkflow-Create", assetInput);
                }
                else
                {
                    string inputFileUrl = _blobClient.GetDownloadUrl(_containerName, inputFileName);
                    ValueTuple<MediaWorkflowManifest, string, string> jobInput = (workflowManifest, inputFileUrl, null);
                    Job job = await context.CallActivityAsync<Job>("MediaWorkflow-CreateJob", jobInput);
                    _logger.LogInformation(Constant.Message.JobCreated, job.Name);
                }
            }
        }
    }
}