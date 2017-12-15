using System;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal enum MediaJobNotificationEvent
    {
        None = 0,
        JobStateChange = 1,
        NotificationEndPointRegistration = 2,
        NotificationEndPointUnregistration = 3,
        TaskStateChange = 4,
        TaskProgress = 5
    }

    internal enum MediaJobState
    {
        Queued = 0,
        Scheduled = 1,
        Processing = 2,
        Finished = 3,
        Error = 4,
        Canceled = 5,
        Canceling = 6
    }

    internal sealed class MediaJobNotification
    {
        public MediaJobNotificationEvent EventType { get; set; }

        public MediaJobNotificationProperties Properties { get; set; }

        public string MessageVersion { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ETag { get; set; }
    }

    internal sealed class MediaJobNotificationProperties
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string JobId { get; set; }

        public string JobName { get; set; }

        public MediaJobState OldState { get; set; }

        public MediaJobState NewState { get; set; }
    }
}