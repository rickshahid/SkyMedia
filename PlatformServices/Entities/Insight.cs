using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaInsight
    {
        public string WidgetUrl { get; set; }

        public string ViewToken { get; set; }
    }

    public class MediaInsightLink
    {
        [JsonProperty(PropertyName = "id")]
        public string InsightId { get; set; }

        public string StreamingAssetName { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public UserAccount UserAccount { get; set; }
    }
}