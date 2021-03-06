﻿using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    public class UserAccount
    {
        public string MobilePhoneNumber { get; set; }
    }

    public class MediaAccount
    {
        public string Name { get; set; }

        public string ResourceGroupName { get; set; }

        public string ResourceId
        {
            get { return string.Format(Constant.Media.AccountResourceId, this.SubscriptionId, this.ResourceGroupName, this.Name); }
        }

        public string SubscriptionId { get; set; }

        public string DirectoryTenantId { get; set; }

        public string ServicePrincipalId { get; set; }

        public string ServicePrincipalKey { get; set; }

        public string VideoIndexerId { get; set; }

        public string VideoIndexerKey { get; set; }

        public string VideoIndexerRegion { get; set; }

        public Dictionary<string, string> StorageAccounts { get; set; }
    }
}