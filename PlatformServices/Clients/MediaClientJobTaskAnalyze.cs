using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static MediaJobTask[] GetVideoAnnotationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.VideoAnnotation;
            string settingKey = Constant.AppSettingKey.MediaProcessorVideoAnnotationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.VideoSummarization;
            string settingKey = Constant.AppSettingKey.MediaProcessorVideoSummarizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["MaxMotionThumbnailDurationInSecs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.SummarizationDurationSeconds.ToString()];
            processorOptions["FadeInFadeOut"] = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.SummarizationFadeTransitions.ToString()];
            processorOptions["OutputAudio"] = !jobTask.ProcessorConfigBoolean[MediaProcessorConfig.VideoOnly.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetCharacterRecognitionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.CharacterRecognition;
            string settingKey = Constant.AppSettingKey.MediaProcessorCharacterRecognitionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetContentModerationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.ContentModeration;
            string settingKey = Constant.AppSettingKey.MediaProcessorContentModerationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetSpeechAnalyzerTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.SpeechAnalyzer;
            string settingKey = Constant.AppSettingKey.MediaProcessorSpeechAnalyzerDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Features"][0]["Options"];
            JArray timedTextFormats = new JArray();
            if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.TimedTextFormatWebVtt.ToString()])
            {
                timedTextFormats.Add("WebVTT");
            }
            if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.TimedTextFormatTtml.ToString()])
            {
                timedTextFormats.Add("TTML");
            }
            processorOptions["Formats"] = timedTextFormats;
            processorOptions["Language"] = jobTask.ProcessorConfigString[MediaProcessorConfig.LanguageId.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.FaceDetection;
            string settingKey = Constant.AppSettingKey.MediaProcessorFaceDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Mode"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceDetectionMode.ToString()];
            processorOptions["TrackingMode"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceDetectionTrackingMode.ToString()];
            processorOptions["AggregateEmotionWindowMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceDetectionAggregateEmotionWindow.ToString()];
            processorOptions["AggregateEmotionIntervalMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceDetectionAggregateEmotionInterval.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetFaceRedactionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.FaceRedaction;
            string settingKey = Constant.AppSettingKey.MediaProcessorFaceRedactionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Mode"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceRedactionMode.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.MotionDetection;
            string settingKey = Constant.AppSettingKey.MediaProcessorMotionDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetMotionHyperlapseTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            int frameStart = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseFrameStart.ToString()];
            int frameEnd = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseFrameEnd.ToString()];
            jobTask.MediaProcessor = MediaProcessor.MotionHyperlapse;
            string settingKey = Constant.AppSettingKey.MediaProcessorMotionHyperlapseDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorSources = processorConfig["Sources"][0];
            processorSources["StartFrame"] = frameStart;
            processorSources["NumFrames"] = frameEnd - frameStart + 1;
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Speed"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseSpeed.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }
    }
}