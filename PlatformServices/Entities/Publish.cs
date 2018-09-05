using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaPublish
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string ProcessState { get; set; }

        public string TransformName { get; set; }

        public string StreamingPolicyName { get; set; }

        public ContentProtection ContentProtection { get; set; }

        public MediaAccount MediaAccount { get; set; }

        public UserContact UserContact { get; set; }
    }

    internal class UserContact
    {
        public string MobilePhoneNumber { get; set; }

        public string NotificationMessage { get; set; }
    }
}