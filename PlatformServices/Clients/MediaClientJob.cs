using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        public static NameValueCollection GetJobTemplates(string authToken)
        {
            NameValueCollection jobTemplates = new NameValueCollection();
            MediaClient mediaClient = new MediaClient(authToken);
            IJobTemplate[] templates = mediaClient.GetEntities(MediaEntity.JobTemplate) as IJobTemplate[];
            foreach (IJobTemplate template in templates)
            {
                jobTemplates.Add(template.Name, template.Id);
            }
            return jobTemplates;
        }

        private INotificationEndPoint GetNotificationEndpoint(NotificationEndPointType endpointType)
        {
            string endpointName = Constant.Media.JobNotification.EndpointNameStorageQueue;
            string settingKey = Constant.AppSettingKey.MediaJobNotificationStorageQueueName;
            string endpointAddress = AppSetting.GetValue(settingKey);
            if (endpointType == NotificationEndPointType.WebHook)
            {
                endpointName = Constant.Media.JobNotification.EndpointNameWebHook;
                settingKey = Constant.AppSettingKey.MediaJobNotificationWebHookUrl;
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
            return string.Concat(messageText, ", Job Running Duration: ", job.RunningDuration.ToString(Constant.TextFormatter.ClockTime));
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
                    case MediaProcessor.EncoderUltra:
                        tasks = GetEncoderTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.Indexer:
                        tasks = GetIndexerTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.VideoAnnotation:
                        tasks = GetVideoAnnotationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.VideoSummarization:
                        tasks = GetVideoSummarizationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.FaceDetection:
                        tasks = GetFaceDetectionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.FaceRedaction:
                        tasks = GetFaceRedactionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.CharacterRecognition:
                        tasks = GetCharacterRecognitionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.ContentModeration:
                        tasks = GetContentModerationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.MotionDetection:
                        tasks = GetMotionDetectionTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.MotionHyperlapse:
                        tasks = GetMotionHyperlapseTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.MotionStabilization:
                        tasks = GetMotionStabilizationTasks(mediaClient, jobTask, inputAssets);
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
                mediaJob.Name = mediaJob.Tasks[0].Name;
            }
            return mediaJob;
        }

        internal IJob CreateJob(MediaJob mediaJob)
        {
            IJob job;
            if (!string.IsNullOrEmpty(mediaJob.TemplateId))
            {
                IJobTemplate jobTemplate = GetEntityById(MediaEntity.JobTemplate, mediaJob.TemplateId) as IJobTemplate;
                IAsset[] inputAssets = new IAsset[] { };
                job = _media.Jobs.Create(mediaJob.Name, jobTemplate, inputAssets, mediaJob.Priority);
            }
            else
            {
                job = _media.Jobs.Create(mediaJob.Name, mediaJob.Priority);
                foreach (MediaJobTask jobTask in mediaJob.Tasks)
                {
                    string processorId = Processor.GetProcessorId(jobTask.MediaProcessor);
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
                INotificationEndPoint notificationEndpoint = GetNotificationEndpoint(mediaJob.NotificationType);
                if (notificationEndpoint != null)
                {
                    job.JobNotificationSubscriptions.AddNew(NotificationJobState.FinalStatesOnly, notificationEndpoint);
                }
            }
            if (mediaJob.SaveAsTemplate)
            {
                string templateName = mediaJob.Name;
                job.SaveAsTemplate(templateName);
            }
            else
            {
                SetProcessorUnits(job, mediaJob.NodeType);
                job.Submit();
            }
            return job;
        }

        internal static void TrackJob(string authToken, IJob job, string storageAccount, ContentProtection[] contentProtections)
        {
            string attributeName = Constant.UserAttribute.MediaAccountName;
            string accountName = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.MediaAccountKey;
            string accountKey = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.UserId;
            string userId = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.MobileNumber;
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

            string tableName = Constant.Storage.TableName.JobPublish;
            entityClient.InsertEntity(tableName, jobPublish);

            tableName = Constant.Storage.TableName.JobPublishProtection;
            foreach (ContentProtection contentProtection in contentProtections)
            {
                contentProtection.PartitionKey = jobPublish.PartitionKey;
                contentProtection.RowKey = jobPublish.RowKey;
                entityClient.InsertEntity(tableName, contentProtection);
            }
        }
    }
}
