using System;
using System.Web.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPublishStorageQueue
    {
        [FunctionName("MediaPublish-StorageQueue")]
        public static void Run([QueueTrigger("%Media.Publish.Queue%")] string message, ILogger logger)
        {
            try
            {
                logger.LogInformation("Queue Message: {0}", message);
                MediaPublish mediaPublish = JsonConvert.DeserializeObject<MediaPublish>(message);
                if (mediaPublish != null)
                {
                    string publishMessage = MediaClient.PublishJobOutput(mediaPublish);
                    logger.LogInformation("Publish Message: {0}", publishMessage);
                }
            }
            catch (HttpResponseException ex)
            {
                logger.LogError(ex, "HTTP Exception: {0}", ex.Response.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception: {0}", ex.ToString());
            }
        }
    }
}