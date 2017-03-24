using Newtonsoft.Json;
    
namespace AzureSkyMedia.PlatformServices
{
    public struct MediaTextTrack
    {
        [JsonProperty(PropertyName = "kind")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "src")]
        public string SourceUrl { get; set; }

        [JsonProperty(PropertyName = "srclang")]
        public string LanguageCode { get; set; }
    }
}
