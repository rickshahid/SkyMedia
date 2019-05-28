using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        private static readonly StorageBlobClient _blobClient = new StorageBlobClient();
        private const string _containerName = Constant.Storage.Blob.WorkflowContainerName;
        private const StringComparison _stringComparison = StringComparison.OrdinalIgnoreCase;

        [FunctionName("MediaWorkflow")]
        public static async Task Orchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            ValueTuple<EventGridEvent, MediaWorkflowManifest> contextInput = context.GetInput<(EventGridEvent, MediaWorkflowManifest)>();
            EventGridEvent queueMessage = contextInput.Item1;
            MediaWorkflowManifest workflowManifest = contextInput.Item2;

            string inputFileName = workflowManifest.InputFileName;
            if (string.IsNullOrEmpty(inputFileName))
            {
                inputFileName = Path.GetFileName(queueMessage.Subject);
            }

            if (inputFileName.Equals(Constant.Storage.Blob.WorkflowContainerFiles, _stringComparison))
            {
                List<Task> parallelTasks = new List<Task>();
                string[] blobNames = await context.CallActivityAsync<string[]>("MediaWorkflow-BlobList", null);
                foreach (string blobName in blobNames)
                {
                    ValueTuple<MediaWorkflowManifest, string> workflowInput = (workflowManifest, blobName);
                    if (workflowManifest.JobInputMode == MediaJobInputMode.Asset)
                    {
                        Task parallelTask = context.CallSubOrchestratorAsync("MediaWorkflow-InputModeAsset", workflowInput);
                        parallelTasks.Add(parallelTask);
                    }
                    else
                    {
                        Task parallelTask = context.CallSubOrchestratorAsync("MediaWorkflow-InputModeFile", workflowInput);
                        parallelTasks.Add(parallelTask);
                    }
                }
                await Task.WhenAll(parallelTasks);
            }
            else
            {
                ValueTuple<MediaWorkflowManifest, string> workflowInput = (workflowManifest, inputFileName);
                if (workflowManifest.JobInputMode == MediaJobInputMode.Asset)
                {
                    await context.CallSubOrchestratorAsync("MediaWorkflow-InputModeAsset", workflowInput);
                }
                else
                {
                    await context.CallSubOrchestratorAsync("MediaWorkflow-InputModeFile", workflowInput);
                }
            }
        }

        [FunctionName("MediaWorkflow-InputModeAsset")]
        public static async Task SubOrchestratorAsset([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            ValueTuple<MediaWorkflowManifest, string> assetInput = context.GetInput<(MediaWorkflowManifest, string)>();
            Asset inputAsset = await context.CallActivityAsync<Asset>("MediaWorkflow-CreateAsset", assetInput);

            ValueTuple<MediaWorkflowManifest, string, Asset> jobInput = (assetInput.Item1, null, inputAsset);
            await context.CallActivityAsync("MediaWorkflow-CreateJob", jobInput);
        }

        [FunctionName("MediaWorkflow-InputModeFile")]
        public static async Task SubOrchestratorFile([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            ValueTuple<MediaWorkflowManifest, string> fileInput = context.GetInput<(MediaWorkflowManifest, string)>();
            string inputFileUrl = await context.CallActivityAsync<string>("MediaWorkflow-BlobUrl", fileInput.Item2);

            ValueTuple<MediaWorkflowManifest, string, Asset> jobInput = (fileInput.Item1, inputFileUrl, null);
            await context.CallActivityAsync("MediaWorkflow-CreateJob", jobInput);
        }
    }
}