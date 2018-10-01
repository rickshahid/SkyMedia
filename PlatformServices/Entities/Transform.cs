using Microsoft.Azure.Management.Media.Models;

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

        public Priority RelativePriority { get; set; }

        public OnErrorType OnError { get; set; }
    }
}