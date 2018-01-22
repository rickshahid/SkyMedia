using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class StreamProtection
    {
        public MediaProtection Type { get; set; }

        public string CertificateUrl { get; set; }

        public string AuthenticationToken { get; set; }
    }

    public class StreamSource
    {
        public string Src { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StreamProtection[] ProtectionInfo { get; set; }
    }

    public class MediaStream
    {
        public MediaStream DeepCopy()
        {
            MediaStream mediaStream = new MediaStream()
            {
                Id = this.Id,
                Name = this.Name,
                Type = this.Type,
                Thumbnails = this.Thumbnails == null ? null : (string[])this.Thumbnails.Clone(),
                Source = new StreamSource()
                {
                    Src = this.Source.Src,
                    ProtectionInfo = this.Source.ProtectionInfo == null ? null : (StreamProtection[])this.Source.ProtectionInfo.Clone(),
                },
                TextTracks = this.TextTracks == null ? null : (MediaTrack[])this.TextTracks.Clone(),
                ContentInsight = this.ContentInsight == null ? null : (MediaInsight[])this.ContentInsight.Clone()
            };
            return mediaStream;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string[] Thumbnails { get; set; }

        public StreamSource Source { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        public MediaInsight[] ContentInsight { get; set; }
    }
}