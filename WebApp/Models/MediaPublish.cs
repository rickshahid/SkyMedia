﻿using Microsoft.WindowsAzure.MediaServices.Client;

namespace SkyMedia.WebApp.Models
{
    public class MediaJobPublish
    {
        public string AccountName { get; set; }

        public string JobId { get; set; }

        public JobState NewState { get; set; }
    }

    public class ContentProtection : StorageEntity
    {
        public bool AES { get; set; }

        public bool DRMPlayReady { get; set; }

        public bool DRMWidevine { get; set; }

        public bool ContentAuthTypeToken { get; set; }

        public bool ContentAuthTypeAddress { get; set; }

        public string ContentAuthAddressRange { get; set; }
    }

    internal class ContentPublish : StorageEntity
    {
        public string MediaAccountKey { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }

        public string MobileNumber { get; set; }
    }

    public class PublishNotification
    {
        public PublishNotification()
        {
            this.MessageText = string.Empty;
            this.MobileNumber = string.Empty;
        }

        public string MessageText { get; set; }

        public string MobileNumber { get; set; }
    }
}
