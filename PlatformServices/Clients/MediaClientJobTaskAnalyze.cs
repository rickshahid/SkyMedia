using System.Globalization;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static void IndexVideo(string authToken, IndexerClient indexerClient, IAsset asset, string locatorUrl, MediaJobTask jobTask)
        {
            string spokenLanguage = jobTask.ProcessorConfigString[MediaProcessorConfig.SpokenLanguage.ToString()];
            string searchPartition = jobTask.ProcessorConfigString[MediaProcessorConfig.SearchPartition.ToString()];
            string videoDescription = jobTask.ProcessorConfigString[MediaProcessorConfig.VideoDescription.ToString()];
            string videoMetadata = jobTask.ProcessorConfigString[MediaProcessorConfig.VideoMetadata.ToString()];
            bool videoPublic = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.VideoPublic.ToString()];
            bool audioOnly = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.AudioOnly.ToString()];

            string indexId = indexerClient.UploadVideo(asset, videoDescription, videoMetadata, spokenLanguage, searchPartition, locatorUrl, videoPublic, audioOnly);

            User authUser = new User(authToken);
            MediaInsightPublish insightPublish = new MediaInsightPublish
            {
                PartitionKey = authUser.MediaAccountId,
                RowKey = indexId,
                MediaAccountDomainName = authUser.MediaAccountDomainName,
                MediaAccountEndpointUrl = authUser.MediaAccountEndpointUrl,
                MediaAccountClientId = authUser.MediaAccountClientId,
                MediaAccountClientKey = authUser.MediaAccountClientKey,
                IndexerAccountKey = authUser.VideoIndexerKey,
            };

            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.TableName.InsightPublish;
            tableClient.InsertEntity(tableName, insightPublish);
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.VideoSummarization;
            string settingKey = Constant.AppSettingKey.MediaProcessorVideoSummarizationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["MaxMotionThumbnailDurationInSecs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.SummarizationDurationSeconds.ToString()];
            processorOptions["FadeInFadeOut"] = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.SummarizationFadeTransitions.ToString()];
            processorOptions["OutputAudio"] = !jobTask.ProcessorConfigBoolean[MediaProcessorConfig.VideoOnly.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetVideoAnnotationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.VideoAnnotation;
            string settingKey = Constant.AppSettingKey.MediaProcessorVideoAnnotationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetSpeechAnalyzerTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.SpeechAnalyzer;
            string settingKey = Constant.AppSettingKey.MediaProcessorSpeechAnalyzerDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Features"][0]["Options"];
            JArray captionFormats = new JArray();
            if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.CaptionFormatTtml.ToString()])
            {
                captionFormats.Add("TTML");
            }
            if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.CaptionFormatWebVtt.ToString()])
            {
                captionFormats.Add("WebVTT");
            }
            processorOptions["Formats"] = captionFormats;
            string spokenLanguage = jobTask.ProcessorConfigString[MediaProcessorConfig.SpokenLanguage.ToString()];
            processorOptions["Language"] = spokenLanguage.Replace("-", string.Empty); ;
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
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
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetFaceRedactionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            string mode = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceRedactionMode.ToString()];
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.FaceRedaction;
            jobTask.Name = string.Concat(jobTask.Name, " ", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mode));
            string settingKey = Constant.AppSettingKey.MediaProcessorFaceRedactionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            JToken processorOptions = processorConfig["Options"];
            processorOptions["Mode"] = mode;
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.MotionDetection;
            string settingKey = Constant.AppSettingKey.MediaProcessorMotionDetectionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetMotionHyperlapseTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            int frameStart = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseFrameStart.ToString()];
            int frameEnd = jobTask.ProcessorConfigInteger[MediaProcessorConfig.MotionHyperlapseFrameEnd.ToString()];
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
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
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetCharacterRecognitionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.CharacterRecognition;
            string settingKey = Constant.AppSettingKey.MediaProcessorCharacterRecognitionDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetContentModerationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.ContentModeration;
            string settingKey = Constant.AppSettingKey.MediaProcessorContentModerationDocumentId;
            string documentId = AppSetting.GetValue(settingKey);
            JObject processorConfig = GetProcessorConfig(documentId);
            jobTask.ProcessorConfig = processorConfig.ToString();
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, false);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }
    }
}