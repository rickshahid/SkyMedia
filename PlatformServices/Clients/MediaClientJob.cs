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
                string settingKey = Constant.AppSettingKey.MediaJobNotificationWebHookUrl;
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

        private void SetProcessorUnits(IJob job, ReservedUnitType scale)
        {
            IEncodingReservedUnit[] processorUnits = GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
            processorUnits[0].CurrentReservedUnits = (job.State == JobState.Queued) ? job.Tasks.Count : 0;
            processorUnits[0].ReservedUnitType = scale;
            processorUnits[0].Update();
        }

        internal static MediaJob GetJob(MediaClient mediaClient, MediaJob mediaJob, MediaAssetInput[] inputAssets)
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

        internal IJob CreateJob(MediaJob mediaJob, out IJobTemplate jobTemplate)
        {
            IJob job;
            jobTemplate = null;
            if (!string.IsNullOrEmpty(mediaJob.TemplateId))
            {
                List<IAsset> inputAssets = new List<IAsset>();
                foreach (MediaJobTask jobTask in mediaJob.Tasks)
                {
                    IAsset[] assets = GetAssets(jobTask.InputAssetIds);
                    inputAssets.AddRange(assets);
                }
                IJobTemplate template = GetEntityById(MediaEntity.JobTemplate, mediaJob.TemplateId) as IJobTemplate;
                job = _media.Jobs.Create(mediaJob.Name, template, inputAssets, mediaJob.Priority);
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
            if (mediaJob.Save)
            {
                string templateName = mediaJob.Name;
                jobTemplate = job.SaveAsTemplate(templateName);
            }
            else
            {
                SetProcessorUnits(job, mediaJob.NodeType);
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
