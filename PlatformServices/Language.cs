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

        public static JObject GetSpokenLanguages()
        {
            JObject spokenLanguages = new JObject();
            spokenLanguages.Add("EnUS", "English");
            spokenLanguages.Add("EsEs", "Spanish");
            spokenLanguages.Add("ArEg", "Arabic");
            spokenLanguages.Add("ZhCn", "Chinese");
            spokenLanguages.Add("FrFr", "French");
            spokenLanguages.Add("DeDe", "German");
            spokenLanguages.Add("ItIt", "Italian");
            spokenLanguages.Add("JaJp", "Japanese");
            spokenLanguages.Add("PtBr", "Portuguese");
            return spokenLanguages;
        }

        public static string GetLanguageLabel(string languageCode)
        {
            string languageLabel = string.Empty;
            JObject spokenLanguages = GetSpokenLanguages();
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
