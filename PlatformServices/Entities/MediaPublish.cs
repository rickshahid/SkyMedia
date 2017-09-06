namespace AzureSkyMedia.PlatformServices
{
    internal class MediaContentPublish : StorageEntity
    {
        public string MediaAccountDomainName { get; set; }

        public string MediaAccountEndpointUrl { get; set; }

        public string MediaAccountClientId { get; set; }

        public string MediaAccountClientKey { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }
    }

    internal class MediaInsightsPublish : StorageEntity
    {
        public string MediaAccountDomainName { get; set; }

        public string MediaAccountEndpointUrl { get; set; }

        public string MediaAccountClientId { get; set; }

        public string MediaAccountClientKey { get; set; }

        public string IndexerAccountKey { get; set; }
    }

    public class MediaPublish
    {
        public string AssetId { get; set; }

        public string IndexId { get; set; }

        public string DocumentId { get; set; }

        public string UserId { get; set; }

        public string MobileNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}