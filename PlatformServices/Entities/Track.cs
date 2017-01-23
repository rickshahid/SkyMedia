using Newtonsoft.Json;
    
namespace AzureSkyMedia.PlatformServices
{
    public struct MediaTrack
    {
        [JsonProperty(PropertyName = "kind")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "src")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "srclang")]
        public string Language { get; set; }
    }
}
