using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.Services
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

        private static string GetLanguageCode(ITask indexerTask, string indexerProcessorV1Id)
        {
            string languageCode = string.Empty;
            if (string.Equals(indexerTask.MediaProcessorId, indexerProcessorV1Id, StringComparison.InvariantCultureIgnoreCase))
            {
                XmlDocument processorConfig = new XmlDocument();
                processorConfig.LoadXml(indexerTask.Configuration);
                XmlNodeList configSettings = processorConfig.SelectNodes(Constants.Media.ProcessorConfig.IndexerV1XPath);
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

        private static void PublishContent(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
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

        private static void PublishContent(MediaClient mediaClient, IJob job, JobPublish jobPublish, ContentProtection contentProtection)
        {
            string settingKey = Constants.AppSettings.MediaProcessorEncoderStandardId;
            string processorId1 = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorEncoderPremiumId;
            string processorId2 = AppSetting.GetValue(settingKey);
            string[] processorIds = new string[] { processorId1, processorId2 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            foreach (ITask jobTask in jobTasks)
            {
                foreach (IAsset outputAsset in jobTask.OutputAssets)
                {
                    PublishContent(mediaClient, outputAsset, contentProtection);
                    PublishIndex(job, outputAsset, jobPublish);
                    PublishAnalytics(job, outputAsset, jobPublish);
                }
            }
        }

        private static void PublishIndex(IJob job, IAsset asset, JobPublish jobPublish)
        {
            string settingKey = Constants.AppSettings.MediaProcessorIndexerV1Id;
            string processorId1 = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorIndexerV2Id;
            string processorId2 = AppSetting.GetValue(settingKey);
            string[] processorIds = new string[] { processorId1, processorId2 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length > 0)
            {
                string[] accountCredentials = new string[] { jobPublish.StorageAccountName, jobPublish.StorageAccountKey };
                BlobClient blobClient = new BlobClient(accountCredentials);
                foreach (ITask jobTask in jobTasks)
                {
                    IAsset outputAsset = jobTask.OutputAssets[0];
                    string fileExtension = Constants.Media.AssetMetadata.VttExtension;
                    string[] fileNames = GetFileNames(outputAsset, fileExtension);
                    if (fileNames.Length > 0)
                    {
                        string sourceFileName = fileNames[0];
                        string languageCode = GetLanguageCode(jobTask, processorId1);
                        string destinationFileName = sourceFileName.Replace(fileExtension, string.Concat(Constants.NamedItemSeparator, languageCode, fileExtension));
                        blobClient.CopyFile(outputAsset, asset, sourceFileName, destinationFileName, false);
                    }
                }
            }
        }

        private static void PublishAnalytics(IJob job, IAsset asset, JobPublish jobPublish)
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
                string[] accountCredentials = new string[] { jobPublish.StorageAccountName, jobPublish.StorageAccountKey };
                BlobClient blobClient = new BlobClient(accountCredentials);
                DatabaseClient databaseClient = new DatabaseClient(true);
                string collectionId = Constants.Database.CollectionName.Metadata;
                foreach (ITask jobTask in jobTasks)
                {
                    IAsset outputAsset = jobTask.OutputAssets[0];
                    string fileExtension = Constants.Media.AssetMetadata.JsonExtension;
                    string[] fileNames = GetFileNames(outputAsset, fileExtension);
                    if (fileNames.Length > 0 && asset != null)
                    {
                        string sourceContainerName = outputAsset.Uri.Segments[1];
                        string sourceFileName = fileNames[0];
                        CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, string.Empty, sourceFileName, false);
                        string jsonData = string.Empty;
                        using (Stream sourceStream = sourceBlob.OpenRead())
                        {
                            StreamReader streamReader = new StreamReader(sourceStream);
                            jsonData = streamReader.ReadToEnd();
                        }
                        if (!string.IsNullOrEmpty(jsonData))
                        {
                            string documentId = databaseClient.CreateDocument(collectionId, jsonData);
                            string processorName = jobTask.Name.Replace(' ', Constants.NamedItemSeparator);
                            string destinationFileName = string.Concat(documentId, Constants.NamedItemsSeparator, processorName, fileExtension);
                            blobClient.CopyFile(outputAsset, asset, sourceFileName, destinationFileName, false);
                        }
                    }
                    if (jobTask.Configuration.Contains("mode") && jobTask.Configuration.Contains("analyze"))
                    {
                        IAsset inputAsset = jobTask.InputAssets[0];
                        string sourceFileName = GetPrimaryFile(inputAsset);
                        blobClient.CopyFile(inputAsset, outputAsset, sourceFileName, sourceFileName, true);
                    }
                }
            }
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

        public static void PublishContent(MediaClient mediaClient, IAsset asset)
        {
            PublishContent(mediaClient, asset, null);
        }

        public static void PublishJob(MediaClient mediaClient, IJob job, JobPublish jobPublish, ContentProtection contentProtection)
        {
            PublishContent(mediaClient, job, jobPublish, contentProtection);
            PublishAnalytics(job, null, jobPublish);
        }
    }
}
