using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaSyncTimerSchedule
    {
        //private const string ScheduleDaily = "0 0 0 * * *";
        //private const string ScheduleWeekly = "0 0 0 * * 1";
        //private const string ScheduleMonthly = "0 0 0 1 * *";

        [FunctionName("MediaSyncTimerSchedule")]
        public static void Run([TimerTrigger("%Media.Sync.Schedule%")] TimerInfo timer, ILogger logger)
        {
            logger.LogInformation("Media Sync @ {0}", DateTime.UtcNow);
            SyncMediaJobs();
        }

        private static void SyncMediaJobs()
        {
            DatabaseClient databaseClient = new DatabaseClient(true);
            string collectionId = Constant.Database.Collection.MediaJobAccount;
            MediaJobAccount[] jobAccounts = databaseClient.GetDocuments<MediaJobAccount>(collectionId);
            foreach (MediaJobAccount jobAccount in jobAccounts)
            {
                Job job = null;
                try
                {
                    using (MediaClient mediaClient = new MediaClient(null, jobAccount.MediaAccount))
                    {
                        job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobAccount.JobName);
                    }
                }
                finally
                {
                    if (job == null)
                    {
                        databaseClient.DeleteDocument(collectionId, jobAccount.JobName);
                    }
                }
            }
        }
    }
}