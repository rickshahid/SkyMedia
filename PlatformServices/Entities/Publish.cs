using System;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaPublish
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string[] TaskIds { get; set; }

        public MediaInsightConfig InsightConfig { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public string MobileNumber { get; set; }
    }

    internal class MediaPublishError
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public Exception Exception { get; set; }
    }

    public class MediaPublished
    {
        public string IndexId { get; set; }

        public string MobileNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}