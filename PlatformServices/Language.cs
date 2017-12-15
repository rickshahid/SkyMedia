using System;
using System.IO;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Language
    {
        public static NameValueCollection GetSpokenLanguages(bool videoIndexer)
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

        public static string GetLanguageId(JObject processorConfig)
        {
            string languageId = string.Empty;
            if (processorConfig["Features"] != null && processorConfig["Features"].HasValues)
            {
                JToken processorOptions = processorConfig["Features"][0]["Options"];
                if (processorOptions != null && processorOptions["Language"] != null)
                {
                    languageId = processorOptions["Language"].ToString();
                }
            }
            return languageId;
        }

        public static string GetLanguageLabel(string languageId, bool videoIndexer)
        {
            string languageLabel = string.Empty;
            NameValueCollection languages = GetSpokenLanguages(videoIndexer);
            foreach (string language in languages.Keys)
            {
                if (string.Equals(languages[language], languageId, StringComparison.OrdinalIgnoreCase))
                {
                    languageLabel = language;
                }
            }
            return languageLabel;
        }

        public static string GetLanguageLabel(string webVttUrl)
        {
            string languageLabel = string.Empty;
            if (!string.IsNullOrEmpty(webVttUrl))
            {
                string languageId = Path.GetFileNameWithoutExtension(webVttUrl);
                languageId = languageId.Substring(languageId.Length - 4);
                languageLabel = GetLanguageLabel(languageId, false);
            }
            return languageLabel;
        }
    }
}