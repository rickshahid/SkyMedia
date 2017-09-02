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
            BindContext(authUser.MediaAccountId, authUser.MediaAccountKey);
        }

        public MediaClient(string accountId, string accountKey)
        {
            BindContext(accountId, accountKey);
        }

        private void BindContext(string accountId, string accountKey)
        {
            MediaServicesCredentials mediaCredentials = new MediaServicesCredentials(accountId, accountKey);
            _media = new CloudMediaContext(mediaCredentials);
            IStorageAccount storageAccount = this.DefaultStorageAccount;
        }

        //public MediaClient(string authToken)
        //{
        //    string settingKey = Constant.AppSettingKey.DirectoryDomainUser;
        //    string userDomain = AppSetting.GetValue(settingKey);

        //    User authUser = new User(authToken);
        //    AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(userDomain, AzureEnvironments.AzureCloudEnvironment);

        //    BindContext(authUser.MediaAccountUrl, tokenCredentials);
        //}

        //public MediaClient(string accountUrl, string clientId, string clientKey)
        //{
        //    string settingKey = Constant.AppSettingKey.DirectoryDomainService;
        //    string serviceDomain = AppSetting.GetValue(settingKey);

        //    AzureAdClientSymmetricKey symmetricKey = new AzureAdClientSymmetricKey(clientId, clientKey);
        //    AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(serviceDomain, symmetricKey, AzureEnvironments.AzureCloudEnvironment);

        //    BindContext(accountUrl, tokenCredentials);
        //}

        //private void BindContext(string accountUrl, AzureAdTokenCredentials tokenCredentials)
        //{
        //    AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);
        //    _media = new CloudMediaContext(new Uri(accountUrl), tokenProvider);
        //    IStorageAccount storageAccount = this.DefaultStorageAccount;
        //}

        public IStorageAccount DefaultStorageAccount
        {
            get { return _media.DefaultStorageAccount; }
        }
    }
}