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

            string[] accountNames = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountName, false);
            string[] resourceGroupNames = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountResourceGroupName, false);
            string[] subscriptionIds = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountSubscriptionId, false);
            string[] directoryTenantIds = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountDirectoryTenantId, false);
            string[] servicePrincipalIds = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountServicePrincipalId, false);
            string[] servicePrincipalKeys = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountServicePrincipalKey, false);
            string[] videoIndexerIds = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerId, true);
            string[] videoIndexerKeys = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerKey, true);
            string[] videoIndexerRegions = AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerRegion, true);

            List<MediaAccount> mediaAccounts = new List<MediaAccount>();
            for (var i = 0; i < accountNames.Length; i++)
            {
                MediaAccount mediaAccount = new MediaAccount()
                {
                    Name = accountNames[i],
                    ResourceGroupName = resourceGroupNames[i],
                    SubscriptionId = subscriptionIds[0],
                    DirectoryTenantId = directoryTenantIds[0],
                    ServicePrincipalId = servicePrincipalIds[i],
                    ServicePrincipalKey = servicePrincipalKeys[i],
                    VideoIndexerId = videoIndexerIds[i],
                    VideoIndexerKey = videoIndexerKeys[i],
                    VideoIndexerRegion = videoIndexerRegions[i],
                    StorageAccounts = GetStorageAccounts(i)
                };
                mediaAccounts.Add(mediaAccount);
            }
            _mediaAccounts = mediaAccounts.ToArray();
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
            AddStorageAccount(storageAccounts, nameClaimType, keyClaimType, false, i);

            nameClaimType = Constant.UserAttribute.StorageAccount2Name;
            keyClaimType = Constant.UserAttribute.StorageAccount2Key;
            AddStorageAccount(storageAccounts, nameClaimType, keyClaimType, true, i);

            nameClaimType = Constant.UserAttribute.StorageAccount3Name;
            keyClaimType = Constant.UserAttribute.StorageAccount3Key;
            AddStorageAccount(storageAccounts, nameClaimType, keyClaimType, true, i);

            return storageAccounts;
        }

        private void AddStorageAccount(Dictionary<string, string> storageAccounts, string nameClaimType, string keyClaimType, bool notRequired, int i)
        {
            string[] claimValues = AuthToken.GetClaimValues(_authToken, nameClaimType, notRequired);
            if (claimValues != null)
            {
                string storageAccountName = claimValues[i];
                string storageAccountKey = string.Empty;

                claimValues = AuthToken.GetClaimValues(_authToken, keyClaimType, notRequired);
                if (claimValues != null)
                {
                    storageAccountKey = claimValues[i];
                }

                storageAccounts.Add(storageAccountName, storageAccountKey);
            }
        }
    }
}