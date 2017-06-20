namespace AzureSkyMedia.PlatformServices
{
    public struct MediaStream
    {
        public string Name { get; set; }

        public string SourceUrl { get; set; }

        public string InsightsUrl { get; set; }

        public string[] ProtectionTypes { get; set; }

        public MediaTextTrack[] TextTracks { get; set; }

        public MediaMetadata[] AnalyticsMetadata { get; set; }
    }
}
