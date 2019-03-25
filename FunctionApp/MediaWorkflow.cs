using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    //public static class MediaWorkflow
    //{
    //    [FunctionName("MediaWorkflow")]
    //    public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
    //    {
    //        List<string> workflowOutputs = new List<string>();

    //        MediaWorkflowManifest workflowManifest = null;

    //        if (true)
    //        {
    //            workflowOutputs.Add(await context.CallActivityAsync<string>("MediaContentWorkflow-CreateAsset", workflowManifest));
    //        }

    //        if (false)
    //        {
    //            workflowOutputs.Add(await context.CallActivityAsync<string>("MediaContentWorkflow-CreateJob", workflowManifest));
    //        }

    //        return workflowOutputs;
    //    }
    //}
}