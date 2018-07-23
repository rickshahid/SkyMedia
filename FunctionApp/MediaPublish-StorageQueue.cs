using System;
using System.Web.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPublishStorageQueue
    {
        [FunctionName("MediaPublish-StorageQueue")]
        public static void Run([QueueTrigger("media-publish")] string message, TraceWriter log)
        {
            try
            {
                log.Info($"Queue Message: {message}");
                MediaPublish mediaPublish = JsonConvert.DeserializeObject<MediaPublish>(message);
                if (mediaPublish != null)
                {
                    string publishMessage = MediaClient.PublishOutput(mediaPublish);
                    log.Info($"Publish Message: {publishMessage}");
                }
            }
            catch (HttpResponseException ex)
            {
                log.Info($"HTTP Exception: {ex.ToString()}");
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.ToString()}");
            }
        }
    }
}