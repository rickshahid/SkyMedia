using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static string GetDocumentId(MediaProcessor mediaProcessor)
        {
            string processorName = Processor.GetProcessorName(mediaProcessor);
            return string.Concat(processorName, Constant.Database.Document.DefaultIdSuffix);
        }

        private static MediaJobTask[] GetVideoAnnotationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.VideoAnnotation;
            string documentId = GetDocumentId(jobTask.MediaProcessor);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.VideoSummarization;
            string documentId = GetDocumentId(jobTask.MediaProcessor);
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
            string documentId = GetDocumentId(jobTask.MediaProcessor);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetContentModerationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.ContentModeration;
            string documentId = GetDocumentId(jobTask.MediaProcessor);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetSpeechAnalyzerTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.SpeechAnalyzer;
            string documentId = GetDocumentId(jobTask.MediaProcessor);
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
            string documentId = GetDocumentId(jobTask.MediaProcessor);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Mode"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceDetectionMode.ToString()];
            processorOptions["AggregateEmotionWindowMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceDetectionAggregateEmotionWindow.ToString()];
            processorOptions["AggregateEmotionIntervalMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceDetectionAggregateEmotionInterval.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetFaceRedactionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.FaceRedaction;
            string documentId = GetDocumentId(jobTask.MediaProcessor);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Mode"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceRedactionMode.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.MotionDetection;
            string documentId = GetDocumentId(jobTask.MediaProcessor);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetMotionHyperlapseTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            jobTask.MediaProcessor = MediaProcessor.MotionHyperlapse;
            string documentId = GetDocumentId(jobTask.MediaProcessor);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorSources = processorConfig["Sources"][0];
            int frameStart = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseFrameStart.ToString()];
            int frameEnd = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseFrameEnd.ToString()];
            processorSources["StartFrame"] = frameStart;
            processorSources["NumFrames"] = frameEnd - frameStart + 1;
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Speed"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseSpeed.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }
    }
}