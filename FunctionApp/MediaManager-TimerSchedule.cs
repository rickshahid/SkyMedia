using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaManagerTimerSchedule
    {
        [FunctionName("MediaManager-TimerSchedule")]
        public static void Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TraceWriter log)
        {
            try
            {
                log.Info($"Media Manager @ {DateTime.Now}");
                MediaClient.PurgePublish();
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.ToString()}");
            }
        }
    }
}