using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    internal class User
    {
        private string _authToken;

        public User(string authToken)
        {
            _authToken = authToken;
            this.MediaAccount = new MediaAccount()
            {
                Name = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountName),
                SubscriptionId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountSubscriptionId),
                ResourceGroupName = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountResourceGroupName),
                DirectoryTenantId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountDirectoryTenantId),
                ClientApplicationId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountClientApplicationId),
                ClientApplicationKey = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountClientApplicationKey),
                StorageAccounts = GetStorageAccounts()
            };
            this.MediaAccount.ResourceId = string.Concat("/subscriptions/", this.MediaAccount.SubscriptionId, "/resourceGroups/", this.MediaAccount.ResourceGroupName, "/providers/Microsoft.Media/mediaservices/", this.MediaAccount.Name);
        }

        public string Id
        {
            get
            {
                string userId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.EMails);
                if (string.IsNullOrEmpty(userId))
                {
                    userId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.EMail);
                }
                return userId;
            }
        }

        public string MobileNumber
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MobileNumber);
            }
        }

        public MediaAccount MediaAccount { get; private set; }

        private Dictionary<string, string> GetStorageAccounts()
        {
            Dictionary<string, string> storageAccounts = new Dictionary<string, string>();

            string claimType = Constant.UserAttribute.StorageAccount1Name;
            string storageAccountName = AuthToken.GetClaimValue(_authToken, claimType);

            claimType = Constant.UserAttribute.StorageAccount1Key;
            string storageAccountKey = AuthToken.GetClaimValue(_authToken, claimType);

            storageAccounts.Add(storageAccountName, storageAccountKey);

            claimType = Constant.UserAttribute.StorageAccount2Name;
            storageAccountName = AuthToken.GetClaimValue(_authToken, claimType);

            claimType = Constant.UserAttribute.StorageAccount2Key;
            storageAccountKey = AuthToken.GetClaimValue(_authToken, claimType);

            storageAccounts.Add(storageAccountName, storageAccountKey);

            claimType = Constant.UserAttribute.StorageAccount3Name;
            storageAccountName = AuthToken.GetClaimValue(_authToken, claimType);

            claimType = Constant.UserAttribute.StorageAccount3Key;
            storageAccountKey = AuthToken.GetClaimValue(_authToken, claimType);

            storageAccounts.Add(storageAccountName, storageAccountKey);

            return storageAccounts;
        }
    }
}