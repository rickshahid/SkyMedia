using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static JToken GetExternalId(JObject insight)
        {
            JToken externalId = null;
            if (insight != null && insight["breakdowns"] != null)
            {
                externalId = insight["breakdowns"][0]["externalId"];
            }
            return externalId;
        }

        private static string[] GetFileNames(IAsset asset, string fileExtension)
        {
            List<string> fileNames = new List<string>();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    fileNames.Add(assetFile.Name);
                }
            }
            return fileNames.ToArray();
        }

        private static void PublishSpeech(MediaClient mediaClient, IJob job)
        {
            string processorId = Constant.Media.ProcessorId.SpeechToText;
            string[] processorIds = new string[] { processorId };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            foreach (ITask jobTask in jobTasks)
            {
                JObject processorConfig = JObject.Parse(jobTask.Configuration);
                string languageCode = Language.GetLanguageCode(processorConfig);
                foreach (IAsset outputAsset in jobTask.OutputAssets)
                {
                    outputAsset.AlternateId = languageCode;
                    outputAsset.Update();
                    mediaClient.CreateLocator(LocatorType.OnDemandOrigin, outputAsset);
                }
            }
        }

        private static void PublishAnalytics(ITask jobTask, BlobClient blobClient, DocumentClient documentClient,
                                             MediaContentPublish contentPublish, string assetId)
        {
            foreach (IAsset outputAsset in jobTask.OutputAssets)
            {
                if (string.IsNullOrEmpty(outputAsset.AlternateId))
                {
                    string[] fileNames = GetFileNames(outputAsset, Constant.Media.FileExtension.Json);
                    foreach (string fileName in fileNames)
                    {
                        string jsonData = string.Empty;
                        string sourceContainerName = outputAsset.Uri.Segments[1];
                        CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, string.Empty, fileName, false);
                        using (Stream sourceStream = sourceBlob.OpenRead())
                        {
                            StreamReader streamReader = new StreamReader(sourceStream);
                            jsonData = streamReader.ReadToEnd();
                        }
                        if (!string.IsNullOrEmpty(jsonData))
                        {
                            MediaProcessor? mediaProcessor = Processor.GetMediaProcessor(jobTask.MediaProcessorId);
                            if (mediaProcessor.HasValue)
                            {
                                string collectionId = Constant.Database.Collection.ContentInsight;
                                JObject jsonDoc = JObject.Parse(jsonData);

                                string accountId = contentPublish.PartitionKey;
                                string accountDomain = contentPublish.MediaAccountDomainName;
                                string accountUrl = contentPublish.MediaAccountEndpointUrl;
                                string clientId = contentPublish.MediaAccountClientId;
                                string clientKey = contentPublish.MediaAccountClientKey;

                                string documentId = documentClient.UpsertDocument(collectionId, jsonDoc, accountId, accountDomain, accountUrl, clientId, clientKey, assetId);

                                string processorName = Processor.GetProcessorName(mediaProcessor.Value);
                                outputAsset.AlternateId = string.Concat(processorName, Constant.TextDelimiter.Identifier, documentId);
                                outputAsset.Update();
                            }
                        }
                    }
                }
            }
        }

        private static void PublishAnalytics(MediaClient mediaClient, IJob job, MediaContentPublish contentPublish, string assetId)
        {
            string processorId1 = Constant.Media.ProcessorId.VideoAnnotation;
            string processorId2 = Constant.Media.ProcessorId.FaceDetection;
            string processorId3 = Constant.Media.ProcessorId.FaceRedaction;
            string processorId4 = Constant.Media.ProcessorId.MotionDetection;
            string processorId5 = Constant.Media.ProcessorId.CharacterRecognition;
            string processorId6 = Constant.Media.ProcessorId.ContentModeration;
            string[] processorIds = new string[] { processorId1, processorId2, processorId3, processorId4, processorId5, processorId6 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length > 0)
            {
                string[] accountCredentials = new string[] { contentPublish.StorageAccountName, contentPublish.StorageAccountKey };
                BlobClient blobClient = new BlobClient(accountCredentials);
                using (DocumentClient documentClient = new DocumentClient(true))
                {
                    foreach (ITask jobTask in jobTasks)
                    {
                        PublishAnalytics(jobTask, blobClient, documentClient, contentPublish, assetId);
                    }
                }
            }
            PublishSpeech(mediaClient, job);
        }

        public static MediaPublish PublishInsight(string queueName)
        {
            MediaPublish mediaPublish = null;
            QueueClient queueClient = new QueueClient();
            MediaInsightPublish insightPublish = queueClient.GetMessage<MediaInsightPublish>(queueName, out string messageId, out string popReceipt);
            if (insightPublish != null)
            {
                string accountId = insightPublish.PartitionKey;
                string indexerKey = insightPublish.IndexerAccountKey;
                string indexId = insightPublish.RowKey;

                IndexerClient indexerClient = new IndexerClient(null, accountId, indexerKey);
                JObject index = indexerClient.GetIndex(indexId, null, false);

                JToken externalId = GetExternalId(index);
                if (externalId != null)
                {
                    string accountDomain = insightPublish.MediaAccountDomainName;
                    string accountUrl = insightPublish.MediaAccountEndpointUrl;
                    string clientId = insightPublish.MediaAccountClientId;
                    string clientKey = insightPublish.MediaAccountClientKey;
                    string assetId = externalId.ToString();

                    DocumentClient documentClient = new DocumentClient(true);
                    string collectionId = Constant.Database.Collection.ContentInsight;
                    string documentId = documentClient.UpsertDocument(collectionId, index, accountId, accountDomain, accountUrl, clientId, clientKey, assetId);

                    mediaPublish = new MediaPublish
                    {
                        AssetId = assetId,
                        IndexId = indexId,
                        DocumentId = documentId,
                        UserId = insightPublish.UserId,
                        MobileNumber = insightPublish.MobileNumber,
                        StatusMessage = string.Empty
                    };
                }
                queueClient.DeleteMessage(queueName, messageId, popReceipt);
            }
            return mediaPublish;
        }
    }
}