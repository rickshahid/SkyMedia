namespace AzureSkyMedia.PlatformServices
{
    internal class MediaPublish
    {
        public string StreamingPolicyName { get; set; }

        public ContentProtection ContentProtection { get; set; }

        public UserContact UserContact { get; set; }
    }

    internal class UserContact
    {
        public string MobilePhoneNumber { get; set; }

        public string NotificationMessage { get; set; }
    }
}