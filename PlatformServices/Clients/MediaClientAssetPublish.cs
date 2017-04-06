using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static IAsset GetParentAsset(IJob job)
        {
            IAsset parentAsset = null;
            foreach (IAsset inputAsset in job.InputMediaAssets)
            {
                bool invalidAsset = false;
                foreach (IAssetFile inputFile in inputAsset.AssetFiles)
                {
                    invalidAsset = MediaClient.IsPremiumWorkflow(inputFile.Name);
                }
                if (!invalidAsset)
                {
                    parentAsset = inputAsset;
                }
            }
            return parentAsset;
        }

        private static IAsset[] GetChildEncodes(MediaClient mediaClient, IAsset parentAsset)
        {
            List<IAsset> childEncodes = new List<IAsset>();
            MediaAsset[] childAssets = mediaClient.GetAssets(parentAsset.Id);
            foreach (MediaAsset childAsset in childAssets)
            {
                if (childAsset.IsStreamable)
                {
                    childEncodes.Add(childAsset.Asset);
                }
            }
            return childEncodes.ToArray();
        }

        private static IAsset[] GetTaskOutputs(IJob job, string[] processorIds)
        {
            List<IAsset> taskOutputs = new List<IAsset>();
            foreach (ITask jobTask in job.Tasks)
            {
                if (processorIds.Contains(jobTask.MediaProcessorId))
                {
                    taskOutputs.AddRange(jobTask.OutputAssets);
                }
            }
            return taskOutputs.ToArray();
        }

        private static ITask[] GetJobTasks(IJob job, string[] processorIds)
        {
            List<ITask> jobTasks = new List<ITask>();
            foreach (ITask jobTask in job.Tasks)
            {
                if (processorIds.Contains(jobTask.MediaProcessorId))
                {
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static BlobClient GetBlobClient(JobPublish jobPublish)
        {
            string[] accountCredentials = new string[] { jobPublish.StorageAccountName, jobPublish.StorageAccountKey };
            return new BlobClient(accountCredentials);
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
            string tableName = Constant.Storage.TableName.JobPublishProtection;
            return entityClient.GetEntity<ContentProtection>(tableName, jobPublish.PartitionKey, jobPublish.RowKey);
        }

        private static void ClearMetadata(DatabaseClient databaseClient, string collectionId, string fileSuffix,
                                          IAsset encoderOutputAsset, MediaProcessor mediaProcessor)
        {
            IAssetFile[] assetFiles = encoderOutputAsset.AssetFiles.ToArray();
            for (int i = assetFiles.Length - 1; i >=0; i--)
            {
                IAssetFile assetFile = assetFiles[i];
                if (assetFile.Name.EndsWith(fileSuffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    string documentId = assetFile.Name.Replace(fileSuffix, string.Empty);
                    databaseClient.DeleteDocument(collectionId, documentId);
                    assetFile.Delete();
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
                IAsset parentAsset = GetParentAsset(job);
                IAsset[] encoderOutputAssets = GetChildEncodes(mediaClient, parentAsset);
                foreach (IAsset encoderOutputAsset in encoderOutputAssets)
                {
                    PublishMetadata(job, encoderOutputAsset, jobPublish);
                    PublishIndex(job, encoderOutputAsset, jobPublish);
                }
            }
            else
            {
                foreach (ITask jobTask in jobTasks)
                {
                    foreach (IAsset encoderOutputAsset in jobTask.OutputAssets)
                    {
                        PublishLocator(mediaClient, encoderOutputAsset, contentProtection);
                        PublishMetadata(job, encoderOutputAsset, jobPublish);
                        PublishIndex(job, encoderOutputAsset, jobPublish);
                    }
                }
            }
        }

        private static void PublishMetadata(IJob job, IAsset encoderOutputAsset, JobPublish jobPublish)
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
                BlobClient blobClient = GetBlobClient(jobPublish);
                using (DatabaseClient databaseClient = new DatabaseClient(true))
                {
                    foreach (ITask jobTask in jobTasks)
                    {
                        MediaProcessor mediaProcessor = Processor.GetProcessorType(jobTask.MediaProcessorId);
                        foreach (IAsset outputAsset in jobTask.OutputAssets)
                        {
                            string fileExtension = Constant.Media.FileExtension.Json;
                            string[] fileNames = GetFileNames(outputAsset, fileExtension);
                            foreach (string fileName in fileNames)
                            {
                                string sourceContainerName = outputAsset.Uri.Segments[1];
                                CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, string.Empty, fileName, false);
                                string jsonData = string.Empty;
                                using (System.IO.Stream sourceStream = sourceBlob.OpenRead())
                                {
                                    StreamReader streamReader = new StreamReader(sourceStream);
                                    jsonData = streamReader.ReadToEnd();
                                }
                                if (!string.IsNullOrEmpty(jsonData))
                                {
                                    string collectionId = Constant.Database.DocumentCollection.Metadata;
                                    string fileSuffix = string.Concat(Constant.TextDelimiter.Identifier, mediaProcessor.ToString(), fileExtension);
                                    ClearMetadata(databaseClient, collectionId, fileSuffix, encoderOutputAsset, mediaProcessor);
                                    string documentId = databaseClient.CreateDocument(collectionId, jsonData, jobPublish.PartitionKey, encoderOutputAsset.Id);
                                    string destinationFileName = string.Concat(documentId, Constant.TextDelimiter.Identifier, mediaProcessor.ToString(), fileExtension);
                                    blobClient.CopyFile(outputAsset, encoderOutputAsset, fileName, destinationFileName, false);
                                }
                            }
                            if (mediaProcessor == MediaProcessor.FaceRedaction && jobTask.Configuration.Contains("analyze"))
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
            }
        }

        private static void PublishIndex(IJob job, IAsset encoderOutputAsset, JobPublish jobPublish)
        {
            string processorId = Constant.Media.ProcessorId.Indexer;
            string[] processorIds = new string[] { processorId };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length > 0)
            {
                BlobClient blobClient = GetBlobClient(jobPublish);
                foreach (ITask jobTask in jobTasks)
                {
                    IAsset outputAsset = jobTask.OutputAssets[0];
                    string fileExtension = Constant.Media.FileExtension.WebVtt;
                    string[] fileNames = GetFileNames(outputAsset, fileExtension);
                    foreach (string fileName in fileNames)
                    {
                        string languageCode = Language.GetLanguageCode(jobTask.Configuration);
                        string languageExtension = string.Concat(Constant.TextDelimiter.Identifier, languageCode, fileExtension);
                        string languageFileName = fileName.Replace(fileExtension, languageExtension);
                        blobClient.CopyFile(outputAsset, encoderOutputAsset, fileName, languageFileName, false);
                    }
                }
            }
        }

        internal static void PublishLocator(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
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
                mediaClient.CreateLocator(null, locatorType, asset, null);
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
                        jobPublication.UserId = jobPublication.UserId;
                        jobPublication.MobileNumber = jobPublish.MobileNumber;
                        ContentProtection contentProtection = GetContentProtection(entityClient, jobPublish);
                        string accountName = jobPublish.PartitionKey;
                        string accountKey = jobPublish.MediaAccountKey;
                        MediaClient mediaClient = new MediaClient(accountName, accountKey);
                        IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobPublish.RowKey) as IJob;
                        if (job != null)
                        {
                            mediaClient.SetProcessorUnits(job, ReservedUnitType.Basic);
                            if (jobNotification.Properties.NewState == JobState.Finished)
                            {
                                PublishContent(mediaClient, job, jobPublish, contentProtection);
                                jobPublication.UserMessage = GetNotificationMessage(accountName, job);
                            }
                        }
                        string tableName = Constant.Storage.TableName.JobPublish;
                        entityClient.DeleteEntity(tableName, jobPublish);
                        if (contentProtection != null)
                        {
                            tableName = Constant.Storage.TableName.JobPublishProtection;
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
    }
}
