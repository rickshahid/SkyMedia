namespace AzureSkyMedia.Services
{
    internal class JobPublish : StorageEntity
    {
        public string MediaAccountKey { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }

        public string MobileNumber { get; set; }
    }
}
