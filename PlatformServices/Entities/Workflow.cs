using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaWorkflowManifest
    {
        public MediaAccount[] MediaAccounts { get; set; }

        public string InputFileName { get; set; }

        public string OutputAssetStorage { get; set; }

        public MediaTransformPreset[] TransformPresets { get; set; }

        public JObject EncodingProfile { get; set; }

        public string JobName { get; set; }

        public Priority JobPriority { get; set; }

        public MediaJobInputMode JobInputMode { get; set; }

        public MediaJobOutputMode JobOutputMode { get; set; }

        public string StreamingPolicyName { get; set; }
    }
}