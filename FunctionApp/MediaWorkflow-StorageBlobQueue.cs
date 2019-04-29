using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaWorkflowStorageBlobQueue
    {
        [FunctionName("MediaWorkflow-StorageBlobQueue")]
        public static void Run([QueueTrigger(Constant.Storage.Blob.WorkflowContainerName)] string queueMessage, ILogger logger)
        {
            try
            {
                logger.LogInformation(queueMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
    }
}