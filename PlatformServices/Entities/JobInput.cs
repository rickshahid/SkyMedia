namespace AzureSkyMedia.PlatformServices
{
    public class MediaJobInput
    {
        public string AssetId { get; set; }

        public string AssetName { get; set; }

        public string PrimaryFile { get; set; }

        public bool WorkflowView { get; set; }

        public int MarkInSeconds { get; set; }

        public string MarkInTime { get; set; }

        public int MarkOutSeconds { get; set; }

        public string MarkOutTime { get; set; }
    }
}