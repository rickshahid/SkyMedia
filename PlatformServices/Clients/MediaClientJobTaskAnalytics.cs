using System.Globalization;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.FaceDetection;
            string settingKey = Constants.AppSettingKey.MediaProcessorFaceDetectionDocumentId;
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
            string settingKey = Constants.AppSettingKey.MediaProcessorFaceRedactionDocumentId;
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
            string settingKey = Constants.AppSettingKey.MediaProcessorMotionDetectionDocumentId;
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
            string settingKey = Constants.AppSettingKey.MediaProcessorMotionHyperlapseDocumentId;
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
            string settingKey = Constants.AppSettingKey.MediaProcessorMotionStabilizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetVideoAnnotationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.VideoAnnotation;
            string settingKey = Constants.AppSettingKey.MediaProcessorVideoAnnotationDocumentId;
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
            string settingKey = Constants.AppSettingKey.MediaProcessorVideoSummarizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["options"];
            processorOptions["maxMotionThumbnailDurationInSecs"] = jobTask.SummaryDurationSeconds;
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetCharacterRecognitionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.CharacterRecognition;
            string settingKey = Constants.AppSettingKey.MediaProcessorCharacterRecognitionDocumentId;
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
            string settingKey = Constants.AppSettingKey.MediaProcessorContentModerationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }
    }
}
