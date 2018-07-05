namespace AzureSkyMedia.PlatformServices
{
    public class MediaTransformOutput
    {
        public bool PresetEnabled { get; set; }

        public string PresetName { get; set; }

        public string RelativePriority { get; set; }

        public string OnError { get; set; }
    }
}