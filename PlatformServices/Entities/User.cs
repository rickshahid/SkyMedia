using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    internal class User
    {
        private readonly string _authToken;

        public User(string authToken)
        {
            _authToken = authToken;
            MediaAccount = new MediaAccount()
            {
                Name = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountName),
                SubscriptionId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountSubscriptionId),
                ResourceGroupName = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountResourceGroupName),
                DirectoryTenantId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountDirectoryTenantId),
                ServicePrincipalId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountServicePrincipalId),
                ServicePrincipalKey = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountServicePrincipalKey),
                VideoIndexerId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerId),
                VideoIndexerKey = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerKey),
                VideoIndexerRegion = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerRegion),
                StorageAccounts = GetStorageAccounts(_authToken)
            };
            MediaAccount.ResourceId = string.Format(Constant.Media.AccountResourceId, MediaAccount.SubscriptionId, MediaAccount.ResourceGroupName, MediaAccount.Name);
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

        public string MobilePhoneNumber
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MobilePhoneNumber);
            }
        }

        public MediaAccount MediaAccount { get; private set; }

        private Dictionary<string, string> GetStorageAccounts(string authToken)
        {
            Dictionary<string, string> storageAccounts = new Dictionary<string, string>();

            string claimType = Constant.UserAttribute.StorageAccount1Name;
            string storageAccountName = AuthToken.GetClaimValue(authToken, claimType);

            claimType = Constant.UserAttribute.StorageAccount1Key;
            string storageAccountKey = AuthToken.GetClaimValue(authToken, claimType);

            storageAccounts.Add(storageAccountName, storageAccountKey);

            claimType = Constant.UserAttribute.StorageAccount2Name;
            storageAccountName = AuthToken.GetClaimValue(authToken, claimType);

            claimType = Constant.UserAttribute.StorageAccount2Key;
            storageAccountKey = AuthToken.GetClaimValue(authToken, claimType);

            storageAccounts.Add(storageAccountName, storageAccountKey);

            claimType = Constant.UserAttribute.StorageAccount3Name;
            storageAccountName = AuthToken.GetClaimValue(authToken, claimType);

            claimType = Constant.UserAttribute.StorageAccount3Key;
            storageAccountKey = AuthToken.GetClaimValue(authToken, claimType);

            storageAccounts.Add(storageAccountName, storageAccountKey);

            return storageAccounts;
        }
    }
}