namespace AzureSkyMedia.PlatformServices
{
    public class MediaTransformPresets
    {
        public bool AdaptiveStreaming { get; set; }

        public bool ThumbnailSprite { get; set; }

        public bool VideoAnalyzer { get; set; }

        public bool AudioAnalyzer { get; set; }

        public bool VideoIndexer { get; set; }

        public bool AudioIndexer { get; set; }
    }

    public class MediaTransformOutput
    {
        public MediaTransformPreset TransformPreset { get; set; }

        public string RelativePriority { get; set; }

        public string OnError { get; set; }
    }
}