using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public static MediaPublished PublishOutput(MediaPublish mediaPublish)
        {
            MediaPublished mediaPublished = null;
            using (MediaClient mediaClient = new MediaClient(null, mediaPublish.MediaAccount))
            {
                string jobName = mediaPublish.Id;
                string transformName = mediaPublish.TransformName;
                Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
                if (job != null)
                {
                    foreach (JobOutputAsset jobOutput in job.Outputs)
                    {
                        mediaClient.CreateLocator(jobOutput.AssetName, PredefinedStreamingPolicy.ClearStreamingOnly);
                    }
                    mediaPublished = new MediaPublished()
                    {
                        MobileNumber = mediaPublish.MobileNumber,
                        UserMessage = GetNotificationMessage(mediaPublish.MediaAccount, job)
                    };
                }
            }
            //    string indexId = null;
            //    mediaClient.SetProcessorUnits(job, ReservedUnitType.Basic, false);
            //    PublishJob(mediaClient, job);
            //    if (mediaPublish.InsightConfig != null)
            //    {
            //        IAsset encoderOutput = GetEncoderOutput(job);
            //        VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaPublish.MediaAccount);
            //        indexId = videoAnalyzer.StartAnalysis(mediaClient, encoderOutput, mediaPublish.InsightConfig);
            //    }
            return mediaPublished;
        }

        public static void PurgePublish()
        {
            //using (DatabaseClient databaseClient = new DatabaseClient())
            //{
            //    string collectionId = Constant.Database.Collection.OutputInsight;
            //    JObject[] documents = databaseClient.GetDocuments(collectionId);
            //    foreach (JObject document in documents)
            //    {
            //        MediaAccount mediaAccount = GetMediaAccount(document);
            //        using MediaClient mediaClient = new MediaClient(null, mediaAccount);
            //        string assetId = document["id"].ToString();
            //        IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
            //        if (asset == null)
            //        {
            //            databaseClient.DeleteDocument(collectionId, assetId);
            //        }
            //    }

            //    collectionId = Constant.Database.Collection.OutputPublish;
            //    documents = databaseClient.GetDocuments(collectionId);
            //    foreach (JObject document in documents)
            //    {
            //        MediaAccount mediaAccount = GetMediaAccount(document);
            //        using MediaClient mediaClient = new MediaClient(null, mediaAccount);
            //        string jobId = document["id"].ToString();
            //        IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;
            //        if (job == null)
            //        {
            //            JToken taskIds = document["TaskIds"];
            //            foreach (JToken taskId in taskIds)
            //            {
            //                databaseClient.DeleteDocument(collectionId, taskId.ToString());
            //            }
            //            databaseClient.DeleteDocument(collectionId, jobId);
            //        }
            //    }
            //}
        }
    }
}