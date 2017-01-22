namespace AzureSkyMedia.Services
{
    public class MediaAssetInput
    {
        public string AssetId { get; set; }

        public string AssetName { get; set; }

        public string PrimaryFile { get; set; }

        public string MarkIn { get; set; }

        public string MarkOut { get; set; }

        public string ClipDuration { get; set; }
    }
}
