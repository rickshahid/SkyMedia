using System;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private AzureEnvironment _azure = AzureEnvironments.AzureCloudEnvironment;
        private CloudMediaContext _media;

        public MediaClient(string authToken)
        {
            User authUser = new User(authToken);
            AzureAdTokenCredentials tokenCredentials;
            if (string.IsNullOrEmpty(authUser.MediaAccountClientId))
            {
                tokenCredentials = new AzureAdTokenCredentials(authUser.MediaAccountDomainName, _azure);
            }
            else
            {
                AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(authUser.MediaAccountClientId, authUser.MediaAccountClientKey);
                tokenCredentials = new AzureAdTokenCredentials(authUser.MediaAccountDomainName, symmetricKey, _azure);
            }
            BindContext(authUser.MediaAccountEndpointUrl, tokenCredentials);
        }

        public MediaClient(string accountDomain, string accountEndpoint, string clientId, string clientKey)
        {
            AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(clientId, clientKey);
            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(accountDomain, symmetricKey, _azure);
            BindContext(accountEndpoint, tokenCredentials);
        }

        private void BindContext(string accountEndpoint, AzureAdTokenCredentials tokenCredentials)
        {
            AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);
            _media = new CloudMediaContext(new Uri(accountEndpoint), tokenProvider);
            IStorageAccount storageAccount = this.DefaultStorageAccount;
        }

        public IStorageAccount DefaultStorageAccount
        {
            get { return _media.DefaultStorageAccount; }
        }
    }
}