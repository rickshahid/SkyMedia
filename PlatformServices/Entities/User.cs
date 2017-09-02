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
                return AuthToken.GetClaimValue(_authToken, "emails");
            }
        }

        public string MobileNumber
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_MobileNumber");
            }
        }

        public string MediaAccountId
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_MediaAccountId");
            }
        }

        //public string MediaAccountName
        //{
        //    get
        //    {
        //        string accountId = this.MediaAccountId;
        //        accountId = accountId.Split('/')[2];
        //        return accountId.Split('.')[0];
        //    }
        //}

        public string MediaAccountKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_MediaAccountKey");
            }
        }

        //public string MediaClientId
        //{
        //    get
        //    {
        //        return AuthToken.GetClaimValue(_authToken, "extension_MediaClientId");
        //    }
        //}

        //public string MediaClientKey
        //{
        //    get
        //    {
        //        return AuthToken.GetClaimValue(_authToken, "extension_MediaClientKey");
        //    }
        //}

        public string VideoIndexerKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_VideoIndexerKey");
            }
        }

        public string[] StorageAccountNames
        {
            get
            {
                return AuthToken.GetClaimValues(_authToken, "extension_StorageAccountName");
            }
        }

        public string[] StorageAccountKeys
        {
            get
            {
                return AuthToken.GetClaimValues(_authToken, "extension_StorageAccountKey");
            }
        }

        public string SigniantServiceGateway
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_SigniantServiceGateway");
            }
        }

        public string SigniantAccountKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_SigniantAccountKey");
            }
        }

        public string AsperaServiceGateway
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_AsperaServiceGateway");
            }
        }

        public string AsperaAccountId
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_AsperaAccountId");
            }
        }

        public string AsperaAccountKey
        {
            get
            {
                return AuthToken.GetClaimValue(_authToken, "extension_AsperaAccountKey");
            }
        }
    }
}