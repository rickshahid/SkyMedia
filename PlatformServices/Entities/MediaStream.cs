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

        public StreamProtection[] ProtectionInfo { get; set; }
    }

    public class MediaStream
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string[] Thumbnails { get; set; }

        public StreamSource Source { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        public MediaInsight[] ContentInsight { get; set; }
    }
}