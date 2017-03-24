namespace AzureSkyMedia.PlatformServices
{
    public struct MediaStream
    {
        public string Name { get; set; }

        public string SourceUrl { get; set; }

        public MediaTextTrack[] TextTracks { get; set; }

        public MediaProtection[] ProtectionTypes { get; set; }

        public MediaMetadata[] AnalyticsMetadata { get; set; }
    }
}
