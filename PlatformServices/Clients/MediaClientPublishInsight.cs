using System.IO;

using Microsoft.Azure.Documents;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static string UpsertDocument(DatabaseClient databaseClient, JObject document, MediaProcessor processor, IAsset asset, string fileName)
        {
            string documentId = string.Empty;
            try
            {
                string collectionId = Constant.Database.Collection.MediaInsight;
                documentId = databaseClient.UpsertDocument(collectionId, document);
                asset.AlternateId = string.Concat(processor.ToString(), Constant.TextDelimiter.Identifier, documentId);
                asset.Update();
            }
            catch (DocumentClientException ex)
            {
                // TODO: If document > 2 MB (Microsoft.Azure.Documents.RequestEntityTooLargeException),
                //       then split it into multiple documents based on the standard metadata sections
            }
            return documentId;
        }

        //private static void PublishTextTracks(BlobClient blobClient, ITask jobTask, IAsset encoderOutput)
        //{
        //    if (encoderOutput != null)
        //    {
        //        IAsset speechOutput = jobTask.OutputAssets[0];
        //        IAssetFile[] webVttFiles = GetAssetFiles(speechOutput, Constant.Media.FileExtension.WebVtt);
        //        foreach (IAssetFile webVttFile in webVttFiles)
        //        {
        //            JObject processorConfig = JObject.Parse(jobTask.Configuration);
        //            string languageId = Language.GetLanguageId(processorConfig);
        //            string fileName = string.Concat(languageId, Constant.Media.FileExtension.WebVtt);
        //            string sourceContainer = webVttFile.Asset.Uri.Segments[1];
        //            CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainer, null, webVttFile.Name);
        //            using (Stream sourceStream = sourceBlob.OpenRead())
        //            {
        //                IAssetFile destinationFile = encoderOutput.AssetFiles.Create(fileName);
        //                destinationFile.Upload(sourceStream);
        //            }
        //        }
        //    }
        //}

        //private static void PublishAnalytics(MediaClient mediaClient, DocumentClient documentClient, MediaPublish contentPublish, ITask jobTask, IAsset encoderOutput)
        //{
        //    foreach (IAsset outputAsset in jobTask.OutputAssets)
        //    {
        //        string[] fileNames = GetFileNames(outputAsset, Constant.Media.FileExtension.Json);
        //        foreach (string fileName in fileNames)
        //        {
        //            string jsonUrl = mediaClient.GetLocatorUrl(outputAsset);
        //            string documentData = WebClient.GetData(jsonUrl);
        //            JObject document = JObject.Parse(documentData);

        //            document = DocumentClient.SetContext(document, contentPublish.MediaAccount, outputAsset.Id);
        //            MediaProcessor? mediaProcessor = Processor.GetMediaProcessor(jobTask.MediaProcessorId);
        //            string documentId = UpsertDocument(documentClient, document, mediaProcessor.Value, outputAsset, fileName);

        //            if (encoderOutput != null)
        //            {
        //                string assetFileName = mediaProcessor.Value.ToString();
        //                if (!string.IsNullOrEmpty(documentId))
        //                {
        //                    assetFileName = string.Concat(assetFileName, Constant.TextDelimiter.Identifier, documentId);
        //                }
        //                assetFileName = string.Concat(assetFileName, Constant.Media.FileExtension.Json);

        //                IAssetFile assetFile = encoderOutput.AssetFiles.Create(assetFileName);
        //                using (Stream sourceStream = sourceBlob.OpenRead())
        //                {
        //                    StreamReader streamReader = new StreamReader(sourceStream);
        //                    assetFile.Upload(sourceStream);
        //                }
        //            }
        //        }
        //    }
        //}

        //private static void PublishAnalytics(MediaClient mediaClient, MediaPublish contentPublish, IJob job, ITask[] encoderTasks)
        //{
        //    if (!string.IsNullOrEmpty(contentPublish.StorageAccountKey))
        //    {
        //        IAsset encoderOutput = encoderTasks.Length == 0 ? null : encoderTasks[0].OutputAssets[0];
        //        string[] accountCredentials = new string[] { contentPublish.StorageAccountName, contentPublish.StorageAccountKey };
        //        BlobClient blobClient = new BlobClient(accountCredentials);
        //        string processorId1 = Constant.Media.ProcessorId.VideoAnnotation;
        //        string processorId2 = Constant.Media.ProcessorId.FaceDetection;
        //        string processorId3 = Constant.Media.ProcessorId.FaceRedaction;
        //        string processorId4 = Constant.Media.ProcessorId.MotionDetection;
        //        string processorId5 = Constant.Media.ProcessorId.ContentModeration;
        //        string processorId6 = Constant.Media.ProcessorId.CharacterRecognition;
        //        string[] processorIds = new string[] { processorId1, processorId2, processorId3, processorId4, processorId5, processorId6 };
        //        ITask[] analyticTasks = GetJobTasks(job, processorIds);
        //        using (DocumentClient documentClient = new DocumentClient())
        //        {
        //            foreach (ITask analyticTask in analyticTasks)
        //            {
        //                PublishAnalytics(mediaClient, documentClient, contentPublish, analyticTask, encoderOutput);
        //            }
        //        }
        //        processorId1 = Constant.Media.ProcessorId.SpeechAnalyzer;
        //        processorIds = new string[] { processorId1 };
        //        analyticTasks = GetJobTasks(job, processorIds);
        //        foreach (ITask analyticTask in analyticTasks)
        //        {
        //            PublishTextTracks(blobClient, analyticTask, encoderOutput);
        //        }
        //    }
        //}

        public static MediaPublished PublishInsight(MediaPublish mediaPublish)
        {
            string indexId = mediaPublish.Id;
            string accountId = mediaPublish.MediaAccount.Id;

            IndexerClient indexerClient = new IndexerClient(mediaPublish.MediaAccount);
            JObject index = indexerClient.GetIndex(indexId, null, false);

            MediaPublished mediaPublished = null;
            string assetId = IndexerClient.GetAssetId(index);
            if (!string.IsNullOrEmpty(assetId))
            {
                MediaClient mediaClient = new MediaClient(mediaPublish.MediaAccount);
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;

                DatabaseClient databaseClient = new DatabaseClient();
                index = DatabaseClient.SetContext(index, mediaPublish.MediaAccount, assetId);
                string documentId = UpsertDocument(databaseClient, index, MediaProcessor.VideoIndexer, asset, null);

                mediaPublished = new MediaPublished
                {
                    AssetId = assetId,
                    IndexId = indexId,
                    MobileNumber = mediaPublish.MobileNumber,
                    StatusMessage = string.Empty
                };
            }
            return mediaPublished;
        }

        public static MediaPublished PublishInsight(string queueName)
        {
            MediaPublished mediaPublished = null;
            QueueClient queueClient = new QueueClient();
            MediaPublish insightPublish = queueClient.GetMessage<MediaPublish>(queueName, out string messageId, out string popReceipt);
            if (insightPublish != null)
            {
                mediaPublished = PublishInsight(insightPublish);
                queueClient.DeleteMessage(queueName, messageId, popReceipt);
            }
            return mediaPublished;
        }
    }
}