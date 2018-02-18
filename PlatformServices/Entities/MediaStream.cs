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
        public string Src { get; set; }

        public StreamProtection[] ProtectionInfo { get; set; }
    }

    public class MediaStream
    {
        public MediaStream DeepCopy()
        {
            MediaStream mediaStream = (MediaStream)this.MemberwiseClone();
            mediaStream.Source = new StreamSource()
            {
                Src = this.Source.Src,
                ProtectionInfo = this.Source.ProtectionInfo == null ? null : (StreamProtection[])this.Source.ProtectionInfo.Clone(),
            };
            mediaStream.Thumbnails = this.Thumbnails == null ? null : (string[])this.Thumbnails.Clone();
            mediaStream.TextTracks = this.TextTracks == null ? null : (MediaTrack[])this.TextTracks.Clone();
            mediaStream.ContentInsight = this.ContentInsight == null ? null : (MediaInsight[])this.ContentInsight.Clone();
            return mediaStream;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public StreamSource Source { get; set; }

        public string[] Thumbnails { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        public MediaInsight[] ContentInsight { get; set; }
    }
}