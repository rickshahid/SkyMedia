using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    public class User
    {
        private string _authToken;

        public User(string authToken)
        {
            _authToken = authToken;
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

        public string MediaAccountId
        {
            get
            {
                string accountId = this.MediaAccountEndpointUrl;
                accountId = accountId.Split('/')[2];
                return accountId.Split('.')[0];
            }
        }

        public string MediaAccountDomainName
        {
            get
            {
                string accountDomain = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountDomainName);
                if (string.IsNullOrEmpty(accountDomain))
                {
                    string settingKey = Constant.AppSettingKey.DirectoryMediaAccountTenantDomain;
                    accountDomain = AppSetting.GetValue(settingKey);
                }
                return accountDomain;
            }
        }

        public string MediaAccountEndpointUrl
        {
            get
            {
                string endpointUrl = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountEndpointUrl);
                if (string.IsNullOrEmpty(endpointUrl))
                {
                    string settingKey = Constant.AppSettingKey.DirectoryMediaAccountEndpointUrl;
                    endpointUrl = AppSetting.GetValue(settingKey);
                }
                return endpointUrl;
            }
        }

        public string MediaAccountClientId
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountClientId);
            }
        }

        public string MediaAccountClientKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountClientKey);
            }
        }

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

        public string VideoIndexerKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.VideoIndexerKey);
            }
        }
    }
}