using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaWorkflowManifest
    {
        public MediaAccount[] MediaAccounts { get; set; }

        public string MediaStorage { get; set; }

        public MediaTransformPreset[] TransformPresets { get; set; }

        public string JobName { get; set; }

        public Priority JobPriority { get; set; }

        public MediaJobInputMode JobInputMode { get; set; }

        public MediaJobOutputMode JobOutputMode { get; set; }

        public string StreamingPolicyName { get; set; }
    }
}