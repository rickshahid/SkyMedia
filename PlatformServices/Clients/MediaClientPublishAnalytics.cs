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
            //string processorId = Constant.Media.ProcessorId.SpeechToText;
            //string[] processorIds = new string[] { processorId };
            //ITask[] jobTasks = GetJobTasks(job, processorIds);
            //foreach (ITask jobTask in jobTasks)
            //{
            //    JObject processorConfig = JObject.Parse(jobTask.Configuration);
            //    string languageCode = Language.GetLanguageCode(processorConfig);
            //    foreach (IAsset outputAsset in jobTask.OutputAssets)
            //    {
            //        outputAsset.AlternateId = languageCode;
            //        outputAsset.Update();
            //        mediaClient.CreateLocator(LocatorType.OnDemandOrigin, outputAsset);
            //    }
            //}
        }

        private static void PublishAnalytics(ITask jobTask, BlobClient blobClient, CosmosClient cosmosClient,
                                             string accountName, string accountKey, string assetId)
        {
            foreach (IAsset outputAsset in jobTask.OutputAssets)
            {
                //if (string.IsNullOrEmpty(outputAsset.AlternateId))
                //{
                //    string[] fileNames = GetFileNames(outputAsset, Constant.Media.FileExtension.Json);
                //    foreach (string fileName in fileNames)
                //    {
                //        string jsonData = string.Empty;
                //        string sourceContainerName = outputAsset.Uri.Segments[1];
                //        CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, string.Empty, fileName, false);
                //        using (Stream sourceStream = sourceBlob.OpenRead())
                //        {
                //            StreamReader streamReader = new StreamReader(sourceStream);
                //            jsonData = streamReader.ReadToEnd();
                //        }
                //        if (!string.IsNullOrEmpty(jsonData))
                //        {
                //            MediaProcessor? mediaProcessor = Processor.GetMediaProcessor(jobTask.MediaProcessorId);
                //            if (mediaProcessor.HasValue)
                //            {
                //                string collectionId = Constant.Database.Collection.ContentInsight;
                //                JObject jsonDoc = JObject.Parse(jsonData);
                //                string documentId = cosmosClient.UpsertDocument(collectionId, jsonDoc, accountName, accountKey, assetId);

                //                string processorName = Processor.GetProcessorName(mediaProcessor.Value);
                //                outputAsset.AlternateId = string.Concat(processorName, Constant.TextDelimiter.Identifier, documentId);
                //                outputAsset.Update();
                //            }
                //        }
                //    }
                //}
            }
        }

        private static void PublishAnalytics(MediaClient mediaClient, IJob job, MediaContentPublish contentPublish, string assetId)
        {
            string processorId1 = Constant.Media.ProcessorId.FaceDetection;
            string processorId2 = Constant.Media.ProcessorId.FaceRedaction;
            string processorId3 = Constant.Media.ProcessorId.VideoAnnotation;
            string processorId4 = Constant.Media.ProcessorId.MotionDetection;
            string processorId5 = Constant.Media.ProcessorId.CharacterRecognition;
            string processorId6 = Constant.Media.ProcessorId.ContentModeration;
            string[] processorIds = new string[] { processorId1, processorId2, processorId3, processorId4, processorId5, processorId6 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length > 0)
            {
                string[] accountCredentials = new string[] { contentPublish.StorageAccountName, contentPublish.StorageAccountKey };
                BlobClient blobClient = new BlobClient(accountCredentials);
                using (CosmosClient cosmosClient = new CosmosClient(true))
                {
                    foreach (ITask jobTask in jobTasks)
                    {
                        string accountName = contentPublish.PartitionKey;
                        string accountKey = contentPublish.MediaAccountKey;
                        PublishAnalytics(jobTask, blobClient, cosmosClient, accountName, accountKey, assetId);
                    }
                }
            }
            PublishSpeech(mediaClient, job);
        }
    }
}