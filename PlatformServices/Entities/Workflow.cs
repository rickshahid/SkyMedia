using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaWorkflowManifest
    {
        public UserAccount UserAccount { get; set; }

        public MediaAccount[] MediaAccounts { get; set; }

        public string AssetName { get; set; }

        public string InputFileName { get; set; }

        public string OutputAssetStorage { get; set; }

        public MediaTransformPreset[] TransformPresets { get; set; }

        public string JobName { get; set; }

        public Priority JobPriority { get; set; }

        public MediaJobInputMode JobInputMode { get; set; }

        public MediaJobOutputPublish JobOutputPublish { get; set; }
    }

    public class MediaWorkflowEntity
    {
        public MediaEntity Type { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string InsightId { get; set; }
    }
}