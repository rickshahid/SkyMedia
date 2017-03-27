using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class jobController : Controller
    {
        [HttpPost]
        [Route("/publish")]
        public void Publish(string accountName, string accountKey, bool poisonQueue)
        {
            string settingKey = Constant.AppSettingKey.MediaJobNotificationStorageQueueName;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue)
            {
                queueName = string.Concat(queueName, Constant.Storage.Queue.PoisonSuffix);
            }
            string messageId, popReceipt;
            string[] accountCredentials = new string[] { accountName, accountKey };
            MessageClient messageClient = string.IsNullOrEmpty(accountName) ? new MessageClient() : new MessageClient(accountCredentials);
            string queueMessage = messageClient.GetMessage(queueName, out messageId, out popReceipt);
            MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(queueMessage);
            if (jobNotification != null)
            {
                MediaClient.PublishJob(jobNotification, false);
                messageClient.DeleteMessage(queueName, messageId, popReceipt);
            }
        }
    }
}
