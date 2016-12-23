using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private string GetProcessorId(MediaProcessor mediaProcessor)
        {
            string processorId = null;
            switch (mediaProcessor)
            {
                case MediaProcessor.EncoderStandard:
                    string settingKey = Constants.AppSettings.MediaProcessorEncoderStandardId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.EncoderPremium:
                    settingKey = Constants.AppSettings.MediaProcessorEncoderPremiumId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.IndexerV1:
                    settingKey = Constants.AppSettings.MediaProcessorIndexerV1Id;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.IndexerV2:
                    settingKey = Constants.AppSettings.MediaProcessorIndexerV2Id;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.FaceDetection:
                case MediaProcessor.FaceDetectionEmotion:
                    settingKey = Constants.AppSettings.MediaProcessorFaceDetectionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.FaceRedaction:
                    settingKey = Constants.AppSettings.MediaProcessorFaceRedactionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.MotionDetection:
                    settingKey = Constants.AppSettings.MediaProcessorMotionDetectionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.MotionHyperlapse:
                    settingKey = Constants.AppSettings.MediaProcessorMotionHyperlapseId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.VideoSummarization:
                    settingKey = Constants.AppSettings.MediaProcessorVideoSummarizationId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.CharacterRecognition:
                    settingKey = Constants.AppSettings.MediaProcessorCharacterRecognitionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
            }
            return processorId;
        }

        private INotificationEndPoint GetNotificationEndpoint()
        {
            string endpointName = Constants.Media.Job.NotificationEndpointName;
            INotificationEndPoint notificationEndpoint = GetEntityByName(EntityType.NotificationEndpoint, endpointName, true) as INotificationEndPoint;
            if (notificationEndpoint == null)
            {
                string queueName = Constants.Storage.QueueName.JobStatus;
                NotificationEndPointType endpointType = NotificationEndPointType.AzureQueue;
                notificationEndpoint = _media.NotificationEndPoints.Create(endpointName, endpointType, queueName);
            }
            return notificationEndpoint;
        }

        internal static string GetNotificationMessage(string accountName, IJob job)
        {
            string messageText = string.Concat("Azure Media Services Job ", job.State.ToString(), ".");
            messageText = string.Concat(messageText, " Account Name: ", accountName);
            messageText = string.Concat(messageText, ", Job Name: ", job.Name);
            messageText = string.Concat(messageText, ", Job ID: ", job.Id);
            return string.Concat(messageText, ", Job Running Duration: ", job.RunningDuration.ToString(Constants.FormatTime));
        }

        public static MediaJob CreateJob(MediaClient mediaClient, string jobName, int jobPriority, MediaJobTask[] jobTasks,
                                         MediaAssetInput[] inputAssets)
        {
            MediaJob mediaJob = new MediaJob();
            mediaJob.Name = jobName;
            mediaJob.Priority = jobPriority;
            List<MediaJobTask> taskList = new List<MediaJobTask>();
            foreach (MediaJobTask jobTask in jobTasks)
            {
                MediaJobTask[] tasks = null;
                switch (jobTask.MediaProcessor)
                {
                    case MediaProcessor.EncoderStandard:
                    case MediaProcessor.EncoderPremium:
                        tasks = GetEncoderTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.IndexerV1:
                        tasks = GetIndexerV1Tasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.IndexerV2:
                        tasks = GetIndexerV2Tasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.FaceDetection:
                        tasks = GetFaceDetectionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.FaceRedaction:
                        tasks = GetFaceRedactionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.MotionDetection:
                        tasks = GetMotionDetectionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.MotionHyperlapse:
                        tasks = GetMotionHyperlapseTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.VideoSummarization:
                        tasks = GetVideoSummarizationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.CharacterRecognition:
                        tasks = GetCharacterRecognitionTasks(mediaClient, jobTask, inputAssets);
                        break;
                }
                if (tasks != null)
                {
                    taskList.AddRange(tasks);
                }
            }
            mediaJob.Tasks = taskList.ToArray();
            if (string.IsNullOrEmpty(mediaJob.Name))
            {
                string taskName = mediaJob.Tasks[0].Name;
                string assetId = inputAssets[0].AssetId;
                IAsset asset = mediaClient.GetEntityById(EntityType.Asset, assetId) as IAsset;
                mediaJob.Name = string.Concat(taskName, " (", asset.Name, ")");
            }
            return mediaJob;
        }

        public IJob CreateJob(MediaJob mediaJob)
        {
            IJob job = _media.Jobs.Create(mediaJob.Name, mediaJob.Priority);
            foreach (MediaJobTask jobTask in mediaJob.Tasks)
            {
                string processorId = GetProcessorId(jobTask.MediaProcessor);
                IMediaProcessor processor = GetEntityById(EntityType.Processor, processorId) as IMediaProcessor;
                ITask currentTask = job.Tasks.AddNew(jobTask.Name, processor, jobTask.ProcessorConfig, jobTask.Options);
                if (jobTask.ParentIndex.HasValue)
                {
                    ITask parentTask = job.Tasks[jobTask.ParentIndex.Value];
                    currentTask.InputAssets.AddRange(parentTask.OutputAssets);
                }
                else
                {
                    IAsset[] assets = GetAssets(jobTask.InputAssetIds);
                    currentTask.InputAssets.AddRange(assets);
                }
                currentTask.OutputAssets.AddNew(jobTask.OutputAssetName, jobTask.OutputAssetEncryption, jobTask.OutputAssetFormat);
            }
            INotificationEndPoint notificationEndpoint = GetNotificationEndpoint();
            NotificationJobState jobStates = NotificationJobState.FinalStatesOnly;
            job.JobNotificationSubscriptions.AddNew(jobStates, notificationEndpoint);
            job.Submit();
            return job;
        }

        public static void TrackJob(string authToken, IJob job, string storageAccount, ContentProtection[] contentProtections)
        {
            string attributeName = Constants.UserAttribute.MediaAccountName;
            string accountName = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.MediaAccountKey;
            string accountKey = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.MobileNumber;
            string mobileNumber = AuthToken.GetClaimValue(authToken, attributeName);

            ContentPublish contentPublish = new ContentPublish();
            contentPublish.PartitionKey = accountName;
            contentPublish.RowKey = job.Id;

            contentPublish.MediaAccountKey = accountKey;
            contentPublish.StorageAccountName = storageAccount;
            contentPublish.StorageAccountKey = Storage.GetAccountKey(authToken, storageAccount);
            contentPublish.MobileNumber = mobileNumber;

            EntityClient entityClient = new EntityClient();

            string tableName = Constants.Storage.TableNames.AssetPublish;
            entityClient.InsertEntity(tableName, contentPublish);

            tableName = Constants.Storage.TableNames.AssetProtection;
            foreach (ContentProtection contentProtection in contentProtections)
            {
                contentProtection.PartitionKey = contentPublish.PartitionKey;
                contentProtection.RowKey = contentPublish.RowKey;
                entityClient.InsertEntity(tableName, contentProtection);
            }
        }
    }
}
