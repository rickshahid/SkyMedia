using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest;
using Microsoft.Identity.Client;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient : IDisposable
    {
        private AzureMediaServicesClient _media;

        private string _indexerRegionUrl;
        private string _indexerAccountId;
        private string _indexerAccountToken;

        public MediaClient(string authToken)
        {
            User userProfile = new User(authToken);
            MediaAccount = userProfile.MediaAccount;
            BindContext(null);
        }

        public MediaClient(MediaAccount mediaAccount, HttpClient httpClient)
        {
            MediaAccount = mediaAccount;
            BindContext(httpClient);
        }

        private void BindContext(HttpClient httpClient)
        {
            MediaClientCredentials clientCredentials = new MediaClientCredentials(MediaAccount);
            _media = new AzureMediaServicesClient(clientCredentials, httpClient, true)
            {
                SubscriptionId = MediaAccount.SubscriptionId
            };
            if (!string.IsNullOrEmpty(MediaAccount.VideoIndexerId) &&
                !string.IsNullOrEmpty(MediaAccount.VideoIndexerKey))
            {
                IndexerSetAccount();
            }
        }

        public MediaAccount MediaAccount { get; }

        public IList<StorageAccount> StorageAccounts
        {
            get
            {
                MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
                return mediaService.StorageAccounts;
            }
        }

        public string PrimaryStorageAccount
        {
            get
            {
                MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
                StorageAccount primaryStorage = mediaService.StorageAccounts.Where(s => s.Type == StorageAccountType.Primary).Single();
                return Path.GetFileName(primaryStorage.Id);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _media != null)
            {
                _media.Dispose();
                _media = null;
            }
        }
    }

    internal class MediaClientCredentials : ServiceClientCredentials
    {
        private readonly MediaAccount _mediaAccount;

        public MediaClientCredentials(MediaAccount mediaAccount)
        {
            _mediaAccount = mediaAccount;
        }

        public static Task<AuthenticationResult> AcquireToken(MediaAccount mediaAccount)
        {
            string settingKey = Constant.AppSettingKey.DirectoryAuthorityUrl;
            string authorityUrl = AppSetting.GetValue(settingKey);
            authorityUrl = string.Format(authorityUrl, mediaAccount.DirectoryTenantId);

            string redirectUri = Constant.AuthIntegration.RedirectUri;
            ClientCredential clientCredential = new ClientCredential(mediaAccount.ServicePrincipalKey);
            ConfidentialClientApplication clientApplication = new ConfidentialClientApplication(mediaAccount.ServicePrincipalId, authorityUrl, redirectUri, clientCredential, null, null);

            settingKey = Constant.AppSettingKey.DirectoryTokenScope;
            string tokenScope = AppSetting.GetValue(settingKey);

            string[] tokenScopes = new string[] { tokenScope };
            return clientApplication.AcquireTokenForClientAsync(tokenScopes);
        }

        public async override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthenticationResult authResult = await AcquireToken(_mediaAccount);

            string authScheme = Constant.AuthIntegration.AuthScheme; 
            request.Headers.Authorization = new AuthenticationHeaderValue(authScheme, authResult.AccessToken);

            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}