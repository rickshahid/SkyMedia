using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Priority Priority { get; set; }

        public string InputFileUrl { get; set; }

        public string InputAssetName { get; set; }

        public string OutputAssetStorage { get; set; }

        public MediaJobOutputPublish OutputAssetPublish { get; set; }
    }

    public class MediaJobOutputPublish
    {
        public StandardBlobTier InputAssetStorageTier { get; set; }

        public string StreamingPolicyName { get; set; }

        public ContentProtection ContentProtection { get; set; }
    }
}