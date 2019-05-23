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

        public MediaJobOutputInsight OutputInsight { get; set; }

        public MediaJobOutputPublish OutputPublish { get; set; }
    }

    public class MediaJobOutputInsight
    {
        public string Id { get; set; }

        public bool VideoIndexer { get; set; }

        public bool AudioIndexer { get; set; }
    }

    public class MediaJobOutputPublish
    {
        public StandardBlobTier InputAssetStorageTier { get; set; }

        public string StreamingPolicyName { get; set; }

        public ContentProtection ContentProtection { get; set; }
    }
}