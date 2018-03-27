using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private INotificationEndPoint GetNotificationEndpoint()
        {
            string settingKey = Constant.AppSettingKey.MediaPublishContentUrl;
            string endpointAddress = AppSetting.GetValue(settingKey);
            string endpointName = Constant.Media.Job.NotificationEndpointName;
            INotificationEndPoint notificationEndpoint = GetEntityByName(MediaEntity.NotificationEndpoint, endpointName) as INotificationEndPoint;
            if (notificationEndpoint != null && !string.Equals(notificationEndpoint.EndPointAddress, endpointAddress, StringComparison.OrdinalIgnoreCase))
            {
                notificationEndpoint.Delete();
                notificationEndpoint = null;
            }
            if (notificationEndpoint == null)
            {
                NotificationEndPointType endpointType = NotificationEndPointType.WebHook;
                notificationEndpoint = _media.NotificationEndPoints.Create(endpointName, endpointType, endpointAddress);
            }
            return notificationEndpoint;
        }

        private void SetProcessorUnits(IJob job, MediaJobNodeType nodeType, bool newJob)
        {
            int unitCount = job.Tasks.Count;
            IEncodingReservedUnit[] processorUnits = GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
            IEncodingReservedUnit processorUnit = processorUnits[0];
            processorUnit.ReservedUnitType = (ReservedUnitType)nodeType;
            if (newJob)
            {
                processorUnit.CurrentReservedUnits += unitCount;
            }
            else if (processorUnit.CurrentReservedUnits >= unitCount)
            {
                processorUnit.CurrentReservedUnits -= unitCount;
            }
            processorUnit.Update();
        }

        public static MediaJob GetJob(string authToken, MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
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
                        IndexerClient indexerClient = new IndexerClient(authToken);
                        if (indexerClient.IndexerEnabled)
                        {
                            foreach (MediaJobInput jobInput in jobInputs)
                            {
                                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, jobInput.AssetId) as IAsset;
                                string documentId = DocumentClient.GetDocumentId(asset, out bool videoIndexer);
                                if (videoIndexer)
                                {
                                    indexerClient.ResetIndex(authToken, documentId);
                                }
                                else
                                {
                                    indexerClient.IndexVideo(authToken, mediaClient, asset, jobTask.ContentIndexer);
                                }
                            }
                        }
                        break;
                    case MediaProcessor.VideoAnnotation:
                        tasks = GetVideoAnnotationTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.VideoSummarization:
                        tasks = GetVideoSummarizationTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.FaceDetection:
                        tasks = GetFaceDetectionTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.SpeechAnalyzer:
                        tasks = GetSpeechAnalyzerTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.MotionDetection:
                        tasks = GetMotionDetectionTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.ContentModeration:
                        tasks = GetContentModerationTasks(mediaClient, jobTask, jobInputs);
                        break;
                    case MediaProcessor.CharacterRecognition:
                        tasks = GetCharacterRecognitionTasks(mediaClient, jobTask, jobInputs);
                        break;
                }
                if (tasks != null)
                {
                    jobTasks.AddRange(tasks);
                }
            }
            mediaJob.Tasks = jobTasks.ToArray();
            if (string.IsNullOrEmpty(mediaJob.Name))
            {
                mediaJob.Name = jobInputs.Length > 1 ? Constant.Media.Job.MultipleInputAssets : jobInputs[0].AssetName;
            }
            return mediaJob;
        }

        public IJob CreateJob(MediaJob mediaJob, MediaJobInput[] jobInputs)
        {
            IJob job = _media.Jobs.Create(mediaJob.Name, mediaJob.Priority);
            foreach (MediaJobTask jobTask in mediaJob.Tasks)
            {
                string processorId = Processor.GetProcessorId(jobTask.MediaProcessor, jobTask.ProcessorConfig);
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
            job.JobNotificationSubscriptions.AddNew(NotificationJobState.FinalStatesOnly, notificationEndpoint);
            SetProcessorUnits(job, MediaJobNodeType.Premium, true);
            job.Submit();
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