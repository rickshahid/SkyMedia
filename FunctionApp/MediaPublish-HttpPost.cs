using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            IEnumerable<KeyValuePair<string, string>> urlParameters = request.GetQueryNameValuePairs();
            string indexId = urlParameters.SingleOrDefault(q => string.Equals(q.Key, "id", StringComparison.OrdinalIgnoreCase)).Value;
            log.Info($"Index Id: {indexId}");

            if (!string.IsNullOrEmpty(notificationMessage))
            {
                MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(notificationMessage);
                if (jobNotification != null)
                {
                    MediaPublish mediaPublish = EnqueuePublish(jobNotification);
                    log.Info($"Media Publish: {JsonConvert.SerializeObject(mediaPublish)}");
                }
            }
            else if (!string.IsNullOrEmpty(indexId))
            {
                MediaPublish mediaPublish = EnqueuePublish(indexId);
                log.Info($"Media Publish: {JsonConvert.SerializeObject(mediaPublish)}");
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
                DatabaseClient databaseClient = new DatabaseClient();
                string collectionId = Constant.Database.Collection.MediaPublish;
                string documentId = jobNotification.Properties.JobId;
                mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, documentId);
                if (mediaPublish != null)
                {
                    string settingKey = Constant.AppSettingKey.MediaPublishQueue;
                    string queueName = AppSetting.GetValue(settingKey);
                    QueueClient queueClient = new QueueClient();
                    queueClient.AddMessage(queueName, mediaPublish);
                }
            }
            return mediaPublish;
        }

        private static MediaPublish EnqueuePublish(string indexId)
        {
            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constant.Database.Collection.MediaPublish;
            MediaPublish mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, indexId);
            if (mediaPublish != null)
            {
                mediaPublish.MediaInsight = true;
                string settingKey = Constant.AppSettingKey.MediaPublishQueue;
                string queueName = AppSetting.GetValue(settingKey);
                QueueClient queueClient = new QueueClient();
                queueClient.AddMessage(queueName, mediaPublish);
            }
            return mediaPublish;
        }
    }
}