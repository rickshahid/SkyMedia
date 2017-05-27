namespace AzureSkyMedia.PlatformServices
{
    public class MediaJobPublish : StorageEntity
    {
        public string MediaAccountKey { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }

        public string UserId { get; set; }

        public string MobileNumber { get; set; }
    }

    public class MediaJobPublication
    {
        public string UserId { get; set; }

        public string MobileNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
