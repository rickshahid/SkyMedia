using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-CreateAsset")]
        public static Asset CreateAsset([ActivityTrigger] DurableActivityContext context)
        {
            ValueTuple<MediaWorkflowManifest, string> jobInput = context.GetInput<(MediaWorkflowManifest, string)>();
            MediaWorkflowManifest workflowManifest = jobInput.Item1;
            string fileName = jobInput.Item2;

            MediaClient mediaClient = new MediaClient(workflowManifest);
            CloudBlockBlob blob = _blobClient.GetBlockBlob(_containerName, null, fileName);
            return mediaClient.CreateAsset(workflowManifest.OutputAssetStorage, workflowManifest.AssetName, blob);
        }
    }
}