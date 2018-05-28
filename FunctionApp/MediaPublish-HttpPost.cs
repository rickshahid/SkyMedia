using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPublishHttpPost
    {
        [FunctionName("MediaPublish-HttpPost")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage request, TraceWriter log)
        {
            string notificationMessage = await request.Content.ReadAsStringAsync();
            log.Info($"Notification Message: {notificationMessage}");

            if (request.Properties.ContainsKey("id"))
            {
                string indexId = request.Properties["id"].ToString();
                MediaPublish mediaPublish = EnqueuePublish(indexId);
                log.Info($"Media Publish: {JsonConvert.SerializeObject(mediaPublish)}");
            }
            else
            {
                MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(notificationMessage);
                if (jobNotification != null)
                {
                    MediaPublish mediaPublish = EnqueuePublish(jobNotification);
                    log.Info($"Media Publish: {JsonConvert.SerializeObject(mediaPublish)}");
                }
            }

            return request.CreateResponse(HttpStatusCode.OK);
        }

        private static MediaPublish EnqueuePublish(MediaJobNotification jobNotification)
        {
            MediaPublish mediaPublish = null;
            if (jobNotification.EventType == MediaJobNotificationEvent.JobStateChange &&
                jobNotification.Properties.OldState == MediaJobState.Processing &&
                jobNotification.Properties.NewState == MediaJobState.Finished)
            {
                mediaPublish = EnqueuePublish(jobNotification.Properties.JobId);
            }
            return mediaPublish;
        }

        private static MediaPublish EnqueuePublish(string documentId)
        {
            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constant.Database.Collection.OutputPublish;
            MediaPublish mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, documentId);
            if (mediaPublish != null)
            {
                string settingKey = Constant.AppSettingKey.MediaPublishQueue;
                string queueName = AppSetting.GetValue(settingKey);
                QueueClient queueClient = new QueueClient();
                queueClient.AddMessage(queueName, mediaPublish);
            }
            return mediaPublish;
        }
    }
}