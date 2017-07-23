using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private INotificationEndPoint GetNotificationEndpoint()
        {
            string endpointName = Constant.Media.JobNotification.EndpointName;
            INotificationEndPoint notificationEndpoint = GetEntityByName(MediaEntity.NotificationEndpoint, endpointName, true) as INotificationEndPoint;
            if (notificationEndpoint == null)
            {
                NotificationEndPointType endpointType = NotificationEndPointType.WebHook;
                string settingKey = Constant.AppSettingKey.MediaNotificationWebHookUrl;
                string endpointAddress = AppSetting.GetValue(settingKey);
                notificationEndpoint = _media.NotificationEndPoints.Create(endpointName, endpointType, endpointAddress);
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

        private void SetProcessorUnits(IJob job, IJobTemplate jobTemplate, ReservedUnitType nodeType)
        {
            int taskCount = jobTemplate != null ? jobTemplate.TaskTemplates.Count : job.Tasks.Count;
            IEncodingReservedUnit[] processorUnits = GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
            processorUnits[0].CurrentReservedUnits = (job.State == JobState.Queued) ? taskCount : 0;
            processorUnits[0].ReservedUnitType = nodeType;
            processorUnits[0].Update();
        }

        internal static MediaJob GetJob(string authToken, MediaClient mediaClient, MediaJob mediaJob, MediaAssetInput[] inputAssets)
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
                    case MediaProcessor.VideoIndexer:
                        string attributeName = Constant.UserAttribute.VideoIndexerKey;
                        string indexerKey = AuthToken.GetClaimValue(authToken, attributeName);
                        if (!string.IsNullOrEmpty(indexerKey))
                        {
                            IndexerClient indexerClient = new IndexerClient(indexerKey);
                            foreach (MediaAssetInput inputAsset in inputAssets)
                            {
                                if (!string.IsNullOrEmpty(inputAsset.AlternateId))
                                {
                                    string indexId = MediaClient.GetIndexId(inputAsset.AlternateId);
                                    if (!string.IsNullOrEmpty(indexId))
                                    {
                                        indexerClient.ResetIndex(inputAsset.AssetId, indexId);
                                    }
                                }
                                else
                                {
                                    IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, inputAsset.AssetId) as IAsset;
                                    string locatorUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, asset, null, true);
                                    MediaClient.UploadVideo(authToken, indexerKey, indexerClient, asset, locatorUrl, jobTask);
                                }
                            }
                        }
                        break;
                    case MediaProcessor.VideoAnnotation:
                        tasks = GetVideoAnnotationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.VideoSummarization:
                        tasks = GetVideoSummarizationTasks(mediaClient, jobTask, inputAssets);
                        break;
                    case MediaProcessor.SpeechToText:
                        tasks = GetSpeechToTextTasks(mediaClient, jobTask, inputAssets);
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
                    case MediaProcessor.MotionStabilization:
                        tasks = GetMotionStabilizationTasks(mediaClient, jobTask, inputAssets);
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
                mediaJob.Name = inputAssets[0].AssetName;
            }
            return mediaJob;
        }

        internal IJob CreateJob(MediaJob mediaJob, MediaAssetInput[] inputAssets, out IJobTemplate jobTemplate)
        {
            IJob job;
            jobTemplate = null;
            if (!string.IsNullOrEmpty(mediaJob.TemplateId))
            {
                List<IAsset> inputAssetList = new List<IAsset>();
                foreach (MediaAssetInput inputAsset in inputAssets)
                {
                    IAsset asset = GetEntityById(MediaEntity.Asset, inputAsset.AssetId) as IAsset;
                    if (asset != null)
                    {
                        inputAssetList.Add(asset);
                    }
                }
                jobTemplate = GetEntityById(MediaEntity.JobTemplate, mediaJob.TemplateId) as IJobTemplate;
                job = _media.Jobs.Create(mediaJob.Name, jobTemplate, inputAssetList, mediaJob.Priority);
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
                INotificationEndPoint notificationEndpoint = GetNotificationEndpoint();
                if (notificationEndpoint != null)
                {
                    job.JobNotificationSubscriptions.AddNew(NotificationJobState.FinalStatesOnly, notificationEndpoint);
                }
            }
            if (mediaJob.SaveWorkflow)
            {
                string templateName = mediaJob.Name;
                jobTemplate = job.SaveAsTemplate(templateName);
            }
            else
            {
                SetProcessorUnits(job, jobTemplate, mediaJob.NodeType);
                job.Submit();
            }
            return job;
        }

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
    }
}
