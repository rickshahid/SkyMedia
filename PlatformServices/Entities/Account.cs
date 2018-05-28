using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaAccount
    {
        public string Name { get; set; }

        public string SubscriptionId { get; set; }

        public string ResourceGroupName { get; set; }        

        public string DirectoryTenantId { get; set; }

        public string ClientApplicationId { get; set; }

        public string ClientApplicationKey { get; set; }

        public Dictionary<string, string> StorageAccounts { get; set; }
    }
}