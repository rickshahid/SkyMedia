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
                spokenLanguages.Add("English (US)", "enUS");
                spokenLanguages.Add("English (British)", "enGB");
                spokenLanguages.Add("Spanish", "esES");
                spokenLanguages.Add("Spanish (Mexican)", "esMX");
            }
            spokenLanguages.Add("Arabic (Egyptian)", videoIndexer ? "ar-EG" : "arEG");
            spokenLanguages.Add("Chinese (Mandarin)", videoIndexer ? "zh-CN" : "zhCN");
            spokenLanguages.Add("French", videoIndexer ? "fr-FR" : "frFR");
            spokenLanguages.Add("German", videoIndexer ? "de-DE" : "deDE");
            spokenLanguages.Add("Italian", videoIndexer ? "it-IT" : "itIT");
            spokenLanguages.Add("Japanese", videoIndexer ? "ja-JP" : "jaJP");
            spokenLanguages.Add("Portuguese", videoIndexer ? "pt-BR" : "ptBR");
            spokenLanguages.Add("Russian", videoIndexer ? "ru-RU" : "ruRU");
            return spokenLanguages;
        }

        public static string GetLanguageId(string webVttUrl)
        {
            string languageId = Path.GetFileNameWithoutExtension(webVttUrl);
            return languageId.Insert(2, "-");
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