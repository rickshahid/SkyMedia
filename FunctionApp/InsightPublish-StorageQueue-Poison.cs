using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AzureSkyMedia.FunctionApp
{
    public static class InsightPublishStorageQueuePoison
    {
        [FunctionName("InsightPublish-StorageQueue-Poison")]
        public static void Run([QueueTrigger("publish-insight-poison"), Disable()]string queueMessage, TraceWriter log)
        {
            log.Info($"Queue Message: {queueMessage}");
        }
    }
}