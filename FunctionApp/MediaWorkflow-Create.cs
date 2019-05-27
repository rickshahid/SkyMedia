using System;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-Create")]
        public static async Task SubOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            ValueTuple<MediaWorkflowManifest, string> assetInput = context.GetInput<(MediaWorkflowManifest, string)>();
            Asset inputAsset = await context.CallActivityAsync<Asset>("MediaWorkflow-CreateAsset", assetInput);
            _logger.LogInformation(Constant.Message.AssetCreated, inputAsset.Name);

            ValueTuple<MediaWorkflowManifest, string, Asset> jobInput = (assetInput.Item1, null, inputAsset);
            Job job = await context.CallActivityAsync<Job>("MediaWorkflow-CreateJob", jobInput);
            _logger.LogInformation(Constant.Message.JobCreated, job.Name);
        }
    }
}