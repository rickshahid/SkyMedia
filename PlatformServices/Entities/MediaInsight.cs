namespace AzureSkyMedia.PlatformServices
{
    public struct MediaInsight
    {
        public MediaProcessor Processor { get; set; }

        public string SourceUrl { get; set; }

        public string DocumentId { get; set; }
    }
}