namespace AzureSkyMedia.PlatformServices
{
    public class MediaTransformOutput
    {
        public MediaProcessor PresetProcessor { get; set; }

        public string PresetName { get; set; }

        public string RelativePriority { get; set; }

        public string OnError { get; set; }
    }
}