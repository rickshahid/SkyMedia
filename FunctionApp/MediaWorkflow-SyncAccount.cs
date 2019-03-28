using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaWorkflowSyncAccount
    {
        //private const string ScheduleDaily = "0 0 0 * * *";
        //private const string ScheduleWeekly = "0 0 0 * * 1";
        //private const string ScheduleMonthly = "0 0 0 1 * *";

        [FunctionName("MediaWorkflow-SyncAccount")]
        public static void Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer, ILogger logger)
        {
            logger.LogInformation("Media Account Sync @ {0}", DateTime.UtcNow);
        }
    }
}