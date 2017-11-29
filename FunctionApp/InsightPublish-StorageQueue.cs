using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class InsightPublishStorageQueue
    {
        [FunctionName("InsightPublish-StorageQueue")]
        public static void Run([QueueTrigger("publish-insight")] string queueMessage, TraceWriter log)
        {
            log.Info($"Queue Message: {queueMessage}");
            MediaInsightPublish insightPublish = JsonConvert.DeserializeObject<MediaInsightPublish>(queueMessage);
            if (insightPublish != null)
            {
                MediaClient.PublishInsight(insightPublish);
                log.Info($"Insight Publish: {JsonConvert.SerializeObject(insightPublish)}");
            }

        }
    }
}