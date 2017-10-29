using System;
using System.IO;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Language
    {
        private static string GetLanguageLabel(string languageCode, bool videoIndexer)
        {
            string languageLabel = string.Empty;
            NameValueCollection languages = GetSpokenLanguages(videoIndexer);
            foreach (string language in languages.Keys)
            {
                if (string.Equals(languages[language], languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    languageLabel = language;
                }
            }
            return languageLabel;
        }

        internal static string GetLanguageCode(string sourceUrl)
        {
            string[] sourceInfo = sourceUrl.Split('.');
            string fileName = sourceInfo[sourceInfo.Length - 2];
            return fileName.Substring(fileName.Length - 2);
        }

        internal static string GetLanguageCode(JObject processorConfig)
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

        internal static NameValueCollection GetSpokenLanguages(bool videoIndexer)
        {
            NameValueCollection spokenLanguages = new NameValueCollection();
            if (videoIndexer)
            {
                spokenLanguages.Add("English", "en-US");
                spokenLanguages.Add("Spanish", "es-ES");
            }
            else
            {
                spokenLanguages.Add("English (US)", "en-US");
                spokenLanguages.Add("English (British)", "en-GB");
                spokenLanguages.Add("Spanish", "es-ES");
                spokenLanguages.Add("Spanish (Mexican)", "es-MX");
            }
            spokenLanguages.Add("Arabic (Egyptian)", "ar-EG");
            spokenLanguages.Add("Chinese (Mandarin)", "zh-CN");
            spokenLanguages.Add("French", "fr-FR");
            spokenLanguages.Add("German", "de-DE");
            spokenLanguages.Add("Italian", "it-IT");
            spokenLanguages.Add("Japanese", "ja-JP");
            spokenLanguages.Add("Portuguese", "pt-BR");
            spokenLanguages.Add("Russian", "ru-RU");
            return spokenLanguages;
        }

        internal static string GetLanguageLabel(string webVttUrl)
        {
            string languageCode = Path.GetFileNameWithoutExtension(webVttUrl);
            return GetLanguageLabel(languageCode, false);
        }

        internal static string GetLanguageLabel(JObject index)
        {
            string languageCode = index["breakdowns"][0]["language"].ToString();
            return GetLanguageLabel(languageCode, false);
        }
    }
}