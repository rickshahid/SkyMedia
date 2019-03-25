using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaTransformOutput
    {
        public MediaTransformPreset PresetType { get; set; }

        public string PresetName { get; set; }

        public Priority RelativePriority { get; set; }

        public OnErrorType OnError { get; set; }
    }
}