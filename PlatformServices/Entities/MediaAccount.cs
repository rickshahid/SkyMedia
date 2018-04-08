namespace AzureSkyMedia.PlatformServices
{
    public struct MediaAccount
    {
        public string Id
        {
            get
            {
                string accountId = this.EndpointUrl;
                accountId = accountId.Split('/')[2];
                return accountId.Split('.')[0];
            }
        }

        public string DomainName { get; set; }

        public string EndpointUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientKey { get; set; }

        public string IndexerKey { get; set; }
    }
}