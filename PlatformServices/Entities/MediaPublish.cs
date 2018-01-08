namespace AzureSkyMedia.PlatformServices
{
    internal class MediaPublish : StorageEntity
    {
        public MediaAccount MediaAccount { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }
    }

    public class MediaPublished
    {
        public string AssetId { get; set; }

        public string IndexId { get; set; }

        public string DocumentId { get; set; }

        public string UserId { get; set; }

        public string MobileNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}