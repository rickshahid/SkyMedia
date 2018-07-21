using System;
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

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient : IDisposable
    {
        private AzureMediaServicesClient _media;
        private string _indexerAccountToken;
        private string _indexerAccountId;

        public MediaClient(string authToken, MediaAccount mediaAccount = null)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                User authUser = new User(authToken);
                mediaAccount = authUser.MediaAccount;
                UserAccount = new UserAccount()
                {
                    MobileNumber = authUser.MobileNumber
                };
            }
            MediaAccount = mediaAccount;
            string settingKey = Constant.AppSettingKey.AzureResourceManagementEndpointUrl;
            string endpointUrl = AppSetting.GetValue(settingKey);
            MediaClientCredentials clientCredentials = new MediaClientCredentials(MediaAccount);
            _media = new AzureMediaServicesClient(new Uri(endpointUrl), clientCredentials)
            {
                SubscriptionId = mediaAccount.SubscriptionId
            };
            if (!string.IsNullOrEmpty(authToken) && !string.IsNullOrEmpty(MediaAccount.VideoIndexerKey))
            {
                settingKey = Constant.AppSettingKey.MediaIndexerAuthUrl;
                string authUrl = AppSetting.GetValue(settingKey);
                WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
                using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, authUrl))
                {
                    JArray indexerAccounts = webClient.GetResponse<JArray>(webRequest);
                    if (indexerAccounts != null)
                    {
                        _indexerAccountId = indexerAccounts[0]["id"].ToString();
                        _indexerAccountToken = indexerAccounts[0]["accessToken"].ToString();
                    }
                }
            }
        }

        public MediaAccount MediaAccount { get; private set; }

        public UserAccount UserAccount { get; private set; }

        public StorageAccount PrimaryStorage
        {
            get
            {
                MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
                return mediaService.StorageAccounts.Where(s => s.Type == StorageAccountType.Primary).Single();
            }
        }

        public IList<StorageAccount> StorageAccounts
        {
            get
            {
                MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
                return mediaService.StorageAccounts;
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