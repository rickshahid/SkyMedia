using System;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaPublish
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string TransformName { get; set; }

        public ContentProtection ContentProtection { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public string MobileNumber { get; set; }
    }

    //internal class MediaPublishError
    //{
    //    [JsonProperty(PropertyName = "id")]
    //    public string Id { get; set; }

    //    public Exception Exception { get; set; }
    //}

    //public class MediaPublished
    //{
    //    public string PublishUrl { get; set; }

    //    public string MobileNumber { get; set; }

    //    public string UserMessage { get; set; }
    //}
}