using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        //private static IAsset GetEncoderOutput(IJob job)
        //{
        //    IAsset encoderOutput = null;
        //    foreach (ITask jobTask in job.Tasks)
        //    {
        //        if (string.Equals(jobTask.MediaProcessorId, Constant.Media.ProcessorId.EncoderStandard, StringComparison.OrdinalIgnoreCase) ||
        //            string.Equals(jobTask.MediaProcessorId, Constant.Media.ProcessorId.EncoderPremium, StringComparison.OrdinalIgnoreCase))
        //        {
        //            encoderOutput = jobTask.OutputAssets[0];
        //        }
        //    }
        //    return encoderOutput;
        //}

        private static void UpsertInsight(JObject index)
        {
            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constant.Database.Collection.OutputInsight;
            try
            {
                databaseClient.UpsertDocument(collectionId, index);
            }
            catch (Exception ex)
            {
                MediaPublishError publishError = new MediaPublishError()
                {
                    Id = index["id"].ToString(),
                    Exception = ex
                };
                databaseClient.UpsertDocument(collectionId, publishError);
            }
        }

        private static MediaAccount GetMediaAccount(JObject document)
        {
            MediaAccount mediaAccount = new MediaAccount()
            {
                Name = document["MediaAccount"]["Name"].ToString(),
                SubscriptionId = document["MediaAccount"]["SubscriptionId"].ToString(),
                ResourceGroupName = document["MediaAccount"]["ResourceGroupName"].ToString(),
                DirectoryTenantId = document["MediaAccount"]["DirectoryTenantId"].ToString(),
                ClientApplicationId = document["MediaAccount"]["ClientApplicationId"].ToString(),
                ClientApplicationKey = document["MediaAccount"]["ClientApplicationKey"].ToString()
            };
            return mediaAccount;
        }

        //private static void PublishAsset(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection, LocatorType locatorType)
        //{
        //    string locatorId = null;
        //    ILocator[] locators = asset.Locators.Where(l => l.Type == locatorType).ToArray();
        //    foreach (ILocator locator in locators)
        //    {
        //        if (locatorId == null)
        //        {
        //            locatorId = locator.Id;
        //        }
        //        locator.Delete();
        //    }
        //    if (contentProtection != null)
        //    {
        //        mediaClient.SetDeliveryPolicies(asset, contentProtection);
        //    }
        //    mediaClient.CreateLocator(locatorId, locatorType, asset, false);
        //}

        //private static void PublishWebVtt(MediaClient mediaClient, ITask analyzerTask, IAsset encoderOutput)
        //{
        //    if (encoderOutput != null)
        //    {
        //        IAsset analyzerOutput = analyzerTask.OutputAssets[0];
        //        string[] webVttFiles = GetAssetFiles(mediaClient.MediaAccount, analyzerOutput, Constant.Media.FileExtension.WebVtt);
        //        foreach (string webVttFile in webVttFiles)
        //        {
        //            string languageId = Media.GetLanguageId(analyzerTask.Configuration);
        //            string webVttFileName = string.Concat(MediaProcessor.AudioAnalyzer, Constant.TextDelimiter.Identifier, languageId, Constant.Media.FileExtension.WebVtt);
        //            string webVttUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, analyzerOutput, webVttFile, false);
        //            using (Stream webVttStream = WebClient.GetStream(webVttUrl))
        //            {
        //                IAssetFile encoderOutputFile = encoderOutput.AssetFiles.Create(webVttFileName);
        //                encoderOutputFile.Upload(webVttStream);
        //            }
        //        }
        //    }
        //}

        //private static void PublishInsight(MediaClient mediaClient, DatabaseClient databaseClient, ITask insightTask, IAsset encoderOutput)
        //{
        //    MediaInsight mediaInsight = null;
        //    List<MediaInsightSource> insightSources = null;

        //    string collectionId = Constant.Database.Collection.OutputInsight;
        //    string insightId = encoderOutput == null ? null : encoderOutput.AlternateId;

        //    if (!string.IsNullOrEmpty(insightId))
        //    {
        //        mediaInsight = databaseClient.GetDocument<MediaInsight>(collectionId, insightId);
        //    }
        //    if (mediaInsight == null)
        //    {
        //        mediaInsight = new MediaInsight();
        //        mediaInsight.Id = databaseClient.UpsertDocument(collectionId, mediaInsight);
        //    }
        //    if (mediaInsight.Sources != null)
        //    {
        //        insightSources = new List<MediaInsightSource>(mediaInsight.Sources);
        //    }
        //    else
        //    {
        //        insightSources = new List<MediaInsightSource>();
        //    }

        //    MediaProcessor? insightProcessor = Processor.GetMediaProcessor(insightTask.MediaProcessorId);
        //    IAsset insightOutput = insightTask.OutputAssets[0];
        //    //string[] insightFiles = GetAssetFiles(mediaClient.MediaAccount, insightOutput, Constant.Media.FileExtension.Json);
        //    //foreach (string insightFile in insightFiles)
        //    //{
        //    //    string insightUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, insightOutput, insightFile, false);
        //    //    string insightData = WebClient.GetData(insightUrl);
        //    //    JObject insight = JObject.Parse(insightData);
        //    //    string documentId = databaseClient.UpsertDocument(collectionId, insight);

        //    //    MediaInsightSource insightSource = new MediaInsightSource()
        //    //    {
        //    //        MediaProcessor = insightProcessor.Value,
        //    //        OutputId = documentId
        //    //    };
        //    //    insightSources.Add(insightSource);
        //    //}

        //    mediaInsight.Sources = insightSources.ToArray();
        //    databaseClient.UpsertDocument(collectionId, mediaInsight);

        //    if (string.IsNullOrEmpty(insightId) && encoderOutput != null)
        //    {
        //        encoderOutput.AlternateId = mediaInsight.Id;
        //        encoderOutput.Update();
        //    }
        //}

        //private static void PublishInsight(MediaClient mediaClient, IJob job, IAsset encoderOutput)
        //{
        //    string processorId1 = Constant.Media.ProcessorId.FaceDetection;
        //    string processorId2 = Constant.Media.ProcessorId.FaceRedaction;
        //    string processorId3 = Constant.Media.ProcessorId.MotionDetection;
        //    string[] processorIds = new string[] { processorId1, processorId2, processorId3 };
        //    ITask[] insightTasks = GetJobTasks(job, processorIds);
        //    using (DatabaseClient databaseClient = new DatabaseClient())
        //    {
        //        foreach (ITask insightTask in insightTasks)
        //        {
        //            PublishInsight(mediaClient, databaseClient, insightTask, encoderOutput);
        //        }
        //    }
        //    processorId1 = Constant.Media.ProcessorId.AudioAnalyzer;
        //    processorIds = new string[] { processorId1 };
        //    insightTasks = GetJobTasks(job, processorIds);
        //    foreach (ITask insightTask in insightTasks)
        //    {
        //        PublishWebVtt(mediaClient, insightTask, encoderOutput);
        //    }
        //}

        //private static void PublishJob(MediaClient mediaClient, IJob job)
        //{
        //    ITask[] encoderTasks = GetEncoderTasks(job);
        //    if (encoderTasks.Length == 0)
        //    {
        //        foreach (IAsset inputAsset in job.InputMediaAssets)
        //        {
        //            PublishInsight(mediaClient, job, null);
        //        }
        //    }
        //    else
        //    {
        //        foreach (ITask encoderTask in encoderTasks)
        //        {
        //            ContentProtection contentProtection = GetContentProtection(job.Id, encoderTask.Id);
        //            foreach (IAsset encoderOutput in encoderTask.OutputAssets)
        //            {
        //                PublishAsset(mediaClient, encoderOutput, contentProtection, LocatorType.OnDemandOrigin);
        //                PublishInsight(mediaClient, job, encoderOutput);
        //            }
        //        }
        //    }
        //}

        public static MediaPublished PublishContent(MediaPublish mediaPublish)
        {
            MediaPublished mediaPublished = null;
            //MediaClient mediaClient = new MediaClient(null, mediaPublish.MediaAccount);
            //IJob job = mediaClient.GetEntityById(MediaEntity.Job, mediaPublish.Id) as IJob;
            //if (job != null)
            //{
            //    string indexId = null;
            //    mediaClient.SetProcessorUnits(job, ReservedUnitType.Basic, false);
            //    PublishJob(mediaClient, job);
            //    if (mediaPublish.InsightConfig != null)
            //    {
            //        IAsset encoderOutput = GetEncoderOutput(job);
            //        VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaPublish.MediaAccount);
            //        indexId = videoAnalyzer.StartAnalysis(mediaClient, encoderOutput, mediaPublish.InsightConfig);
            //    }
            //    mediaPublished = new MediaPublished()
            //    {
            //        IndexId = indexId,
            //        MobileNumber = mediaPublish.MobileNumber,
            //        StatusMessage = GetNotificationMessage(mediaPublish.MediaAccount, job)
            //    };
            //}
            return mediaPublished;
        }

        public static MediaPublished PublishInsight(MediaPublish mediaPublish)
        {
            string indexId = mediaPublish.Id;
            //VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaPublish.MediaAccount);
            //JObject index = videoAnalyzer.GetIndex(indexId, null, false);
            //UpsertInsight(index);

            MediaPublished mediaPublished = new MediaPublished
            {
                IndexId = indexId,
                MobileNumber = mediaPublish.MobileNumber,
                StatusMessage = string.Empty
            };
            return mediaPublished;
        }

        public static void PurgePublish()
        {
            DatabaseClient databaseClient = new DatabaseClient();

            string collectionId = Constant.Database.Collection.OutputInsight;
            JObject[] documents = databaseClient.GetDocuments(collectionId);
            foreach (JObject document in documents)
            {
                MediaAccount mediaAccount = GetMediaAccount(document);
                MediaClient mediaClient = new MediaClient(null, mediaAccount);
                string assetId = document["id"].ToString();
                //IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                //if (asset == null)
                //{
                //    databaseClient.DeleteDocument(collectionId, assetId);
                //}
            }

            //collectionId = Constant.Database.Collection.OutputPublish;
            //documents = databaseClient.GetDocuments(collectionId);
            //foreach (JObject document in documents)
            //{
            //    MediaAccount mediaAccount = GetMediaAccount(document);
            //    MediaClient mediaClient = new MediaClient(null, mediaAccount);
            //    string jobId = document["id"].ToString();
            //    IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;
            //    if (job == null)
            //    {
            //        JToken taskIds = document["TaskIds"];
            //        foreach (JToken taskId in taskIds)
            //        {
            //            databaseClient.DeleteDocument(collectionId, taskId.ToString());
            //        }
            //        databaseClient.DeleteDocument(collectionId, jobId);
            //    }
            //}
        }
    }
}