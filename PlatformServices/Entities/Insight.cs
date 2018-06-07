using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaInsightSource
    {
        public MediaProcessor MediaProcessor { get; set; }

        public string OutputId { get; set; }

        public string OutputUrl { get; set; }
    }

    public class MediaInsight
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public MediaInsightSource[] Sources { get; set; }
    }

//    public class MediaInsightConfig
//    {
//        public MediaInsightConfig()
//        {
//            LanguageId = string.Empty;
//            LinguisticModelId = string.Empty;
//            SearchPartition = string.Empty;
//            VideoDescription = string.Empty;
//            VideoMetadata = string.Empty;
//            VideoPublic = false;
//            AudioOnly = false;
//        }

//        public string LanguageId { get; set; }

//        public string LinguisticModelId { get; set; }

//        public string SearchPartition { get; set; }

//        public string VideoDescription { get; set; }

//        public string VideoMetadata { get; set; }

//        public bool VideoPublic { get; set; }

//        public bool AudioOnly { get; set; }
//    }
}