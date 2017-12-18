namespace AzureSkyMedia.PlatformServices
{
    public struct MediaInsight
    {
        public MediaProcessor Processor { get; set; }

        public string DocumentId { get; set; }

        public string SourceUrl { get; set; }
    }

    public class ContentIndex : StorageEntity
    {
        public string MediaAccountDomainName { get; set; }

        public string MediaAccountEndpointUrl { get; set; }

        public string MediaAccountClientId { get; set; }

        public string MediaAccountClientKey { get; set; }

        public string IndexerAccountKey { get; set; }

        public string LanguageId { get; set; }

        public string SearchPartition { get; set; }

        public string VideoDescription { get; set; }

        public string VideoMetadata { get; set; }

        public bool VideoPublic { get; set; }

        public bool AudioOnly { get; set; }
    }
}