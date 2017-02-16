using System.Xml;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static MediaJobTask[] GetIndexerV1Tasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.IndexerV1;
            string settingKey = Constants.AppSettingKey.MediaProcessorIndexerV1DocumentId;
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
                MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
                jobTasks.AddRange(mappedJobTasks);
            }
            else
            {
                foreach (string spokenLanguage in jobTask.IndexerSpokenLanguages)
                {
                    configSettings[0].Attributes[1].Value = spokenLanguage;
                    jobTask.ProcessorConfig = processorConfigXml.OuterXml;
                    MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, false);
                    jobTasks.AddRange(mappedJobTasks);
                }
            }
            return jobTasks.ToArray();
        }

        private static MediaJobTask[] GetIndexerV2Tasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            jobTask.MediaProcessor = MediaProcessor.IndexerV2;
            string settingKey = Constants.AppSettingKey.MediaProcessorIndexerV2DocumentId;
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
    }
}
