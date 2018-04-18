using Newtonsoft.Json;

using Microsoft.Azure.Documents;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaPublish
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string[] TaskIds { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public string MobileNumber { get; set; }
    }

    internal class MediaPublishError
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public DocumentClientException Exception { get; set; }
    }

    public class MediaPublished
    {
        public string AssetId { get; set; }

        public string IndexId { get; set; }

        public string MobileNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}