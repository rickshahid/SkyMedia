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

        public string VideoIndexerKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.VideoIndexerKey);
            }
        }

        public string[] StorageAccountNames
        {
            get
            {
                return AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.StorageAccountName);
            }
        }

        public string[] StorageAccountKeys
        {
            get
            {
                return AuthToken.GetClaimValues(_authToken, Constant.UserAttribute.StorageAccountKey);
            }
        }

        public string SigniantServiceGateway
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.SigniantServiceGateway);
            }
        }

        public string SigniantAccountKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.SigniantAccountKey);
            }
        }

        public string AsperaServiceGateway
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.AsperaServiceGateway);
            }
        }

        public string AsperaAccountId
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.AsperaAccountId);
            }
        }

        public string AsperaAccountKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, Constant.UserAttribute.AsperaAccountKey);
            }
        }
    }
}