using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

//using Twilio;
using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPublishStorageQueue
    {
        [FunctionName("MediaPublish-StorageQueue")]
        //[return: TwilioSms(AccountSidSetting = "Twilio.Account.Id", AuthTokenSetting = "Twilio.Account.Token", From = "%Twilio.Message.From%")]
        //public static SMSMessage Run([QueueTrigger("media-publish")] string message, TraceWriter log)
        public static void Run([QueueTrigger("media-publish")] string message, TraceWriter log)
        {
            //SMSMessage smsMessage = null;
            try
            {
                log.Info($"Queue Message: {message}");
                MediaPublish mediaPublish = JsonConvert.DeserializeObject<MediaPublish>(message);
                if (mediaPublish != null)
                {
                    MediaPublished mediaPublished = MediaClient.PublishOutput(mediaPublish);
                    log.Info($"Media Published: {JsonConvert.SerializeObject(mediaPublished)}");
                    if (!string.IsNullOrEmpty(mediaPublished.MobileNumber) &&
                        !string.IsNullOrEmpty(mediaPublished.UserMessage))
                    {
                        //smsMessage = new SMSMessage()
                        //{
                        //    To = mediaPublished.MobileNumber,
                        //    Body = mediaPublished.UserMessage
                        //};
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.ToString()}");
            }
            //return smsMessage;
        }
    }
}