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

        private string _indexerAccountId;
        private string _indexerAccountToken;

        public MediaClient(string authToken, MediaAccount mediaAccount = null)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                User userProfile = new User(authToken);
                mediaAccount = userProfile.MediaAccount;
            }
            MediaAccount = mediaAccount;
            string settingKey = Constant.AppSettingKey.AzureResourceManagementServiceUrl;
            string serviceUrl = AppSetting.GetValue(settingKey);
            MediaClientCredentials clientCredentials = new MediaClientCredentials(mediaAccount);
            _media = new AzureMediaServicesClient(new Uri(serviceUrl), clientCredentials)
            {
                SubscriptionId = mediaAccount.SubscriptionId
            };
            if (!string.IsNullOrEmpty(MediaAccount.VideoIndexerRegion) && 
                !string.IsNullOrEmpty(MediaAccount.VideoIndexerKey))
            {
                IndexerSetAccountContext();
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

            settingKey = Constant.AppSettingKey.AzureResourceManagementTokenScope;
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