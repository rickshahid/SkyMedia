using System;
using System.IO;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Language
    {
        public static NameValueCollection GetLanguages(bool videoIndexer)
        {
            NameValueCollection languages = new NameValueCollection();
            if (videoIndexer)
            {
                languages.Add("English", "en-us");
                languages.Add("Spanish", "es-es");
            }
            else
            {
                languages.Add("English (US)", "enUS");
                languages.Add("English (British)", "enGB");
                languages.Add("Spanish", "esES");
                languages.Add("Spanish (Mexican)", "esMX");
            }
            languages.Add("Arabic (Egyptian)", videoIndexer ? "ar-eg" : "arEG");
            languages.Add("Chinese (Simplified)", videoIndexer ? "zh-hans" : "zhCN");
            languages.Add("French", videoIndexer ? "fr-fr" : "frFR");
            languages.Add("German", videoIndexer ? "de-de" : "deDE");
            languages.Add("Italian", videoIndexer ? "it-it" : "itIT");
            languages.Add("Japanese", videoIndexer ? "ja-jp" : "jaJP");
            languages.Add("Portuguese", videoIndexer ? "pt-br" : "ptBR");
            languages.Add("Russian", videoIndexer ? "ru-ru" : "ruRU");
            return languages;
        }

        public static string GetLanguageId(string taskConfig)
        {
            JObject processorConfig = JObject.Parse(taskConfig);
            return processorConfig["Features"][0]["Options"]["Language"].ToString();
        }

        public static string GetLanguageLabel(string webVttUrl)
        {
            string languageId = Path.GetFileNameWithoutExtension(webVttUrl);
            languageId = languageId.Substring(languageId.Length - 4);
            return GetLanguageLabel(languageId, false);
        }

        public static string GetLanguageLabel(string languageId, bool videoIndexer)
        {
            string languageLabel = string.Empty;
            NameValueCollection languages = GetLanguages(videoIndexer);
            foreach (string language in languages.Keys)
            {
                if (string.Equals(languages[language], languageId, StringComparison.OrdinalIgnoreCase))
                {
                    languageLabel = language;
                }
            }
            return languageLabel;
        }
    }
}