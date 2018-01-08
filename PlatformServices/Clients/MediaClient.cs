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
            if (string.IsNullOrEmpty(authUser.MediaAccount.ClientId))
            {
                tokenCredentials = new AzureAdTokenCredentials(authUser.MediaAccount.DomainName, _azure);
            }
            else
            {
                AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(authUser.MediaAccount.ClientId, authUser.MediaAccount.ClientKey);
                tokenCredentials = new AzureAdTokenCredentials(authUser.MediaAccount.DomainName, symmetricKey, _azure);
            }
            BindContext(authUser.MediaAccount.EndpointUrl, tokenCredentials);
        }

        public MediaClient(MediaAccount mediaAccount)
        {
            AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(mediaAccount.ClientId, mediaAccount.ClientKey);
            AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(mediaAccount.DomainName, symmetricKey, _azure);
            BindContext(mediaAccount.EndpointUrl, tokenCredentials);
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