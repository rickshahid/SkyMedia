using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Priority Priority { get; set; }

        public JObject Data { get; set; }

        public string InputAssetName { get; set; }

        public string InputFileUrl { get; set; }

        public bool OutputEncoderIndexer { get; set; }

        public bool OutputAssetSeparation { get; set; }

        public string[] OutputAssetDescriptions { get; set; }

        public string[] OutputAssetAlternateIds { get; set; }
    }
}