namespace AzureSkyMedia.PlatformServices
{
    public class MediaIndexPublish : StorageEntity
    {
        public string IndexerAccountKey { get; set; }

        public string MediaAccountName { get; set; }

        public string MediaAccountKey { get; set; }
    }
}
