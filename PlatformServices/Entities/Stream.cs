using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class StreamProtection
    {
        public MediaProtection Type { get; set; }

        public string AuthenticationToken { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertificateUrl { get; set; }
    }

    public class StreamSource
    {
        [JsonProperty(PropertyName = "src")]
        public string Url { get; set; }

        public StreamProtection[] ProtectionInfo { get; set; }
    }

    public class MediaStream
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public string[] ThumbnailUrls { get; set; }

        public StreamSource Source { get; set; }

        public TextTrack[] TextTracks { get; set; }

        public MediaInsight ContentInsight { get; set; }
    }
}