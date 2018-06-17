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
            MediaStream mediaStream = (MediaStream)this.MemberwiseClone();
            mediaStream.Source = new StreamSource()
            {
                Url = this.Source.Url,
                ProtectionInfo = this.Source.ProtectionInfo == null ? null : (StreamProtection[])this.Source.ProtectionInfo.Clone(),
            };
            mediaStream.ThumbnailUrls = this.ThumbnailUrls == null ? null : (string[])this.ThumbnailUrls.Clone();
            mediaStream.TextTracks = this.TextTracks == null ? null : (MediaTrack[])this.TextTracks.Clone();
            //mediaStream.ContentInsight = this.ContentInsight;
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