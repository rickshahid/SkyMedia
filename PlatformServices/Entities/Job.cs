using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Priority Priority { get; set; }

        public string InputAssetName { get; set; }

        public string InputFileUrl { get; set; }

        public string OutputAssetDescription { get; set; }

        public string OutputAssetAlternateId { get; set; }
    }
}