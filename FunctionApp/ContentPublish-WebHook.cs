using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentPublishWebHook
    {
        private static MediaPublish EnqueuePublish(MediaJobNotification jobNotification)
        {
            MediaPublish contentPublish = null;
            if (jobNotification.EventType == MediaJobNotificationEvent.JobStateChange &&
                jobNotification.Properties.OldState == MediaJobState.Processing &&
                jobNotification.Properties.NewState == MediaJobState.Finished)
            {
                TableClient tableClient = new TableClient();
                string tableName = Constant.Storage.Table.ContentPublish;
                string partitionKey = jobNotification.Properties.AccountName;
                string rowKey = jobNotification.Properties.JobId;
                contentPublish = tableClient.GetEntity<MediaPublish>(tableName, partitionKey, rowKey);
                if (contentPublish != null)
                {
                    string settingKey = Constant.AppSettingKey.MediaPublishContentQueue;
                    string queueName = AppSetting.GetValue(settingKey);
                    QueueClient queueClient = new QueueClient();
                    queueClient.AddMessage(queueName, contentPublish);
                }
            }
            return contentPublish;
        }

        [FunctionName("ContentPublish-WebHook")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")] HttpRequestMessage req, TraceWriter log)
        {
            string notificationMessage = await req.Content.ReadAsStringAsync();
            log.Info($"Notification Message: {notificationMessage}");
            if (!string.IsNullOrEmpty(notificationMessage))
            {
                MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(notificationMessage);
                if (jobNotification != null)
                {
                    MediaPublish contentPublish = EnqueuePublish(jobNotification);
                    log.Info($"Content Publish: {JsonConvert.SerializeObject(contentPublish)}");
                }
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}