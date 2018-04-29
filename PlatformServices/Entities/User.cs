using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    public class User
    {
        private string _authToken;
        private MediaAccount _mediaAccount;

        public User(string authToken)
        {
            _authToken = authToken;
            _mediaAccount = new MediaAccount()
            {
                DomainName = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountDomainName),
                EndpointUrl = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountEndpointUrl),
                ClientId = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountClientId),
                ClientKey = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.MediaAccountClientKey),
                IndexerKey = AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.VideoIndexerKey)
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

        public MediaAccount MediaAccount
        {
            get { return _mediaAccount; }
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
    }
}