using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaTransformOutput
    {
        public bool PresetEnabled { get; set; }

        public string PresetName { get; set; }

        public string RelativePriority { get; set; }

        public string OnErrorMode { get; set; }
    }
}