using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Twilio;
using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentPublishStorageQueue
    {
        [FunctionName("ContentPublish-StorageQueue")]
        [return: TwilioSms(AccountSidSetting = "Twilio.Account.Id", AuthTokenSetting = "Twilio.Account.Token", From = "%Twilio.Message.From%")]
        public static SMSMessage Run([QueueTrigger("publish-content")] string message, TraceWriter log)
        {
            SMSMessage publishMessage = null;
            log.Info($"Queue Message: {message}");
            MediaPublish contentPublish = JsonConvert.DeserializeObject<MediaPublish>(message);
            if (contentPublish != null)
            {
                log.Info($"Content Publish: {JsonConvert.SerializeObject(contentPublish)}");
                MediaPublished contentPublished = MediaClient.PublishContent(contentPublish);
                log.Info($"Content Published: {JsonConvert.SerializeObject(contentPublished)}");
                if (!string.IsNullOrEmpty(contentPublished.MobileNumber) &&
                    !string.IsNullOrEmpty(contentPublished.StatusMessage))
                {
                    publishMessage = new SMSMessage()
                    {
                        To = contentPublished.MobileNumber,
                        Body = contentPublished.StatusMessage
                    };
                }
            }
            return publishMessage;
        }
    }
}