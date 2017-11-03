using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private INotificationEndPoint GetNotificationEndpoint()
        {
            string endpointName = Constant.Media.JobNotification.EndpointName;
            INotificationEndPoint notificationEndpoint = GetEntityByName(MediaEntity.NotificationEndpoint, endpointName, true) as INotificationEndPoint;
            if (notificationEndpoint == null)
            {
                NotificationEndPointType endpointType = NotificationEndPointType.WebHook;
                string settingKey = Constant.AppSettingKey.MediaPublishContentUrl;
                string endpointAddress = AppSetting.GetValue(settingKey);
                notificationEndpoint = _media.NotificationEndPoints.Create(endpointName, endpointType, endpointAddress);
            }
            return notificationEndpoint;
        }

        private void SetProcessorUnits(IJob job, IJobTemplate jobTemplate, ReservedUnitType nodeType, bool newJob)
        {
            int unitCount = jobTemplate != null ? jobTemplate.TaskTemplates.Count : job.Tasks.Count;
            IEncodingReservedUnit[] processorUnits = GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
            processorUnits[0].ReservedUnitType = nodeType;
            if (newJob)
            {
                processorUnits[0].CurrentReservedUnits += unitCount;
            }
            else
            {
                processorUnits[0].CurrentReservedUnits -= unitCount;
            }
            processorUnits[0].Update();
        }

        internal static MediaJob GetJob(string authToken, MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> taskList = new List<MediaJobTask>();
            foreach (MediaJobTask jobTask in mediaJob.Tasks)
            {
                MediaJobTask[] tasks = null;
                switch (jobTask.MediaProcessor)
                {
                    case MediaProcessor.EncoderStandard:
                    case MediaProcessor.EncoderPremium:
                        tasks = GetEncoderTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.VideoIndexer:
                        IndexerClient indexerClient = new IndexerClient(authToken, null, null);
                        foreach (MediaJobInput jobInput in jobInputs)
                        {
                            if (jobInput.WorkflowView)
                            {
                                string indexId = indexerClient.GetIndexId(jobInput.AssetId);
                                if (!string.IsNullOrEmpty(indexId))
                                {
                                    indexerClient.ResetIndex(indexId);
                                }
                            }
                            else
                            {
                                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, jobInput.AssetId) as IAsset;
                                string locatorUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, asset, null, false);
                                IndexVideo(authToken, indexerClient, asset, locatorUrl, jobTask);
                            }
                        }
                        break;
                    case MediaProcessor.VideoAnnotation:
                        tasks = GetVideoAnnotationTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.VideoSummarization:
                        tasks = GetVideoSummarizationTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.SpeechAnalyzer:
                        tasks = GetSpeechAnalyzerTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.FaceDetection:
                        tasks = GetFaceDetectionTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.FaceRedaction:
                        tasks = GetFaceRedactionTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.MotionDetection:
                        tasks = GetMotionDetectionTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.MotionHyperlapse:
                        tasks = GetMotionHyperlapseTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.MotionStabilization:
                        tasks = GetMotionStabilizationTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.CharacterRecognition:
                        tasks = GetCharacterRecognitionTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.ContentModeration:
                        tasks = GetContentModerationTasks(mediaClient, jobTask, jobInputs);
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
                mediaJob.Name = jobInputs[0].AssetName;
            }
            return mediaJob;
        }

        internal IJob CreateJob(MediaJob mediaJob, MediaJobInput[] jobInputs, out IJobTemplate jobTemplate)
        {
            IJob job = null;
            jobTemplate = null;
            if (!string.IsNullOrEmpty(mediaJob.TemplateId))
            {
                List<IAsset> jobInputList = new List<IAsset>();
                foreach (MediaJobInput jobInput in jobInputs)
                {
                    IAsset asset = GetEntityById(MediaEntity.Asset, jobInput.AssetId) as IAsset;
                    if (asset != null)
                    {
                        jobInputList.Add(asset);
                    }
                }
                jobTemplate = GetEntityById(MediaEntity.JobTemplate, mediaJob.TemplateId) as IJobTemplate;
                job = _media.Jobs.Create(mediaJob.Name, jobTemplate, jobInputList, mediaJob.Priority);
            }
            else if (mediaJob.Tasks.Length > 0)
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
            if (job != null)
            {
                if (mediaJob.SaveWorkflow)
                {
                    string templateName = mediaJob.Name;
                    jobTemplate = job.SaveAsTemplate(templateName);
                }
                else
                {
                    SetProcessorUnits(job, jobTemplate, mediaJob.NodeType, true);
                    job.Submit();
                }
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