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
            MediaPublish insightPublish = JsonConvert.DeserializeObject<MediaPublish>(queueMessage);
            if (insightPublish != null)
            {
                MediaPublished insightPublished = MediaClient.PublishInsight(insightPublish);
                log.Info($"Insight Publish: {JsonConvert.SerializeObject(insightPublish)}");
                log.Info($"Insight Published: {JsonConvert.SerializeObject(insightPublished)}");
            }
        }
    }
}