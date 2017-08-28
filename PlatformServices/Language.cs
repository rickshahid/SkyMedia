using System;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Language
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

        public static NameValueCollection GetSpokenLanguages(bool videoIndexer)
        {
            NameValueCollection spokenLanguages = new NameValueCollection();
            if (videoIndexer)
            {
                spokenLanguages.Add("English", "English");
                spokenLanguages.Add("Spanish", "Spanish");
                spokenLanguages.Add("French", "French");
                spokenLanguages.Add("German", "German");
                spokenLanguages.Add("Italian", "Italian");
                spokenLanguages.Add("Chinese", "Chinese");
                spokenLanguages.Add("Portuguese", "Portuguese");
                spokenLanguages.Add("Japanese", "Japanese");
                spokenLanguages.Add("Russian", "Russian");
            }
            else
            {
                spokenLanguages.Add("English", "EnUs");
                spokenLanguages.Add("English (British)", "EnGb");
                spokenLanguages.Add("Spanish", "EsEs");
                spokenLanguages.Add("Spanish (Mexican)", "EsMx");
                spokenLanguages.Add("Arabic (Egyptian)", "ArEg");
                spokenLanguages.Add("Chinese (Mandarin)", "ZhCn");
                spokenLanguages.Add("French", "FrFr");
                spokenLanguages.Add("German", "DeDe");
                spokenLanguages.Add("Italian", "ItIt");
                spokenLanguages.Add("Japanese", "JaJp");
                spokenLanguages.Add("Portuguese", "PtBr");
                spokenLanguages.Add("Russian", "RuRu");
            }
            return spokenLanguages;
        }

        public static string GetLanguageLabel(string languageCode)
        {
            string languageLabel = string.Empty;
            NameValueCollection languages = GetSpokenLanguages(false);
            languageCode = languageCode.Substring(0, 2);
            foreach (string language in languages.Keys)
            {
                if (string.Equals(language.Substring(0, 2), languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    languageLabel = languages[language];
                }
            }
            return languageLabel;
        }
    }
}