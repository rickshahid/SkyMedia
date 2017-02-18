using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/processors")]
        public object GetMediaProcessors([FromBody] string accountName, [FromBody] string accountKey)
        {
            MediaClient mediaClient = new MediaClient(accountName, accountKey);
            return mediaClient.GetEntities(MediaEntity.Processor);
        }

        [HttpPost]
        [Route("/publish")]
        public void PublishMediaJob([FromBody] MediaJobNotification jobNotification, [FromBody] bool webHook, [FromBody] bool poisonQueue)
        {
            if (jobNotification == null || string.IsNullOrEmpty(jobNotification.ETag))
            {
                string settingKey = Constants.AppSettingKey.MediaJobNotificationStorageQueueName;
                string queueName = AppSetting.GetValue(settingKey);
                if (poisonQueue)
                {
                    queueName = string.Concat(queueName, "-poison");
                }
                string messageId, popReceipt;
                MessageClient messageClient = new MessageClient();
                string queueMessage = messageClient.GetMessage(queueName, out messageId, out popReceipt);
                jobNotification = Newtonsoft.Json.JsonConvert.DeserializeObject<MediaJobNotification>(queueMessage);
                if (jobNotification != null)
                {
                    MediaClient.PublishJob(jobNotification, false);
                    messageClient.DeleteMessage(queueName, messageId, popReceipt);
                }
            }
            else
            {
                MediaClient.PublishJob(jobNotification, webHook);
            }
        }

        [HttpPost]
        [Route("/message")]
        public void SendSMSText([FromBody] string messageText, [FromBody] string mobileNumber)
        {
            MessageClient.SendText(messageText, mobileNumber);
        }
    }
}
