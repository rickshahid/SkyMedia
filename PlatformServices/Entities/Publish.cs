using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaPublish
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string TransformName { get; set; }

        public string StreamingPolicyName { get; set; }

        public ContentProtection ContentProtection { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public UserAccount UserAccount { get; set; }
    }
}