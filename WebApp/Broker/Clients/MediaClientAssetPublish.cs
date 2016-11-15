using System;
using System.Xml;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private static IAsset[] GetTaskOutputs(IJob job, string[] processorIds)
        {
            List<IAsset> taskOutputs = new List<IAsset>();
            foreach (string processorId in processorIds)
            {
                foreach (ITask jobTask in job.Tasks)
                {
                    if (string.Equals(jobTask.MediaProcessorId, processorId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        taskOutputs.AddRange(jobTask.OutputAssets);
                    }
                }
            }
            return taskOutputs.ToArray();
        }

        private static IAsset GetTaskOutput(IJob job, string[] processorIds)
        {
            IAsset[] taskOutputs = GetTaskOutputs(job, processorIds);
            return (taskOutputs.Length == 0) ? null : taskOutputs[0];
        }

        private static IAsset GetEncoderAsset(IJob job)
        {
            string settingKey = Constants.AppSettings.MediaProcessorEncoderStandardId;
            string processorIdStandard = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorEncoderPremiumId;
            string processorIdPremium = AppSetting.GetValue(settingKey);
            string[] processorIds = new string[] { processorIdStandard, processorIdPremium };
            return GetTaskOutput(job, processorIds);
        }

        private static ITask[] GetJobTasks(IJob job, string[] processorIds)
        {
            List<ITask> jobTasks = new List<ITask>();
            foreach (string processorId in processorIds)
            {
                foreach (ITask jobTask in job.Tasks)
                {
                    if (string.Equals(jobTask.MediaProcessorId, processorId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        jobTasks.Add(jobTask);
                    }
                }
            }
            return jobTasks.ToArray();
        }

        private static string GetLanguageCode(ITask indexerTask, string processorIdV1)
        {
            string languageCode = string.Empty;
            if (string.Equals(indexerTask.MediaProcessorId, processorIdV1, StringComparison.InvariantCultureIgnoreCase))
            {
                XmlDocument processorConfigXml = new XmlDocument();
                processorConfigXml.LoadXml(indexerTask.Configuration);
                XmlNodeList configSettings = processorConfigXml.SelectNodes(Constants.Media.ProcessorConfig.IndexerV1XPath);
                string spokenLanguage = configSettings[0].Attributes[1].Value;
                languageCode = string.Equals(spokenLanguage, "Spanish", StringComparison.InvariantCultureIgnoreCase) ? "es" : "en";
            }
            else
            {
                JObject processorConfig = JObject.Parse(indexerTask.Configuration);
                JToken processorOptions = processorConfig["Features"][0]["Options"];
                string spokenLanguage = processorOptions["Language"].ToString();
                languageCode = spokenLanguage.Substring(0, 2).ToLower();
            }
            return languageCode;
        }

        private static void CopyOutputFile(BlobClient blobClient, IAsset sourceAsset, IAsset destinationAsset, string sourceFileName, string destinationFileName)
        {
            string sourceContainerName = sourceAsset.Uri.Segments[1];
            CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, string.Empty, sourceFileName, true);
            string destinationContainerName = destinationAsset.Uri.Segments[1];
            CloudBlockBlob destinationBlob = blobClient.GetBlob(destinationContainerName, string.Empty, destinationFileName, false);
            blobClient.CopyBlob(sourceBlob, destinationBlob, false);
            IAssetFile indexerFile = destinationAsset.AssetFiles.Create(destinationFileName);
            indexerFile.ContentFileSize = sourceBlob.Properties.Length;
            indexerFile.MimeType = sourceBlob.Properties.ContentType;
            indexerFile.Update();
        }

        public static string[] GetFileNames(IAsset asset, string fileExtension)
        {
            List<string> fileNames = new List<string>();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    fileNames.Add(assetFile.Name);
                }
            }
            return fileNames.ToArray();
        }

        public static void PublishAsset(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
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

        private static void PublishIndex(IJob job, IAsset encoderAsset, ContentPublish contentPublish)
        {
            string settingKey = Constants.AppSettings.MediaProcessorIndexerV1Id;
            string processorId1 = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorIndexerV2Id;
            string processorId2 = AppSetting.GetValue(settingKey);
            string[] processorIds = new string[] { processorId1, processorId2 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length > 0)
            {
                BlobClient blobClient = new BlobClient(contentPublish);
                foreach (ITask jobTask in jobTasks)
                {
                    IAsset outputAsset = jobTask.OutputAssets[0];
                    string fileExtension = Constants.Media.AssetMetadata.VttExtension;
                    string[] fileNames = GetFileNames(outputAsset, fileExtension);
                    if (fileNames.Length > 0)
                    {
                        string sourceFileName = fileNames[0];
                        string languageCode = GetLanguageCode(jobTask, processorId1);
                        string destinationFileName = sourceFileName.Replace(fileExtension, string.Concat("-", languageCode, fileExtension));
                        CopyOutputFile(blobClient, outputAsset, encoderAsset, sourceFileName, destinationFileName);
                    }
                }
                encoderAsset.Update();
            }
        }

        private static void PublishAnalytics(IJob job, IAsset encoderAsset, ContentPublish contentPublish)
        {
            string settingKey = Constants.AppSettings.MediaProcessorFaceDetectionId;
            string processorId1 = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorFaceRedactionId;
            string processorId2 = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorMotionDetectionId;
            string processorId3 = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorCharacterRecognitionId;
            string processorId4 = AppSetting.GetValue(settingKey);
            string[] processorIds = new string[] { processorId1, processorId2, processorId3, processorId4 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length > 0)
            {
                BlobClient blobClient = new BlobClient(contentPublish);
                foreach (ITask jobTask in jobTasks)
                {
                    IAsset outputAsset = jobTask.OutputAssets[0];
                    string fileExtension = Constants.Media.AssetMetadata.JsonExtension;
                    string[] fileNames = GetFileNames(outputAsset, fileExtension);
                    if (fileNames.Length > 0)
                    {
                        string sourceFileName = fileNames[0];
                        string processorName = jobTask.Name.Replace(" ", "");
                        string destinationFileName = sourceFileName.Replace(fileExtension, string.Concat("-", processorName, fileExtension));
                        CopyOutputFile(blobClient, outputAsset, encoderAsset, sourceFileName, destinationFileName);
                    }
                }
            }
        }

        public static void PublishJob(MediaClient mediaClient, IJob job, ContentPublish contentPublish, ContentProtection contentProtection)
        {
            IAsset asset = GetEncoderAsset(job);
            if (asset != null)
            {
                PublishAsset(mediaClient, asset, contentProtection);
                if (asset.IsStreamable)
                {
                    PublishIndex(job, asset, contentPublish);
                    PublishAnalytics(job, asset, contentPublish);
                }
            }
        }
    }
}
