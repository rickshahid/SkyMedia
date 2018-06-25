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
        public MediaStream DeepCopy()
        {
            MediaStream mediaStream = (MediaStream)MemberwiseClone();
            mediaStream.Source = new StreamSource()
            {
                Url = Source.Url,
                ProtectionInfo = Source.ProtectionInfo == null ? null : (StreamProtection[])Source.ProtectionInfo.Clone(),
            };
            mediaStream.ThumbnailUrls = ThumbnailUrls == null ? null : (string[])ThumbnailUrls.Clone();
            mediaStream.TextTracks = TextTracks == null ? null : (MediaTrack[])TextTracks.Clone();
            //mediaStream.ContentInsight = ContentInsight;
            return mediaStream;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public StreamSource Source { get; set; }

        [JsonProperty(PropertyName = "thumbnails")]
        public string[] ThumbnailUrls { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        //public MediaInsight ContentInsight { get; set; }
    }
}