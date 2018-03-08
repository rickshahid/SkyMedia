using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentPublishStorageQueue
    {
        [FunctionName("ContentPublish-StorageQueue")]
        public static void Run([QueueTrigger("publish-content")] string message, TraceWriter log)
        {
            log.Info($"Queue Message: {message}");
            MediaPublish contentPublish = JsonConvert.DeserializeObject<MediaPublish>(message);
            if (contentPublish != null)
            {
                log.Info($"Content Publish: {JsonConvert.SerializeObject(contentPublish)}");
                MediaPublished contentPublished = MediaClient.PublishContent(contentPublish);
                log.Info($"Content Published: {JsonConvert.SerializeObject(contentPublished)}");
            }
        }
    }
}