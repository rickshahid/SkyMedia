using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Twilio;
using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPublishStorageQueue
    {
        [FunctionName("MediaPublish-StorageQueue")]
        [return: TwilioSms(AccountSidSetting = "Twilio.Account.Id", AuthTokenSetting = "Twilio.Account.Token", From = "%Twilio.Message.From%")]
        public static SMSMessage Run([QueueTrigger("media-publish")] string message, TraceWriter log)
        {
            SMSMessage publishMessage = null;
            log.Info($"Queue Message: {message}");
            MediaPublish mediaPublish = JsonConvert.DeserializeObject<MediaPublish>(message);
            if (mediaPublish != null)
            {
                MediaPublished mediaPublished;
                log.Info($"Media Publish: {JsonConvert.SerializeObject(mediaPublish)}");
                if (mediaPublish.MediaInsight)
                {
                    mediaPublished = MediaClient.PublishInsight(mediaPublish);
                }
                else
                {
                    mediaPublished = MediaClient.PublishContent(mediaPublish);
                }
                log.Info($"Media Published: {JsonConvert.SerializeObject(mediaPublished)}");
                if (!string.IsNullOrEmpty(mediaPublished.MobileNumber) &&
                    !string.IsNullOrEmpty(mediaPublished.StatusMessage))
                {
                    publishMessage = new SMSMessage()
                    {
                        To = mediaPublished.MobileNumber,
                        Body = mediaPublished.StatusMessage
                    };
                }
            }
            return publishMessage;
        }
    }
}