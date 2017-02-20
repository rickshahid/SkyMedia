using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpPost]
        [Route("/publish")]
        public void PublishMediaJob([FromBody] bool poisonQueue)
        {
            string settingKey = Constants.AppSettingKey.MediaJobNotificationStorageQueueName;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue)
            {
                queueName = string.Concat(queueName, Constants.Storage.Queue.PoisonSuffix);
            }
            string messageId, popReceipt;
            MessageClient messageClient = new MessageClient();
            string queueMessage = messageClient.GetMessage(queueName, out messageId, out popReceipt);
            MediaJobNotification jobNotification = Newtonsoft.Json.JsonConvert.DeserializeObject<MediaJobNotification>(queueMessage);
            if (jobNotification != null)
            {
                MediaClient.PublishJob(jobNotification, false);
                messageClient.DeleteMessage(queueName, messageId, popReceipt);
            }
        }
    }
}
