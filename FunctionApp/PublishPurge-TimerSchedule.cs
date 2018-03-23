using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class PublishPurgeTimerSchedule
    {
        [FunctionName("PublishPurge-TimerSchedule")]
        public static void Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TraceWriter log)
        {
            log.Info($"Publish Purge @ {DateTime.Now}");
            TableClient tableClient = new TableClient();
            MediaClient.PurgePublishContent(tableClient);
            MediaClient.PurgePublishInsight(tableClient);
        }
    }
}