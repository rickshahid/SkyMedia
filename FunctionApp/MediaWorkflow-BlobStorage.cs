using System;
using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaWorkflowBlobStorage
    {
        [FunctionName("MediaWorkflow-BlobStorage")]
        public static async void Run([EventGridTrigger] EventGridEvent eventTrigger, [Blob("{data.url}", FileAccess.ReadWrite)] CloudBlockBlob blobInput, ILogger logger)
        {
            logger.LogInformation(eventTrigger.Data.ToString());
            logger.LogInformation(blobInput.Name);

            if (blobInput.Name.EndsWith(Constant.FileExtension.Json, StringComparison.OrdinalIgnoreCase))
            {
                string workflowManifest = await blobInput.DownloadTextAsync();
                logger.LogInformation(workflowManifest);
            }
            else
            {

            }
        }
    }
}