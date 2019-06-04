using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    internal class User
    {
        private readonly string _authToken;
        private readonly MediaAccount[] _mediaAccounts;

        public User(string authToken)
        {
            _authToken = authToken;

            string[] accountNames = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountName);
            string[] resourceGroupNames = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountResourceGroupName);
            string subscriptionId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountSubscriptionId);
            string directoryTenantId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountDirectoryTenantId);
            string[] servicePrincipalIds = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountServicePrincipalId);
            string[] servicePrincipalKeys = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountServicePrincipalKey);
            string[] videoIndexerIds = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerId);
            string[] videoIndexerKeys = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerKey);
            string[] videoIndexerRegions = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerRegion);

            List<MediaAccount> mediaAccounts = new List<MediaAccount>();
            for (int i = 0; i < accountNames.Length; i++)
            {
                MediaAccount mediaAccount = new MediaAccount()
                {
                    Name = accountNames[i],
                    ResourceGroupName = resourceGroupNames[i],
                    SubscriptionId = subscriptionId,
                    DirectoryTenantId = directoryTenantId,
                    ServicePrincipalId = servicePrincipalIds[i],
                    ServicePrincipalKey = servicePrincipalKeys[i],
                    VideoIndexerId = videoIndexerIds == null || i >= videoIndexerIds.Length ? null : videoIndexerIds[i],
                    VideoIndexerKey = videoIndexerKeys == null || i >= videoIndexerKeys.Length ? null : videoIndexerKeys[i],
                    VideoIndexerRegion = videoIndexerRegions == null || i >= videoIndexerRegions.Length ? null : videoIndexerRegions[i],
                    StorageAccounts = GetStorageAccounts(i)
                };
                mediaAccounts.Add(mediaAccount);
            }
            _mediaAccounts = mediaAccounts.ToArray();
        }

        public string Id
        {
            get { return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.Id); }
        }

        public string MobilePhoneNumber
        {
            get { return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MobilePhoneNumber); }
        }

        public MediaAccount MediaAccount
        {
            get { return _mediaAccounts[0]; }
        }

        private Dictionary<string, string> GetStorageAccounts(int i)
        {
            Dictionary<string, string> storageAccounts = new Dictionary<string, string>();

            string nameClaimType = Constant.UserAttribute.StorageAccount1Name;
            string keyClaimType = Constant.UserAttribute.StorageAccount1Key;
            AddStorageAccount(storageAccounts, nameClaimType, keyClaimType, i);

            nameClaimType = Constant.UserAttribute.StorageAccount2Name;
            keyClaimType = Constant.UserAttribute.StorageAccount2Key;
            AddStorageAccount(storageAccounts, nameClaimType, keyClaimType, i);

            nameClaimType = Constant.UserAttribute.StorageAccount3Name;
            keyClaimType = Constant.UserAttribute.StorageAccount3Key;
            AddStorageAccount(storageAccounts, nameClaimType, keyClaimType, i);

            return storageAccounts;
        }

        private void AddStorageAccount(Dictionary<string, string> storageAccounts, string nameClaimType, string keyClaimType, int i)
        {
            string[] claimValues = AuthToken.GetClaimValues(_authToken, nameClaimType);
            if (claimValues != null)
            {
                string storageAccountName = claimValues[i];
                string storageAccountKey = string.Empty;

                claimValues = AuthToken.GetClaimValues(_authToken, keyClaimType);
                if (claimValues != null)
                {
                    storageAccountKey = claimValues[i];
                }

                storageAccounts.Add(storageAccountName, storageAccountKey);
            }
        }
    }
}