namespace AzureSkyMedia.PlatformServices
{
    public struct MediaAccount
    {
        public string Id { get; set; }

        public string DomainName { get; set; }

        public string EndpointUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientKey { get; set; }

        public string IndexerKey { get; set; }
    }
}