using System.IO;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static void CreateFile(BlobClient blobClient, IAssetFile sourceFile, IAsset targetAsset, string targetFileName)
        {
            string sourceContainer = sourceFile.Asset.Uri.Segments[1];
            CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainer, null, sourceFile.Name);
            using (Stream sourceStream = sourceBlob.OpenRead())
            {
                IAssetFile destinationFile = targetAsset.AssetFiles.Create(targetFileName);
                destinationFile.Upload(sourceStream);
            }
        }

        private static void PublishTextTracks(BlobClient blobClient, ITask jobTask, IAsset encoderOutput)
        {
            IAsset analyticsAsset = jobTask.OutputAssets[0];
            IAssetFile[] webVttFiles = GetAssetFiles(analyticsAsset, Constant.Media.FileExtension.WebVtt);
            foreach (IAssetFile webVttFile in webVttFiles)
            {
                JObject processorConfig = JObject.Parse(jobTask.Configuration);
                string languageId = Language.GetLanguageId(processorConfig);
                string fileName = string.Concat(languageId, Constant.Media.FileExtension.WebVtt);
                CreateFile(blobClient, webVttFile, encoderOutput, fileName);
            }
        }

        private static void PublishAnalytics(BlobClient blobClient, DocumentClient documentClient, MediaContentPublish contentPublish, ITask jobTask, IAsset encoderOutput)
        {
            foreach (IAsset outputAsset in jobTask.OutputAssets)
            {
                string[] fileNames = GetFileNames(outputAsset, Constant.Media.FileExtension.Json);
                foreach (string fileName in fileNames)
                {
                    string documentData = string.Empty;
                    string sourceContainerName = outputAsset.Uri.Segments[1];
                    CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, null, fileName);
                    using (Stream sourceStream = sourceBlob.OpenRead())
                    {
                        StreamReader streamReader = new StreamReader(sourceStream);
                        documentData = streamReader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(documentData))
                    {
                        MediaProcessor? mediaProcessor = Processor.GetMediaProcessor(jobTask.MediaProcessorId);
                        if (mediaProcessor.HasValue)
                        {
                            string collectionId = Constant.Database.Collection.ContentInsight;
                            JObject document = JObject.Parse(documentData);

                            string accountId = contentPublish.PartitionKey;
                            string accountDomain = contentPublish.MediaAccountDomainName;
                            string accountEndpoint = contentPublish.MediaAccountEndpointUrl;
                            string clientId = contentPublish.MediaAccountClientId;
                            string clientKey = contentPublish.MediaAccountClientKey;

                            document = DocumentClient.SetContext(document, accountId, accountDomain, accountEndpoint, clientId, clientKey, outputAsset.Id);
                            string documentId = documentClient.UpsertDocument(collectionId, document);

                            string processorName = Processor.GetProcessorName(mediaProcessor.Value);
                            outputAsset.AlternateId = string.Concat(processorName, Constant.TextDelimiter.Identifier, documentId);
                            outputAsset.Update();

                            if (encoderOutput != null)
                            {
                                IAssetFile assetFile = encoderOutput.AssetFiles.Create(outputAsset.AlternateId);
                                using (Stream sourceStream = sourceBlob.OpenRead())
                                {
                                    StreamReader streamReader = new StreamReader(sourceStream);
                                    assetFile.Upload(sourceStream);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void PublishAnalytics(MediaClient mediaClient, MediaContentPublish contentPublish, IJob job)
        {
            string[] accountCredentials = new string[] { contentPublish.StorageAccountName, contentPublish.StorageAccountKey };
            BlobClient blobClient = new BlobClient(accountCredentials);
            string processorId1 = Constant.Media.ProcessorId.VideoAnnotation;
            string processorId2 = Constant.Media.ProcessorId.CharacterRecognition;
            string processorId3 = Constant.Media.ProcessorId.ContentModeration;
            string processorId4 = Constant.Media.ProcessorId.FaceDetection;
            string processorId5 = Constant.Media.ProcessorId.FaceRedaction;
            string processorId6 = Constant.Media.ProcessorId.MotionDetection;
            string[] processorIds = new string[] { processorId1, processorId2, processorId3, processorId4, processorId5, processorId6 };
            ITask[] analyticTasks = GetJobTasks(job, processorIds);
            processorId1 = Constant.Media.ProcessorId.EncoderStandard;
            processorId2 = Constant.Media.ProcessorId.EncoderPremium;
            processorIds = new string[] { processorId1, processorId2 };
            ITask[] encoderTasks = GetJobTasks(job, processorIds);
            IAsset encoderOutput = encoderTasks.Length == 1 ? encoderTasks[0].OutputAssets[0] : null;
            if (analyticTasks.Length > 0)
            {
                using (DocumentClient documentClient = new DocumentClient())
                {
                    foreach (ITask analyticTask in analyticTasks)
                    {
                        PublishAnalytics(blobClient, documentClient, contentPublish, analyticTask, encoderOutput);
                    }
                }
            }
            processorId1 = Constant.Media.ProcessorId.SpeechAnalyzer;
            processorIds = new string[] { processorId1 };
            analyticTasks = GetJobTasks(job, processorIds);
            if (analyticTasks.Length > 0 && encoderOutput != null)
            {
                foreach (ITask analyticTask in analyticTasks)
                {
                    PublishTextTracks(blobClient, analyticTask, encoderOutput);
                }
            }
        }

        public static MediaPublish PublishInsight(MediaInsightPublish insightPublish)
        {
            string accountId = insightPublish.PartitionKey;
            string accountDomain = insightPublish.MediaAccountDomainName;
            string accountEndpoint = insightPublish.MediaAccountEndpointUrl;
            string clientId = insightPublish.MediaAccountClientId;
            string clientKey = insightPublish.MediaAccountClientKey;

            string indexerKey = insightPublish.IndexerAccountKey;
            string indexId = insightPublish.RowKey;

            IndexerClient indexerClient = new IndexerClient(accountId, indexerKey);
            JObject index = indexerClient.GetIndex(indexId, null, false);

            MediaPublish mediaPublish = null;
            string assetId = IndexerClient.GetAssetId(index);
            if (!string.IsNullOrEmpty(assetId))
            {
                index = DocumentClient.SetContext(index, accountId, accountDomain, accountEndpoint, clientId, clientKey, assetId);

                DocumentClient documentClient = new DocumentClient();
                string collectionId = Constant.Database.Collection.ContentInsight;
                string documentId = documentClient.UpsertDocument(collectionId, index);

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
            return mediaPublish;
        }

        public static MediaPublish PublishInsight(string queueName)
        {
            MediaPublish mediaPublish = null;
            QueueClient queueClient = new QueueClient();
            MediaInsightPublish insightPublish = queueClient.GetMessage<MediaInsightPublish>(queueName, out string messageId, out string popReceipt);
            if (insightPublish != null)
            {
                mediaPublish = PublishInsight(insightPublish);
                queueClient.DeleteMessage(queueName, messageId, popReceipt);
            }
            return mediaPublish;
        }
    }
}