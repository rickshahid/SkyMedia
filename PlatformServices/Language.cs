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
            string spokenLanguage = string.Empty;
            if (processorConfig["Features"] != null && processorConfig["Features"].HasValues)
            {
                JToken processorOptions = processorConfig["Features"][0]["Options"];
                if (processorOptions != null && processorOptions["Language"] != null)
                {
                    spokenLanguage = processorOptions["Language"].ToString().Substring(0, 2).ToLower();
                }
            }
            return spokenLanguage;
        }

        public static JObject GetSpokenLanguages()
        {
            JObject spokenLanguages = new JObject();
            spokenLanguages.Add("en", "English");
            spokenLanguages.Add("es", "Spanish");
            spokenLanguages.Add("ar", "Arabic");
            spokenLanguages.Add("zh", "Chinese");
            spokenLanguages.Add("fr", "French");
            spokenLanguages.Add("de", "German");
            spokenLanguages.Add("it", "Italian");
            spokenLanguages.Add("jp", "Japanese");
            spokenLanguages.Add("pt", "Portuguese");
            return spokenLanguages;
        }

        public static string GetLanguageLabel(string languageCode)
        {
            JObject spokenLanguages = GetSpokenLanguages();
            return spokenLanguages[languageCode].ToString();
        }
    }
}
