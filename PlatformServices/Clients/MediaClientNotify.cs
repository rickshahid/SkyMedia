using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static string GetNotificationMessage(string accountId, IJob job)
        {
            string messageText = string.Concat("Azure Media Services Job ", job.State.ToString(), ".");
            messageText = string.Concat(messageText, " Account Id: ", accountId);
            messageText = string.Concat(messageText, ", Job Id: ", job.Id);
            messageText = string.Concat(messageText, ", Job Name: ", job.Name);
            return string.Concat(messageText, ", Job Running Duration: ", job.RunningDuration.ToString(Constant.TextFormatter.ClockTime));
        }
    }
}