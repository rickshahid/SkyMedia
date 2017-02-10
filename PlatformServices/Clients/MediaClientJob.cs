using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private string GetProcessorId(MediaProcessor mediaProcessor)
        {
            string processorId = null;
            switch (mediaProcessor)
            {
                case MediaProcessor.EncoderStandard:
                    string settingKey = Constants.AppSettingKey.MediaProcessorEncoderStandardId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.EncoderPremium:
                    settingKey = Constants.AppSettingKey.MediaProcessorEncoderPremiumId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.EncoderUltra:
                    settingKey = Constants.AppSettingKey.MediaProcessorEncoderUltraId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.IndexerV1:
                    settingKey = Constants.AppSettingKey.MediaProcessorIndexerV1Id;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.IndexerV2:
                    settingKey = Constants.AppSettingKey.MediaProcessorIndexerV2Id;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.FaceDetection:
                    settingKey = Constants.AppSettingKey.MediaProcessorFaceDetectionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.FaceRedaction:
                    settingKey = Constants.AppSettingKey.MediaProcessorFaceRedactionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.MotionDetection:
                    settingKey = Constants.AppSettingKey.MediaProcessorMotionDetectionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.MotionHyperlapse:
                    settingKey = Constants.AppSettingKey.MediaProcessorMotionHyperlapseId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.MotionStablization:
                    settingKey = Constants.AppSettingKey.MediaProcessorMotionStabilizationId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.VideoSummarization:
                    settingKey = Constants.AppSettingKey.MediaProcessorVideoSummarizationId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.ThumbnailGeneration:
                    settingKey = Constants.AppSettingKey.MediaProcessorThumbnailGenerationId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
                case MediaProcessor.CharacterRecognition:
                    settingKey = Constants.AppSettingKey.MediaProcessorCharacterRecognitionId;
                    processorId = AppSetting.GetValue(settingKey);
                    break;
            }
            return processorId;
        }

        private INotificationEndPoint GetNotificationEndpoint(NotificationEndPointType endpointType)
        {
            string endpointName = Constants.Media.Job.NotificationEndpointNameStorageQueue;
            string settingKey = Constants.AppSettingKey.MediaJobNotificationStorageQueueName;
            string endpointAddress = AppSetting.GetValue(settingKey);
            if (endpointType == NotificationEndPointType.WebHook)
            {
                endpointName = Constants.Media.Job.NotificationEndpointNameWebHook;
                settingKey = Constants.AppSettingKey.MediaJobNotificationWebHookUrl;
                endpointAddress = AppSetting.GetValue(settingKey);
            }
            INotificationEndPoint notificationEndpoint = null;
            if (!string.IsNullOrEmpty(endpointAddress))
            {
                notificationEndpoint = GetEntityByName(MediaEntity.NotificationEndpoint, endpointName, true) as INotificationEndPoint;
                if (notificationEndpoint == null)
                {
                    notificationEndpoint = _media.NotificationEndPoints.Create(endpointName, endpointType, endpointAddress);
                }
            }
            return notificationEndpoint;
        }

        private static string GetNotificationMessage(string accountName, IJob job)
        {
            string messageText = string.Concat("Azure Media Services Job ", job.State.ToString(), ".");
            messageText = string.Concat(messageText, " Account Name: ", accountName);
            messageText = string.Concat(messageText, ", Job Name: ", job.Name);
            messageText = string.Concat(messageText, ", Job ID: ", job.Id);
            return string.Concat(messageText, ", Job Running Duration: ", job.RunningDuration.ToString(Constants.FormatTime));
        }

        private void SetProcessorUnits(IJob job, ReservedUnitType scale)
        {
            IEncodingReservedUnit[] processorUnits = GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
            processorUnits[0].CurrentReservedUnits = (job.State == JobState.Queued) ? job.Tasks.Count : 0;
            processorUnits[0].ReservedUnitType = scale;
            processorUnits[0].Update();
        }

        internal static MediaJob SetJob(MediaClient mediaClient, MediaJob mediaJob, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> taskList = new List<MediaJobTask>();
            foreach (MediaJobTask jobTask in mediaJob.Tasks)
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
                    case MediaProcessor.VideoAnnotation:
                        tasks = GetVideoAnnotationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.VideoSummarization:
                        tasks = GetVideoSummarizationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.ThumbnailGeneration:
                        tasks = GetThumbnailGenerationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.CharacterRecognition:
                        tasks = GetCharacterRecognitionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.ContentModeration:
                        tasks = GetContentModerationTasks(mediaClient, jobTask, inputAssets);
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
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                mediaJob.Name = string.Concat(taskName, " (", asset.Name, ")");
            }
            return mediaJob;
        }

        internal IJob CreateJob(MediaJob mediaJob)
        {
            IJob job = _media.Jobs.Create(mediaJob.Name, mediaJob.Priority);
            foreach (MediaJobTask jobTask in mediaJob.Tasks)
            {
                string processorId = GetProcessorId(jobTask.MediaProcessor);
                IMediaProcessor processor = GetEntityById(MediaEntity.Processor, processorId) as IMediaProcessor;
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
            INotificationEndPoint notificationEndpoint = GetNotificationEndpoint(mediaJob.Notification);
            if (notificationEndpoint != null)
            {
                job.JobNotificationSubscriptions.AddNew(NotificationJobState.FinalStatesOnly, notificationEndpoint);
            }
            SetProcessorUnits(job, mediaJob.Scale);
            job.Submit();
            return job;
        }

        internal static void TrackJob(string authToken, IJob job, string storageAccount, ContentProtection[] contentProtections)
        {
            string attributeName = Constants.UserAttribute.MediaAccountName;
            string accountName = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.MediaAccountKey;
            string accountKey = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.UserId;
            string userId = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.MobileNumber;
            string mobileNumber = AuthToken.GetClaimValue(authToken, attributeName);

            JobPublish jobPublish = new JobPublish();
            jobPublish.PartitionKey = accountName;
            jobPublish.RowKey = job.Id;

            jobPublish.MediaAccountKey = accountKey;
            jobPublish.StorageAccountName = storageAccount;
            jobPublish.StorageAccountKey = Storage.GetUserAccountKey(authToken, storageAccount);
            jobPublish.UserId = userId;
            jobPublish.MobileNumber = mobileNumber;

            EntityClient entityClient = new EntityClient();

            string tableName = Constants.Storage.TableNames.JobPublish;
            entityClient.InsertEntity(tableName, jobPublish);

            tableName = Constants.Storage.TableNames.JobPublishProtection;
            foreach (ContentProtection contentProtection in contentProtections)
            {
                contentProtection.PartitionKey = jobPublish.PartitionKey;
                contentProtection.RowKey = jobPublish.RowKey;
                entityClient.InsertEntity(tableName, contentProtection);
            }
        }
    }
}
