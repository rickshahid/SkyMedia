using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient : IDisposable
    {
        private AzureMediaServicesClient _media;
        private string _indexerAccountToken;
        private string _indexerAccountId;

        public MediaClient(string authToken, MediaAccount mediaAccount = null, UserAccount userAccount = null)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                User authUser = new User(authToken);
                mediaAccount = authUser.MediaAccount;
                userAccount = new UserAccount()
                {
                    MobileNumber = authUser.MobileNumber
                };
            }
            MediaAccount = mediaAccount;
            UserAccount = userAccount;
            string settingKey = Constant.AppSettingKey.AzureResourceManagementEndpointUrl;
            string endpointUrl = AppSetting.GetValue(settingKey);
            MediaClientCredentials clientCredentials = new MediaClientCredentials(MediaAccount);
            _media = new AzureMediaServicesClient(new Uri(endpointUrl), clientCredentials)
            {
                SubscriptionId = mediaAccount.SubscriptionId
            };
            if (!string.IsNullOrEmpty(MediaAccount.VideoIndexerKey))
            {
                IndexerSetAccountContext();
            }
        }

        public MediaAccount MediaAccount { get; private set; }

        public UserAccount UserAccount { get; private set; }

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
        private AuthenticationContext _authContext;
        private ClientCredential _clientCredential;

        public MediaClientCredentials(MediaAccount mediaAccount)
        {
            string settingKey = Constant.AppSettingKey.DirectoryIssuerUrl;
            string authorityUrl = AppSetting.GetValue(settingKey);
            authorityUrl = string.Format(authorityUrl, mediaAccount.DirectoryTenantId);

            _authContext = new AuthenticationContext(authorityUrl);
            _clientCredential = new ClientCredential(mediaAccount.ServicePrincipalId, mediaAccount.ServicePrincipalKey);
        }

        public async override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string settingKey = Constant.AppSettingKey.AzureResourceManagementAudienceUrl;
            string audienceUrl = AppSetting.GetValue(settingKey);

            AuthenticationResult azureToken = await _authContext.AcquireTokenAsync(audienceUrl, _clientCredential);
            request.Headers.Authorization = new AuthenticationHeaderValue(azureToken.AccessTokenType, azureToken.AccessToken);
            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}