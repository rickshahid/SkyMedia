using System;

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

        public string InputFileUrl { get; set; }

        public string InputAssetName { get; set; }

        public MediaJobOutputMode OutputAssetMode { get; set; }

        public string[] OutputAssetAlternateIds { get; set; }

        public string[] OutputAssetDescriptions { get; set; }
    }

    internal class MediaJobAccount
    {
        public MediaJobAccount()
        {
            this.Created = DateTime.UtcNow;
        }

        public string InsightId { get; set; }

        public string JobName { get; set; }

        public string TransformName { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public DateTime Created { get; }
    }
}