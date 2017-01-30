using System.Xml;
using System.Globalization;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static JObject GetProcessorConfig(string documentId)
        {
            JObject processorConfig;
            using (DatabaseClient databaseClient = new DatabaseClient(false))
            {
                processorConfig = databaseClient.GetDocument(documentId);
            }
            return processorConfig;
        }

        private static MediaJobTask[] GetIndexerV1Tasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.IndexerV1;
            string settingKey = Constants.AppSettingKey.MediaProcessorIndexerV1Id;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorIndexerV1DocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            XmlDocument processorConfigXml = new XmlDocument();
            processorConfigXml.LoadXml(processorConfig["Xml"].ToString());
            XmlNodeList configSettings = processorConfigXml.SelectNodes(Constants.Media.ProcessorConfig.IndexerV1XPath);
            List<string> captionFormats = new List<string>();
            if (jobTask.IndexerCaptionWebVtt)
            {
                captionFormats.Add("WebVTT");
            }
            if (jobTask.IndexerCaptionTtml)
            {
                captionFormats.Add("TTML");
            }
            if (captionFormats.Count > 0)
            {
                configSettings[1].Attributes[1].Value = string.Join(";", captionFormats);
            }
            if (jobTask.IndexerSpokenLanguages == null)
            {
                jobTask.ProcessorConfig = processorConfigXml.OuterXml;
                jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.IndexerSpokenLanguages)
                {
                    configSettings[0].Attributes[1].Value = spokenLanguage;
                    jobTask.ProcessorConfig = processorConfigXml.OuterXml;
                    jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetIndexerV2Tasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.IndexerV2;
            string settingKey = Constants.AppSettingKey.MediaProcessorIndexerV2Id;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorIndexerV2DocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Features"][0]["Options"];
            JArray captionFormats = new JArray();
            if (jobTask.IndexerCaptionWebVtt)
            {
                captionFormats.Add("WebVTT");
            }
            if (jobTask.IndexerCaptionTtml)
            {
                captionFormats.Add("TTML");
            }
            if (captionFormats.Count > 0)
            {
                processorOptions["Formats"] = captionFormats;
            }
            if (jobTask.IndexerSpokenLanguages == null)
            {
                jobTask.ProcessorConfig = processorConfig.ToString();
                jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.IndexerSpokenLanguages)
                {
                    processorOptions["Language"] = spokenLanguage;
                    jobTask.ProcessorConfig = processorConfig.ToString();
                    jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.FaceDetection;
            string settingKey = Constants.AppSettingKey.MediaProcessorFaceDetectionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorFaceDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["mode"] = jobTask.FaceDetectionMode;
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceRedactionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.FaceRedaction;
            jobTask.Name = string.Concat(jobTask.Name, " ", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(jobTask.FaceRedactionMode));
            string settingKey = Constants.AppSettingKey.MediaProcessorFaceRedactionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorFaceRedactionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["mode"] = jobTask.FaceRedactionMode;
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.MotionDetection;
            string settingKey = Constants.AppSettingKey.MediaProcessorMotionDetectionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorMotionDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionHyperlapseTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.MotionHyperlapse;
            string settingKey = Constants.AppSettingKey.MediaProcessorMotionHyperlapseId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorMotionHyperlapseDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorSources = processorConfig["Sources"][0];
            processorSources["StartFrame"] = jobTask.HyperlapseStartFrame;
            processorSources["NumFrames"] = jobTask.HyperlapseFrameCount;
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Speed"] = jobTask.HyperlapseSpeed;
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetVideoAnnotationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.VideoAnnotation;
            string settingKey = Constants.AppSettingKey.MediaProcessorVideoAnnotationId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorVideoAnnotationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.VideoSummarization;
            string settingKey = Constants.AppSettingKey.MediaProcessorVideoSummarizationId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorVideoSummarizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["maxMotionThumbnailDurationInSecs"] = jobTask.SummaryDurationSeconds;
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetThumbnailGenerationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.ThumbnailGeneration;
            string settingKey = Constants.AppSettingKey.MediaProcessorThumbnailGenerationId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorThumbnailGenerationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetCharacterRecognitionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.CharacterRecognition;
            string settingKey = Constants.AppSettingKey.MediaProcessorCharacterRecognitionId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorCharacterRecognitionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetContentModerationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.ContentModeration;
            string settingKey = Constants.AppSettingKey.MediaProcessorContentModerationId;
            string processorId = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettingKey.MediaProcessorContentModerationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            jobTask = SetJobTask(mediaClient, jobTask, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }
    }
}
