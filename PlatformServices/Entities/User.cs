using System.Collections.Generic;

using Microsoft.Identity.Client;

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
                VideoIndexerRegion = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerRegion),
                VideoIndexerKey = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountVideoIndexerKey),
                StorageAccounts = GetStorageAccounts(_authToken),
                ClientApplication = GetClientApplication()
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

        private ConfidentialClientApplication GetClientApplication()
        {
            string settingKey = Constant.AppSettingKey.DirectoryAuthorityUrl;
            string authorityUrl = AppSetting.GetValue(settingKey);
            authorityUrl = string.Format(authorityUrl, MediaAccount.DirectoryTenantId);

            string redirectUri = Constant.AuthIntegration.RedirectUri;
            ClientCredential clientCredential = new ClientCredential(MediaAccount.ServicePrincipalKey);
            return new ConfidentialClientApplication(MediaAccount.ServicePrincipalId, authorityUrl, redirectUri, clientCredential, null, null);
        }
    }
}