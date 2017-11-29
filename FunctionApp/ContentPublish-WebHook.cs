using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentPublishWebHook
    {
        private static MediaContentPublish EnqueuePublish(MediaJobNotification jobNotification)
        {
            MediaContentPublish contentPublish = null;
            if (jobNotification.EventType == MediaJobNotificationEvent.JobStateChange &&
                jobNotification.Properties.OldState == JobState.Processing &&
                jobNotification.Properties.NewState == JobState.Finished)
            {
                TableClient tableClient = new TableClient();
                string tableName = Constant.Storage.Table.ContentPublish;
                string partitionKey = jobNotification.Properties.AccountName;
                string rowKey = jobNotification.Properties.JobId;
                contentPublish = tableClient.GetEntity<MediaContentPublish>(tableName, partitionKey, rowKey);
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
                    MediaContentPublish contentPublish = EnqueuePublish(jobNotification);
                    log.Info($"Content Publish: {JsonConvert.SerializeObject(contentPublish)}");
                }
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}