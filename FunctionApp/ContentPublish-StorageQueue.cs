using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentPublishStorageQueue
    {
        [FunctionName("ContentPublish-StorageQueue")]
        public static void Run([QueueTrigger("publish-content")] string queueMessage, TraceWriter log)
        {
            log.Info($"Queue Message: {queueMessage}");
            MediaContentPublish contentPublish = JsonConvert.DeserializeObject<MediaContentPublish>(queueMessage);
            if (contentPublish != null)
            {
                MediaClient.PublishContent(contentPublish);
                log.Info($"Content Publish: {JsonConvert.SerializeObject(contentPublish)}");
            }
        }
    }
}