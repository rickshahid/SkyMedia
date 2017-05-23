using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
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

        private static ITask[] GetJobTasks(IJob job, string[] processorIds)
        {
            List<ITask> jobTasks = new List<ITask>();
            foreach (ITask jobTask in job.Tasks)
            {
                if (processorIds.Contains(jobTask.MediaProcessorId, StringComparer.OrdinalIgnoreCase))
                {
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static JobPublish GetJobPublish(EntityClient entityClient, MediaJobNotification jobNotification)
        {
            string tableName = Constant.Storage.TableName.JobPublish;
            string partitionKey = jobNotification.Properties.AccountName;
            string rowKey = jobNotification.Properties.JobId;
            return entityClient.GetEntity<JobPublish>(tableName, partitionKey, rowKey);
        }

        private static ContentProtection GetContentProtection(EntityClient entityClient, JobPublish jobPublish)
        {
            string tableName = Constant.Storage.TableName.ContentProtection;
            string partitionKey = jobPublish.PartitionKey;
            string rowKey = jobPublish.RowKey;
            return entityClient.GetEntity<ContentProtection>(tableName, partitionKey, rowKey);
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

        private static void PublishMetadata(ITask jobTask, BlobClient blobClient, CosmosClient cosmosClient, IDictionary<string, string> dataAttributes)
        {
            foreach (IAsset outputAsset in jobTask.OutputAssets)
            {
                if (string.IsNullOrEmpty(outputAsset.AlternateId))
                {
                    string[] fileNames = GetFileNames(outputAsset, Constant.Media.FileExtension.Json);
                    foreach (string fileName in fileNames)
                    {
                        string fileData = string.Empty;
                        string sourceContainerName = outputAsset.Uri.Segments[1];
                        CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, string.Empty, fileName, false);
                        using (Stream sourceStream = sourceBlob.OpenRead())
                        {
                            StreamReader streamReader = new StreamReader(sourceStream);
                            fileData = streamReader.ReadToEnd();
                        }
                        if (!string.IsNullOrEmpty(fileData))
                        {
                            MediaProcessor processorType = Processor.GetProcessorType(jobTask.MediaProcessorId);
                            string processorName = Processor.GetProcessorName(processorType);
                            string collectionId = Constant.Database.Collection.Metadata;
                            string documentId = cosmosClient.CreateDocument(collectionId, fileData, dataAttributes);
                            outputAsset.AlternateId = string.Concat(processorName, Constant.TextDelimiter.Identifier, documentId);
                            outputAsset.Update();
                        }
                    }
                    JObject processorConfig = JObject.Parse(jobTask.Configuration);
                    if (processorConfig["options"] != null &&
                        processorConfig["options"]["mode"] != null &&
                        processorConfig["options"]["mode"].ToString() == "analyze")
                    {
                        foreach (IAsset inputAsset in jobTask.InputAssets)
                        {
                            string primaryFileName = GetPrimaryFile(inputAsset);
                            blobClient.CopyFile(inputAsset, outputAsset, primaryFileName, primaryFileName, true);
                        }
                    }
                }
            }
        }

        private static void PublishMetadata(IJob job, JobPublish jobPublish, string assetId)
        {
            string processorId1 = Constant.Media.ProcessorId.FaceDetection;
            string processorId2 = Constant.Media.ProcessorId.FaceRedaction;
            string processorId3 = Constant.Media.ProcessorId.VideoAnnotation;
            string processorId4 = Constant.Media.ProcessorId.CharacterRecognition;
            string processorId5 = Constant.Media.ProcessorId.ContentModeration;
            string processorId6 = Constant.Media.ProcessorId.MotionDetection;
            string[] processorIds = new string[] { processorId1, processorId2, processorId3, processorId4, processorId5, processorId6 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length > 0)
            {
                string[] accountCredentials = new string[] { jobPublish.StorageAccountName, jobPublish.StorageAccountKey };
                BlobClient blobClient = new BlobClient(accountCredentials);
                using (CosmosClient cosmosClient = new CosmosClient(true))
                {
                    foreach (ITask jobTask in jobTasks)
                    {
                        Dictionary<string, string> dataAttributes = new Dictionary<string, string>();
                        dataAttributes.Add("AccountName", jobPublish.PartitionKey);
                        dataAttributes.Add("AccountKey", jobPublish.MediaAccountKey);
                        dataAttributes.Add("AssetId", assetId);
                        PublishMetadata(jobTask, blobClient, cosmosClient, dataAttributes);
                    }
                }
            }
        }

        private static void PublishContent(MediaClient mediaClient, IJob job, JobPublish jobPublish, ContentProtection contentProtection)
        {
            string processorId1 = Constant.Media.ProcessorId.EncoderStandard;
            string processorId2 = Constant.Media.ProcessorId.EncoderPremium;
            string processorId3 = Constant.Media.ProcessorId.EncoderUltra;
            string[] processorIds = new string[] { processorId1, processorId2, processorId3 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length == 0)
            {
                foreach (IAsset inputAsset in job.InputMediaAssets)
                {
                    PublishSpeech(mediaClient, job);
                    PublishMetadata(job, jobPublish, inputAsset.Id);
                }
            }
            else
            {
                foreach (ITask jobTask in jobTasks)
                {
                    foreach (IAsset outputAsset in jobTask.OutputAssets)
                    {
                        PublishContent(mediaClient, outputAsset, contentProtection);
                        PublishSpeech(mediaClient, job);
                        PublishMetadata(job, jobPublish, outputAsset.Id);
                    }
                }
            }
        }

        internal static void PublishContent(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
        {
            if (asset.IsStreamable || asset.AssetType == AssetType.MP4)
            {
                LocatorType locatorType = LocatorType.OnDemandOrigin;
                for (int i = asset.Locators.Count - 1; i >= 0; i--)
                {
                    if (asset.Locators[i].Type == locatorType)
                    {
                        asset.Locators[i].Delete();
                    }
                }
                for (int i = asset.DeliveryPolicies.Count - 1; i >= 0; i--)
                {
                    asset.DeliveryPolicies.RemoveAt(i);
                }
                if (contentProtection != null)
                {
                    mediaClient.AddDeliveryPolicies(asset, contentProtection);
                }
                mediaClient.CreateLocator(locatorType, asset);
            }
        }

        public static JobPublication PublishJob(MediaJobNotification jobNotification, bool webHook)
        {
            JobPublication jobPublication = new JobPublication();
            if (jobNotification != null && jobNotification.EventType == MediaJobNotificationEvent.JobStateChange &&
                (jobNotification.Properties.NewState == JobState.Error ||
                 jobNotification.Properties.NewState == JobState.Canceled ||
                 jobNotification.Properties.NewState == JobState.Finished))
            {
                if (webHook)
                {
                    string settingKey = Constant.AppSettingKey.MediaJobNotificationStorageQueueName;
                    string queueName = AppSetting.GetValue(settingKey);
                    MessageClient messageClient = new MessageClient();
                    messageClient.AddMessage(queueName, jobNotification);
                }
                else
                {
                    EntityClient entityClient = new EntityClient();
                    JobPublish jobPublish = GetJobPublish(entityClient, jobNotification);
                    if (jobPublish != null)
                    {
                        jobPublication.UserId = jobPublish.UserId;
                        jobPublication.MobileNumber = jobPublish.MobileNumber;
                        ContentProtection contentProtection = GetContentProtection(entityClient, jobPublish);
                        string accountName = jobPublish.PartitionKey;
                        string accountKey = jobPublish.MediaAccountKey;
                        MediaClient mediaClient = new MediaClient(accountName, accountKey);
                        IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobPublish.RowKey) as IJob;
                        if (job != null)
                        {
                            mediaClient.SetProcessorUnits(job, null, ReservedUnitType.Basic);
                            if (jobNotification.Properties.NewState == JobState.Finished)
                            {
                                PublishContent(mediaClient, job, jobPublish, contentProtection);
                                jobPublication.StatusMessage = GetNotificationMessage(accountName, job);
                            }
                        }
                        string tableName = Constant.Storage.TableName.JobPublish;
                        entityClient.DeleteEntity(tableName, jobPublish);
                        if (contentProtection != null)
                        {
                            tableName = Constant.Storage.TableName.ContentProtection;
                            entityClient.DeleteEntity(tableName, contentProtection);
                        }
                    }
                }
            }
            return jobPublication;
        }

        public static JobPublication PublishJob(bool poisonQueue)
        {
            JobPublication jobPublication = null;
            string settingKey = Constant.AppSettingKey.MediaJobNotificationStorageQueueName;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue)
            {
                queueName = string.Concat(queueName, Constant.Storage.Queue.PoisonSuffix);
            }
            CloudQueueMessage queueMessage;
            MessageClient messageClient = new MessageClient();
            string jobMessage = messageClient.GetMessage(queueName, out queueMessage);
            MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(jobMessage);
            if (jobNotification != null)
            {
                jobPublication = MediaClient.PublishJob(jobNotification, false);
                messageClient.DeleteMessage(queueName, queueMessage);
            }
            return jobPublication;
        }

        public static void PurgePublish()
        {
            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.ContentProtection;
            entityClient.PurgeEntities<ContentProtection>(tableName);
            tableName = Constant.Storage.TableName.JobPublish;
            entityClient.PurgeEntities<JobPublish>(tableName);

            CosmosClient cosmosClient = new CosmosClient(true);
            string collectionId = Constant.Database.Collection.Metadata;
            JObject[] documents = cosmosClient.GetDocuments(collectionId);
            foreach (JObject document in documents)
            {
                string accountName = document["AccountName"].ToString();
                string accountKey = document["AccountKey"].ToString();
                string assetId = document["AssetId"].ToString();
                MediaClient mediaClient = new MediaClient(accountName, accountKey);
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                if (asset == null)
                {
                    string documentId = document["id"].ToString();
                    cosmosClient.DeleteDocument(collectionId, documentId);
                }
            }
        }
    }
}
