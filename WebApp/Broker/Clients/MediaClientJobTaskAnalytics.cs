using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private static MediaJobTask[] GetIndexerTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets,
                                                      ContentProtection contentProtection)
        {
            List<MediaJobTask> indexerTasks = new List<MediaJobTask>();
            string taskName = Constants.Media.Task.IndexerV2;
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
            processorOptions["Formats"] = captionFormats;
            if (jobTask.SpokenLanguages == null || jobTask.SpokenLanguages.Length == 0)
            {
                MediaJobTask indexerTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                indexerTasks.Add(indexerTask);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.SpokenLanguages)
                {
                    processorOptions["Language"] = spokenLanguage;
                    MediaJobTask indexerTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig.ToString(), inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                    indexerTasks.Add(indexerTask);
                }
            }
            return indexerTasks.ToArray();
        }
    }
}
