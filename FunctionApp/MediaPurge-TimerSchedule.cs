using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPurgeTimerSchedule
    {
        [FunctionName("MediaPurge-TimerSchedule")]
        public static void Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TraceWriter log)
        {
            log.Info($"Media Purge @ {DateTime.Now}");
            MediaClient.PurgePublish();
        }
    }
}