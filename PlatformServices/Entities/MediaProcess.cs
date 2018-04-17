using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaProcess
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string[] MissingFiles { get; set; }
    }
}