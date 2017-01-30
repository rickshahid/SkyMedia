using System;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public sealed class MediaJobNotification
    {
        public MediaJobNotificationEvent EventType { get; set; }

        public MediaJobNotificationProperties Properties { get; set; }

        public string MessageVersion { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ETag { get; set; }
    }

    public sealed class MediaJobNotificationProperties
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string JobId { get; set; }

        public string JobName { get; set; }

        public JobState OldState { get; set; }

        public JobState NewState { get; set; }
    }
}
