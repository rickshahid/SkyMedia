using System;
using System.IO;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static void UpsertIndex(JObject index)
        {
            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constant.Database.Collection.MediaInsight;
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
                DomainName = document["MediaAccount"]["DomainName"].ToString(),
                EndpointUrl = document["MediaAccount"]["EndpointUrl"].ToString(),
                ClientId = document["MediaAccount"]["ClientId"].ToString(),
                ClientKey = document["MediaAccount"]["ClientKey"].ToString(),
                IndexerKey = document["MediaAccount"]["IndexerKey"].ToString()
            };
            return mediaAccount;
        }

        private static void PublishAsset(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection, LocatorType locatorType)
        {
            string locatorId = null;
            ILocator[] locators = asset.Locators.Where(l => l.Type == locatorType).ToArray();
            foreach (ILocator locator in locators)
            {
                if (locatorId == null)
                {
                    locatorId = locator.Id;
                }
                locator.Delete();
            }
            if (contentProtection != null)
            {
                mediaClient.SetDeliveryPolicies(asset, contentProtection);
            }
            mediaClient.CreateLocator(locatorId, locatorType, asset, false);
        }

        private static void PublishWebVtt(MediaClient mediaClient, ITask insightTask, IAsset encoderOutput)
        {
            if (encoderOutput != null)
            {
                IAsset insightOutput = insightTask.OutputAssets[0];
                IAssetFile[] webVttFiles = GetAssetFiles(insightOutput, Constant.Media.FileExtension.WebVtt);
                foreach (IAssetFile webVttFile in webVttFiles)
                {
                    JObject processorConfig = JObject.Parse(insightTask.Configuration);
                    string languageId = Language.GetLanguageId(processorConfig);
                    string fileName = string.Concat(MediaProcessor.SpeechAnalyzer, Constant.TextDelimiter.Identifier, languageId, Constant.Media.FileExtension.WebVtt);

                    string webVttUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, webVttFile.Asset, webVttFile.Name, false);
                    using (Stream webVttStream = WebClient.GetStream(webVttUrl))
                    {
                        IAssetFile encoderOutputFile = encoderOutput.AssetFiles.Create(fileName);
                        encoderOutputFile.Upload(webVttStream);
                    }
                }
            }
        }

        private static void PublishInsight(MediaClient mediaClient, DatabaseClient databaseClient, ITask insightTask, IAsset encoderOutput)
        {
            MediaProcessor? insightProcessor = Processor.GetMediaProcessor(insightTask.MediaProcessorId);
            IAsset insightOutput = insightTask.OutputAssets[0];
            foreach (IAssetFile insightFile in insightOutput.AssetFiles)
            {
                string insightUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, insightOutput, insightFile.Name, false);
                string insightData = WebClient.GetData(insightUrl);
                JObject insight = JObject.Parse(insightData);
                string collectionId = Constant.Database.Collection.MediaInsight;
                string documentId = databaseClient.UpsertDocument(collectionId, insight);
                if (encoderOutput != null)
                {
                    //TODO: Add insightProcessor and documentId into the metadata of the encoderOutput asset
                }
            }
        }

        private static void PublishInsight(MediaClient mediaClient, IJob job, IAsset encoderOutput)
        {
            string processorId1 = Constant.Media.ProcessorId.VideoAnnotation;
            string processorId2 = Constant.Media.ProcessorId.FaceDetection;
            string processorId3 = Constant.Media.ProcessorId.FaceRedaction;
            string processorId4 = Constant.Media.ProcessorId.MotionDetection;
            string processorId5 = Constant.Media.ProcessorId.ContentModeration;
            string processorId6 = Constant.Media.ProcessorId.CharacterRecognition;
            string[] processorIds = new string[] { processorId1, processorId2, processorId3, processorId4, processorId5, processorId6 };
            ITask[] insightTasks = GetJobTasks(job, processorIds);
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                foreach (ITask insightTask in insightTasks)
                {
                    PublishInsight(mediaClient, databaseClient, insightTask, encoderOutput);
                }
            }
            processorId1 = Constant.Media.ProcessorId.SpeechAnalyzer;
            processorIds = new string[] { processorId1 };
            insightTasks = GetJobTasks(job, processorIds);
            foreach (ITask insightTask in insightTasks)
            {
                PublishWebVtt(mediaClient, insightTask, encoderOutput);
            }
        }

        private static void PublishJob(MediaClient mediaClient, IJob job)
        {
            ITask[] encoderTasks = GetEncoderTasks(job);
            if (encoderTasks.Length == 0)
            {
                foreach (IAsset inputAsset in job.InputMediaAssets)
                {
                    PublishInsight(mediaClient, job, null);
                }
            }
            else
            {
                foreach (ITask encoderTask in encoderTasks)
                {
                    ContentProtection contentProtection = GetContentProtection(job.Id, encoderTask.Id);
                    foreach (IAsset encoderOutput in encoderTask.OutputAssets)
                    {
                        PublishAsset(mediaClient, encoderOutput, contentProtection, LocatorType.OnDemandOrigin);
                        PublishInsight(mediaClient, job, encoderOutput);
                    }
                }
            }
        }

        public static MediaPublished PublishContent(MediaPublish mediaPublish)
        {
            MediaPublished mediaPublished = null;
            MediaClient mediaClient = new MediaClient(mediaPublish.MediaAccount);
            IJob job = mediaClient.GetEntityById(MediaEntity.Job, mediaPublish.Id) as IJob;
            if (job != null)
            {
                mediaClient.SetProcessorUnits(job, ReservedUnitType.Basic, false);
                PublishJob(mediaClient, job);
                mediaPublished = new MediaPublished()
                {
                    MobileNumber = mediaPublish.MobileNumber,
                    StatusMessage = GetNotificationMessage(mediaPublish.MediaAccount.Id, job)
                };
            }
            return mediaPublished;
        }

        public static MediaPublished PublishInsight(MediaPublish mediaPublish)
        {
            string indexId = mediaPublish.Id;
            IndexerClient indexerClient = new IndexerClient(mediaPublish.MediaAccount);
            JObject index = indexerClient.GetIndex(indexId, null, false);
            UpsertIndex(index);

            string assetId = IndexerClient.GetAssetId(index);
            MediaPublished mediaPublished = new MediaPublished
            {
                AssetId = assetId,
                IndexId = indexId,
                MobileNumber = mediaPublish.MobileNumber,
                StatusMessage = string.Empty
            };
            return mediaPublished;
        }

        public static void PurgePublish()
        {
            DatabaseClient databaseClient = new DatabaseClient();

            string collectionId = Constant.Database.Collection.MediaInsight;
            JObject[] documents = databaseClient.GetDocuments(collectionId);
            foreach (JObject document in documents)
            {
                MediaAccount mediaAccount = GetMediaAccount(document);
                MediaClient mediaClient = new MediaClient(mediaAccount);
                string assetId = document["id"].ToString();
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                if (asset == null)
                {
                    databaseClient.DeleteDocument(collectionId, assetId);
                }
            }

            collectionId = Constant.Database.Collection.MediaPublish;
            documents = databaseClient.GetDocuments(collectionId);
            foreach (JObject document in documents)
            {
                MediaAccount mediaAccount = GetMediaAccount(document);
                MediaClient mediaClient = new MediaClient(mediaAccount);
                string jobId = document["id"].ToString();
                IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;
                if (job == null)
                {
                    JToken taskIds = document["TaskIds"];
                    foreach (JToken taskId in taskIds)
                    {
                        databaseClient.DeleteDocument(collectionId, taskId.ToString());
                    }
                    databaseClient.DeleteDocument(collectionId, jobId);
                }
            }
        }
    }
}