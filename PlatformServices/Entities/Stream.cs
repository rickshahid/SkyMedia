namespace AzureSkyMedia.PlatformServices
{
    public class MediaStream
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Poster { get; set; }

        public MediaTrack[] Tracks { get; set; }

        public MediaInsight Insight { get; set; }

        public MediaProtection[] Protection { get; set; }
    }
}