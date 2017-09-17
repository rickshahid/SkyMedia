using System;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private CloudMediaContext _media;

        public MediaClient(string authToken)
        {
            User authUser = new User(authToken);

            AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(authUser.MediaAccountClientId, authUser.MediaAccountClientKey);
            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(authUser.MediaAccountDomainName, symmetricKey, AzureEnvironments.AzureCloudEnvironment);
            //AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(authUser.MediaAccountDomainName, AzureEnvironments.AzureCloudEnvironment);

            BindContext(authUser.MediaAccountEndpointUrl, tokenCredentials);
        }

        public MediaClient(string accountDomain, string accountUrl, string clientId, string clientKey)
        {
            AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(clientId, clientKey);
            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(accountDomain, symmetricKey, AzureEnvironments.AzureCloudEnvironment);

            BindContext(accountUrl, tokenCredentials);
        }

        private void BindContext(string accountUrl, AzureAdTokenCredentials tokenCredentials)
        {
            AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);
            _media = new CloudMediaContext(new Uri(accountUrl), tokenProvider);
            IStorageAccount storageAccount = this.DefaultStorageAccount;
        }

        public IStorageAccount DefaultStorageAccount
        {
            get { return _media.DefaultStorageAccount; }
        }
    }
}