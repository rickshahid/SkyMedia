using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class InsightPublishStorageQueue
    {
        [FunctionName("InsightPublish-StorageQueue")]
        public static void Run([QueueTrigger("publish-insight")] string message, TraceWriter log)
        {
            log.Info($"Queue Message: {message}");
            MediaPublish insightPublish = JsonConvert.DeserializeObject<MediaPublish>(message);
            if (insightPublish != null)
            {
                log.Info($"Insight Publish: {JsonConvert.SerializeObject(insightPublish)}");
                MediaPublished insightPublished = MediaClient.PublishInsight(insightPublish);
                log.Info($"Insight Published: {JsonConvert.SerializeObject(insightPublished)}");
            }
        }
    }
}