using System.Globalization;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static MediaJobTask[] GetIndexerTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.Indexer;
            string settingKey = Constant.AppSettingKey.MediaProcessorIndexerDocumentId;
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
                MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
                jobTasks.AddRange(mappedJobTasks);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.IndexerSpokenLanguages)
                {
                    processorOptions["Language"] = spokenLanguage;
                    jobTask.ProcessorConfig = processorConfig.ToString();
                    MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
                    jobTasks.AddRange(mappedJobTasks);
                }
            }
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetVideoAnnotationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.VideoAnnotation;
            string settingKey = Constant.AppSettingKey.MediaProcessorVideoAnnotationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.VideoSummarization;
            string settingKey = Constant.AppSettingKey.MediaProcessorVideoSummarizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["maxMotionThumbnailDurationInSecs"] = jobTask.SummaryDurationSeconds;
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.FaceDetection;
            string settingKey = Constant.AppSettingKey.MediaProcessorFaceDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["mode"] = jobTask.FaceDetectionMode;
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceRedactionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.FaceRedaction;
            jobTask.Name = string.Concat(jobTask.Name, " ", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(jobTask.FaceRedactionMode));
            string settingKey = Constant.AppSettingKey.MediaProcessorFaceRedactionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["mode"] = jobTask.FaceRedactionMode;
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.MotionDetection;
            string settingKey = Constant.AppSettingKey.MediaProcessorMotionDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionHyperlapseTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.MotionHyperlapse;
            string settingKey = Constant.AppSettingKey.MediaProcessorMotionHyperlapseDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorSources = processorConfig["Sources"][0];
            processorSources["StartFrame"] = jobTask.HyperlapseStartFrame;
            processorSources["NumFrames"] = jobTask.HyperlapseFrameCount;
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Speed"] = jobTask.HyperlapseSpeed;
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionStabilizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.MotionStabilization;
            string settingKey = Constant.AppSettingKey.MediaProcessorMotionStabilizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetCharacterRecognitionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.CharacterRecognition;
            string settingKey = Constant.AppSettingKey.MediaProcessorCharacterRecognitionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetContentModerationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.ContentModeration;
            string settingKey = Constant.AppSettingKey.MediaProcessorContentModerationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }
    }
}
