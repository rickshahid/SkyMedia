using System.Xml;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private static MediaJobTask[] GetIndexerV1Tasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.IndexerV1);
            MediaProcessor mediaProcessor = MediaProcessor.IndexerV1;
            string settingKey = Constants.AppSettings.MediaProcessorIndexerV1Id;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorIndexerV1DocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            XmlDocument processorConfigXml = new XmlDocument();
            processorConfigXml.LoadXml(processorConfig["Xml"].ToString());
            XmlNodeList configSettings = processorConfigXml.SelectNodes(Constants.Media.ProcessorConfig.IndexerV1XPath);
            List<string> captionFormats = new List<string>();
            if (jobTask.CaptionFormatWebVtt)
            {
                captionFormats.Add("WebVTT");
            }
            if (jobTask.CaptionFormatTtml)
            {
                captionFormats.Add("TTML");
            }
            if (captionFormats.Count > 0)
            {
                configSettings[1].Attributes[1].Value = string.Join(";", captionFormats);
            }
            if (jobTask.SpokenLanguages == null)
            {
                jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfigXml.OuterXml, inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.SpokenLanguages)
                {
                    configSettings[0].Attributes[1].Value = spokenLanguage;
                    jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfigXml.OuterXml, inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetIndexerV2Tasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.IndexerV2);
            MediaProcessor mediaProcessor = MediaProcessor.IndexerV2;
            string settingKey = Constants.AppSettings.MediaProcessorIndexerV2Id;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorIndexerV2DocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            JToken processorOptions = processorConfig["Features"][0]["Options"];
            JArray captionFormats = new JArray();
            if (jobTask.CaptionFormatWebVtt)
            {
                captionFormats.Add("WebVTT");
            }
            if (jobTask.CaptionFormatTtml)
            {
                captionFormats.Add("TTML");
            }
            if (captionFormats.Count > 0)
            {
                processorOptions["Formats"] = captionFormats;
            }
            if (jobTask.SpokenLanguages == null)
            {
                jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.SpokenLanguages)
                {
                    processorOptions["Language"] = spokenLanguage;
                    jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.FaceDetection);
            MediaProcessor mediaProcessor = MediaProcessor.FaceDetection;
            string settingKey = Constants.AppSettings.MediaProcessorFaceDetectionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorFaceDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceRedactionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.FaceRedaction);
            MediaProcessor mediaProcessor = MediaProcessor.FaceRedaction;
            string settingKey = Constants.AppSettings.MediaProcessorFaceRedactionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorFaceRedactionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.MotionDetection);
            MediaProcessor mediaProcessor = MediaProcessor.MotionDetection;
            string settingKey = Constants.AppSettings.MediaProcessorMotionDetectionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorMotionDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionHyperlapseTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.MotionHyperlapse);
            MediaProcessor mediaProcessor = MediaProcessor.MotionHyperlapse;
            string settingKey = Constants.AppSettings.MediaProcessorMotionHyperlapseId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorMotionHyperlapseDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.VideoSummarization);
            MediaProcessor mediaProcessor = MediaProcessor.VideoSummarization;
            string settingKey = Constants.AppSettings.MediaProcessorVideoSummarizationId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorVideoSummarizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["maxMotionThumbnailDurationInSecs"] = jobTask.DurationSeconds.ToString();
            jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetCharacterRecognitionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            string taskName = Selections.GetProcessorName(MediaProcessor.CharacterRecognition);
            MediaProcessor mediaProcessor = MediaProcessor.CharacterRecognition;
            string settingKey = Constants.AppSettings.MediaProcessorCharacterRecognitionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorCharacterRecognitionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            DatabaseClient databaseClient = new DatabaseClient();
            JObject processorConfig = databaseClient.GetDocument(documentId);
            jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }
    }
}
