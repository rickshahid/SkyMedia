using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    //public static class MediaWorkflowSyncContent
    //{
    //    //private const string ScheduleDaily = "0 0 0 * * *";
    //    //private const string ScheduleWeekly = "0 0 0 * * 1";
    //    //private const string ScheduleMonthly = "0 0 0 1 * *";

    //    [FunctionName("MediaWorkflow-SyncContent")]
    //    public static void Run([TimerTrigger("%Media.Sync.Schedule%")] TimerInfo timer, ILogger logger)
    //    {
    //        logger.LogInformation("Media Sync @ {0}", DateTime.UtcNow);
    //        SyncMediaJobs();
    //    }

    //    private static void SyncMediaJobs()
    //    {
    //        DatabaseClient databaseClient = new DatabaseClient(true);
    //        string collectionId = Constant.Database.Collection.MediaJobAccount;
    //        MediaJobAccount[] jobAccounts = databaseClient.GetDocuments<MediaJobAccount>(collectionId);
    //        foreach (MediaJobAccount jobAccount in jobAccounts)
    //        {
    //            bool entityFound = false;
    //            try
    //            {
    //                using (MediaClient mediaClient = new MediaClient(jobAccount.MediaAccount, null))
    //                {
    //                    if (!string.IsNullOrEmpty(jobAccount.InsightId))
    //                    {
    //                        JObject insight = mediaClient.IndexerGetInsight(jobAccount.InsightId);
    //                        entityFound = insight != null;
    //                    }
    //                    else
    //                    {
    //                        Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobAccount.JobName);
    //                        entityFound = job != null;
    //                    }
    //                }
    //            }
    //            finally
    //            {
    //                if (!entityFound)
    //                {
    //                    databaseClient.DeleteDocument(collectionId, jobAccount.Id);
    //                }
    //            }
    //        }
    //    }
    //}
}
