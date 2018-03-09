using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentPublishWebHook
    {
        [FunctionName("ContentPublish-WebHook")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request, TraceWriter log)
        {
            StreamReader streamReader = new StreamReader(request.Body);
            string notificationMessage = streamReader.ReadToEnd();
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
            return new OkResult();
        }

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
    }
}