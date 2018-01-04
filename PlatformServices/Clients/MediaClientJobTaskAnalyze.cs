using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static MediaJobTask[] GetVideoAnnotationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.VideoAnnotation;
            JObject processorConfig = GetProcessorConfig(jobTask);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.VideoSummarization;
            JObject processorConfig = GetProcessorConfig(jobTask);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["MaxMotionThumbnailDurationInSecs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.SummarizationDurationSeconds.ToString()];
            processorOptions["FadeInFadeOut"] = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.SummarizationFadeTransitions.ToString()];
            processorOptions["OutputAudio"] = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.SummarizationIncludeAudio.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.FaceDetection;
            JObject processorConfig = GetProcessorConfig(jobTask);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Mode"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceDetectionMode.ToString()];
            if (jobTask.ProcessorConfigString.ContainsKey(MediaProcessorConfig.FaceRedactionBlurMode.ToString()))
            {
                processorOptions["BlurType"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceRedactionBlurMode.ToString()];
            }
            if (jobTask.ProcessorConfigInteger.ContainsKey(MediaProcessorConfig.FaceEmotionAggregateWindow.ToString()))
            {
                processorOptions["AggregateEmotionWindowMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceEmotionAggregateWindow.ToString()];
            }
            if (jobTask.ProcessorConfigInteger.ContainsKey(MediaProcessorConfig.FaceEmotionAggregateInterval.ToString()))
            {
                processorOptions["AggregateEmotionIntervalMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceEmotionAggregateInterval.ToString()];
            }
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetSpeechAnalyzerTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.SpeechAnalyzer;
            JObject processorConfig = GetProcessorConfig(jobTask);
            JToken processorOptions = processorConfig["Features"][0]["Options"];
            JArray timedTextFormats = new JArray();
            if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.SpeechAnalyzerTimedTextFormatWebVtt.ToString()])
            {
                timedTextFormats.Add("WebVTT");
            }
            if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.SpeechAnalyzerTimedTextFormatTtml.ToString()])
            {
                timedTextFormats.Add("TTML");
            }
            processorOptions["Formats"] = timedTextFormats;
            processorOptions["Language"] = jobTask.ProcessorConfigString[MediaProcessorConfig.SpeechAnalyzerLanguageId.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.MotionDetection;
            JObject processorConfig = GetProcessorConfig(jobTask);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetContentModerationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.ContentModeration;
            JObject processorConfig = GetProcessorConfig(jobTask);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetCharacterRecognitionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.CharacterRecognition;
            JObject processorConfig = GetProcessorConfig(jobTask);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }
    }
}