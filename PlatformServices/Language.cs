using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public static class Language
    {
        public static string GetLanguageCode(string sourceUrl)
        {
            string[] sourceInfo = sourceUrl.Split('.');
            string fileName = sourceInfo[sourceInfo.Length - 2];
            return fileName.Substring(fileName.Length - 2);
        }

        public static string GetLanguageCode(JObject processorConfig)
        {
            string languageCode = string.Empty;
            if (processorConfig["Features"] != null && processorConfig["Features"].HasValues)
            {
                JToken processorOptions = processorConfig["Features"][0]["Options"];
                if (processorOptions != null && processorOptions["Language"] != null)
                {
                    languageCode = processorOptions["Language"].ToString();
                }
            }
            return languageCode;
        }

        public static JObject GetSpokenLanguages(bool videoIndexer)
        {
            JObject spokenLanguages = new JObject();
            if (videoIndexer)
            {
                spokenLanguages.Add("English", "English");
                spokenLanguages.Add("Spanish", "Spanish");
                spokenLanguages.Add("French", "French");
                spokenLanguages.Add("German", "German");
                spokenLanguages.Add("Italian", "Italian");
                spokenLanguages.Add("Chinese", "Chinese (Simplified)");
                spokenLanguages.Add("Portuguese", "Portuguese (Brazilian)");
                spokenLanguages.Add("Japanese", "Japanese");
                spokenLanguages.Add("Russian", "Russian");
            }
            else
            {
                spokenLanguages.Add("EnUs", "English");
                spokenLanguages.Add("EnGb", "English (British)");
                spokenLanguages.Add("EsEs", "Spanish");
                spokenLanguages.Add("EsMx", "Spanish (Mexican)");
                spokenLanguages.Add("ArEg", "Arabic (Egyptian)");
                spokenLanguages.Add("ZhCn", "Chinese (Mandarin)");
                spokenLanguages.Add("FrFr", "French");
                spokenLanguages.Add("DeDe", "German");
                spokenLanguages.Add("ItIt", "Italian");
                spokenLanguages.Add("JaJp", "Japanese");
                spokenLanguages.Add("PtBr", "Portuguese");
                spokenLanguages.Add("RuRu", "Russian");
            }
            return spokenLanguages;
        }

        public static string GetLanguageLabel(string languageCode)
        {
            string languageLabel = string.Empty;
            JObject spokenLanguages = GetSpokenLanguages(false);
            languageCode = languageCode.Substring(0, 2);
            IEnumerable<JProperty> properties = spokenLanguages.Properties();
            foreach (JProperty property in properties)
            {
                string propertyCode = property.Name.Substring(0, 2);
                if (string.Equals(propertyCode, languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    languageLabel = property.Value.ToString();
                }
            }
            return languageLabel;
        }
    }
}
