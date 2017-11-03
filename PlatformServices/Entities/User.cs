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
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.UserId);
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
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountDomainName);
            }
        }

        public string MediaAccountEndpointUrl
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountEndpointUrl);
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