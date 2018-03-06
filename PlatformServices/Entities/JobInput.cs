namespace AzureSkyMedia.PlatformServices
{
    public class MediaJobInput
    {
        public string AssetId { get; set; }

        public string AssetName { get; set; }

        public bool AssetFilter { get; set; }

        public string PrimaryFile { get; set; }
    }
}