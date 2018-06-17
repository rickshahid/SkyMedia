using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJobInput
    {
        public string AssetId { get; set; }

        public string AssetName { get; set; }

        public string AssetType { get; set; }

        public string PrimaryFile { get; set; }
    }

    public class MediaJob
    {
        public string Name { get; set; }

        public int Priority { get; set; }

        //public MediaJobTask[] Tasks { get; set; }
    }

    internal class MediaProcess
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string[] MissingFiles { get; set; }
    }
}