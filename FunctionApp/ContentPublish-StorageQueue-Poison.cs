using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentPublishStorageQueuePoison
    {
        [FunctionName("ContentPublish-StorageQueue-Poison")]
        public static void Run([QueueTrigger("publish-content-poison"), Disable()]string queueMessage, TraceWriter log)
        {
            log.Info($"Queue Message: {queueMessage}");
        }
    }
}