using System;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private MediaAccount _account;
        private CloudMediaContext _media;

        public MediaClient(string authToken)
        {
            User authUser = new User(authToken);
            _account = authUser.MediaAccount;
            BindContext();
        }

        public MediaClient(MediaAccount mediaAccount)
        {
            _account = mediaAccount;
            BindContext();
        }

        private void BindContext()
        {
            AzureAdTokenCredentials tokenCredentials;
            AzureEnvironment azureEnvironment = AzureEnvironments.AzureCloudEnvironment;
            if (string.IsNullOrEmpty(_account.ClientId))
            {
                tokenCredentials = new AzureAdTokenCredentials(_account.DomainName, azureEnvironment);
            }
            else
            {
                AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(_account.ClientId, _account.ClientKey);
                tokenCredentials = new AzureAdTokenCredentials(_account.DomainName, symmetricKey, azureEnvironment);
            }
            Uri accountEndpoint = new Uri(_account.EndpointUrl);
            AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);
            _media = new CloudMediaContext(accountEndpoint, tokenProvider);
            IStorageAccount storageAccount = this.StorageAccount;
        }

        public MediaAccount MediaAccount
        {
            get { return _account; }
        }

        public IStorageAccount StorageAccount
        {
            get { return _media.DefaultStorageAccount; }
        }
    }
}