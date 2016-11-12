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
            List<MediaJobTask> indexerTasks = new List<MediaJobTask>();
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
                MediaJobTask indexerTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfigXml.OuterXml, inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                indexerTasks.Add(indexerTask);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.SpokenLanguages)
                {
                    configSettings[0].Attributes[1].Value = spokenLanguage;
                    MediaJobTask indexerTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfigXml.OuterXml, inputAssets, jobTask.OutputAssetName, contentProtection, jobTask.Options);
                    indexerTasks.Add(indexerTask);
                }
            }
            return indexerTasks.ToArray();
        }

        private static MediaJobTask[] GetIndexerV2Tasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, ContentProtection contentProtection)
        {
            List<MediaJobTask> indexerTasks = new List<MediaJobTask>();
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
