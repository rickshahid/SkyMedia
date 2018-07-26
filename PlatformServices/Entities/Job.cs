using System.Collections.Generic;

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

        public IDictionary<string, string> CorrelationData { get; set; }
    }

    //internal class MediaProcess
    //{
    //    [JsonProperty(PropertyName = "id")]
    //    public string Id { get; set; }

    //    public string[] MissingFiles { get; set; }
    //}
}