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
                ClientApplicationKey = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountClientApplicationKey)
            };
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

        public string[] StorageAccountNames
        {
            get
            {
                List<string> accountNames = new List<string>();

                string claimType = Constant.UserAttribute.StorageAccount1Name;
                string claimValue = AuthToken.GetClaimValue(_authToken, claimType);
                if (!string.IsNullOrEmpty(claimValue))
                {
                    accountNames.Add(claimValue);
                }

                claimType = Constant.UserAttribute.StorageAccount2Name;
                claimValue = AuthToken.GetClaimValue(_authToken, claimType);
                if (!string.IsNullOrEmpty(claimValue))
                {
                    accountNames.Add(claimValue);
                }

                claimType = Constant.UserAttribute.StorageAccount3Name;
                claimValue = AuthToken.GetClaimValue(_authToken, claimType);
                if (!string.IsNullOrEmpty(claimValue))
                {
                    accountNames.Add(claimValue);
                }

                return accountNames.ToArray();
            }
        }

        public string[] StorageAccountKeys
        {
            get
            {
                List<string> accountKeys = new List<string>();

                string claimType = Constant.UserAttribute.StorageAccount1Key;
                string claimValue = AuthToken.GetClaimValue(_authToken, claimType);
                if (!string.IsNullOrEmpty(claimValue))
                {
                    accountKeys.Add(claimValue);
                }

                claimType = Constant.UserAttribute.StorageAccount2Key;
                claimValue = AuthToken.GetClaimValue(_authToken, claimType);
                if (!string.IsNullOrEmpty(claimValue))
                {
                    accountKeys.Add(claimValue);
                }

                claimType = Constant.UserAttribute.StorageAccount3Key;
                claimValue = AuthToken.GetClaimValue(_authToken, claimType);
                if (!string.IsNullOrEmpty(claimValue))
                {
                    accountKeys.Add(claimValue);
                }

                return accountKeys.ToArray();
            }
        }
    }
}