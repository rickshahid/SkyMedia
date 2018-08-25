using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaIngestTimerSchedule
    {
        private const string ScheduleDaily = "0 0 0 * * *";
        private const string ScheduleWeekly = "0 0 0 * * 1";
        private const string ScheduleMonthly = "0 0 0 1 * *";

        [FunctionName("MediaIngest-TimerSchedule")]
        public static void Run([TimerTrigger(ScheduleDaily)] TimerInfo timer, ILogger logger)
        {
            logger.LogInformation("Media Ingest @ {0}", DateTime.Now);
            WebClient.SendAsync("http://www.skymedia.tv/gallery/refresh");
        }
    }
}