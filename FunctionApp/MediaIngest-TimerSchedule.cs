using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaIngestTimerSchedule
    {
        [FunctionName("MediaIngest-TimerSchedule")]
        public static void Run([TimerTrigger("0 0 0 1 * *")] TimerInfo timer, TraceWriter log)
        {
            log.Info($"Media Ingest @ {DateTime.Now}");
            WebClient.SendAsync("http://www.skymedia.tv/gallery/refresh");
        }
    }
}