using System.Collections.Generic;

using Newtonsoft.Json;

using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Priority Priority { get; set; }

        public string InputAssetName { get; set; }

        public string[] OutputAssetNames { get; set; }

        public IDictionary<string, string> CorrelationData { get; set; }
    }

    //public class MediaJobInput
    //{
    //    public string AssetId { get; set; }

    //    public string AssetName { get; set; }

    //    public string AssetType { get; set; }

    //    public string PrimaryFile { get; set; }
    //}

    //public class MediaJob
    //{
    //    public string Name { get; set; }

    //    public int Priority { get; set; }

    //    //public MediaJobTask[] Tasks { get; set; }
    //}

    //internal class MediaProcess
    //{
    //    [JsonProperty(PropertyName = "id")]
    //    public string Id { get; set; }

    //    public string[] MissingFiles { get; set; }
    //}
}