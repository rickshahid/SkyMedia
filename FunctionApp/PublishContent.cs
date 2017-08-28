using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class PublishContent
    {
        private static MediaContentPublish EnqueuePublish(MediaJobNotification jobNotification)
        {
            MediaContentPublish contentPublish = null;
            if (jobNotification.EventType == MediaJobNotificationEvent.JobStateChange &&
               (jobNotification.Properties.NewState == JobState.Error ||
                jobNotification.Properties.NewState == JobState.Canceled ||
                jobNotification.Properties.NewState == JobState.Finished))
            {
                EntityClient entityClient = new EntityClient();
                string tableName = Constant.Storage.TableName.ContentPublish;
                string partitionKey = jobNotification.Properties.AccountName;
                string rowKey = jobNotification.Properties.JobId;
                contentPublish = entityClient.GetEntity<MediaContentPublish>(tableName, partitionKey, rowKey);
                if (contentPublish != null)
                {
                    string settingKey = Constant.AppSettingKey.MediaPublishContentQueue;
                    string queueName = AppSetting.GetValue(settingKey);
                    MessageClient messageClient = new MessageClient();
                    messageClient.AddMessage(queueName, contentPublish);
                }
            }
            return contentPublish;
        }

        [FunctionName("PublishContent")]
        public static async Task<object> Run([HttpTrigger(AuthorizationLevel.Function, "post", WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
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