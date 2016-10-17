using System;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private static IAsset[] GetTaskOutputs(IJob job, string processorId)
        {
            List<IAsset> taskOutputs = new List<IAsset>();
            foreach (ITask jobTask in job.Tasks)
            {
                if (string.Equals(jobTask.MediaProcessorId, processorId, StringComparison.InvariantCultureIgnoreCase))
                {
                    taskOutputs.AddRange(jobTask.OutputAssets);
                }
            }
            return taskOutputs.ToArray();
        }

        internal static IAsset GetTaskOutput(IJob job, string processorId)
        {
            IAsset[] taskOutputs = GetTaskOutputs(job, processorId);
            return (taskOutputs.Length == 0) ? null : taskOutputs[0];
        }

        internal static void PublishAsset(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
        {
            if (asset.IsStreamable || asset.AssetType == AssetType.MP4)
            {
                if (asset.Options == AssetCreationOptions.StorageEncrypted && asset.DeliveryPolicies.Count == 0)
                {
                    mediaClient.AddDeliveryPolicies(asset, contentProtection);
                }
                if (asset.Locators.Count == 0)
                {
                    LocatorType locatorType = LocatorType.OnDemandOrigin;
                    mediaClient.CreateLocator(null, locatorType, asset, null);
                }
            }
        }

        private static string InsertMetadata(MediaClient mediaClient, BlobClient blobClient, DatabaseClient databaseClient, IAsset metadataAsset)
        {
            string containerName = metadataAsset.Uri.Segments[1];
            string primaryFile = MediaClient.GetPrimaryFile(metadataAsset);
            CloudBlockBlob metadataFile = blobClient.GetBlob(containerName, null, primaryFile);
            string collectionId = Constants.Media.AssetMetadata.DocumentCollection;
            string jsonData = metadataFile.DownloadText();
            return databaseClient.CreateDocument(collectionId, jsonData);
        }

        //internal static void PublishAnalytics(MediaClient mediaClient, IJob job, IAsset parentAsset, ContentPublish contentPublish)
        //{
        //    DatabaseClient databaseClient = new DatabaseClient(true);
        //    BlobClient blobClient = new BlobClient(contentPublish);
        //    string containerName = parentAsset.Uri.Segments[1];
        //    CloudBlobContainer parentContainer = blobClient.GetContainer(containerName);
        //    if (contentPublish.IndexAudio)
        //    {
        //        string settingKey = Constants.AppSettings.MediaProcessorIndexerAudioId;
        //        string processorId = Configuration.GetSetting(settingKey);
        //        IAsset indexerOutput = GetTaskOutput(job, processorId);
        //        string sourceContainerName = indexerOutput.Uri.Segments[1];
        //        string sourceFileName = assetController.GetFileNamesWebVtt(indexerOutput)[0];
        //        CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, null, sourceFileName);
        //        string[] languages = contentPublish.SubtitleLanguages.Split(Constants.MultiItemSeparator);
        //        foreach (string language in languages)
        //        {
        //            string fileName = string.Concat(language, ".vtt");
        //            CloudBlockBlob destinationBlob = blobClient.GetBlob(containerName, null, fileName);
        //            blobClient.CopyBlob(sourceBlob, destinationBlob, false);
        //            parentAsset.AssetFiles.Create(fileName);
        //        }
        //    }
        //    if (contentPublish.FaceDetect)
        //    {
        //        string settingKey = Constants.AppSettings.MediaProcessorFaceDetectorId;
        //        string processorId = Configuration.GetSetting(settingKey);
        //        IAsset metadataAsset = GetTaskOutput(job, processorId);
        //        string documentId = InsertMetadata(mediaClient, blobClient, databaseClient, metadataAsset);
        //        string metadataKey = Constants.Media.AssetMetadata.DocIdFaceDetect;
        //        parentContainer.Metadata[metadataKey] = documentId;
        //    }
        //    parentContainer.SetMetadata();
        //}

        //public void CreateFilter(IAsset asset)
        //{
        //    if (asset == null)
        //    {
        //        IStreamingFilter filter;
        //    }
        //    else
        //    {
        //        IStreamingAssetFilter filter;
        //    }
        //}
    }
}
