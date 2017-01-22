namespace AzureSkyMedia.Services
{
    public class ContentProtection : StorageEntity
    {
        public bool AES { get; set; }

        public bool DRMPlayReady { get; set; }

        public bool DRMWidevine { get; set; }

        public bool ContentAuthTypeToken { get; set; }

        public bool ContentAuthTypeAddress { get; set; }

        public string ContentAuthAddressRange { get; set; }
    }
}
