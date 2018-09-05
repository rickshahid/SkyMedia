using System.Collections.Generic;

using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaIngestManifest
    {
        [JsonProperty(PropertyName = "id")]
        public string Name { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public string StorageAccount
        {
            get
            {
                Dictionary<string, string>.Enumerator storageAccounts = MediaAccount.StorageAccounts.GetEnumerator();
                storageAccounts.MoveNext();
                return storageAccounts.Current.Key;
            }
        }

        public string[] MissingFiles { get; set; }

        public string[] BlobFiles { get; set; }

        public string AssetName { get; set; }

        public string AssetDescription { get; set; }

        public string AssetAlternateId { get; set; }

        public MediaProcessor[] MediaProcessors { get; set; }

        public string JobName { get; set; }

        public string JobDescription { get; set; }

        public Priority JobPriority { get; set; }

        public JObject JobData { get; set; }

        public string JobInputFileUrl { get; set; }

        public string JobOutputAssetDescription { get; set; }

        public string StreamingPolicyName { get; set; }
    }
}