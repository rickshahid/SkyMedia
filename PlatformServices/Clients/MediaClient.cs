using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rest;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient : IDisposable
    {
        private AzureMediaServicesClient _media;

        public MediaClient(string authToken, MediaAccount mediaAccount = null)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                User authUser = new User(authToken);
                mediaAccount = authUser.MediaAccount;
            }
            this.MediaAccount = mediaAccount;
            string settingKey = Constant.AppSettingKey.AzureResourceManagementValidateUrl;
            string validateUrl = AppSetting.GetValue(settingKey);
            MediaClientCredentials clientCredentials = new MediaClientCredentials(MediaAccount);
            _media = new AzureMediaServicesClient(new Uri(validateUrl), clientCredentials)
            {
                SubscriptionId = mediaAccount.SubscriptionId
            };
            string primaryStorageId = this.PrimaryStorage.Id;
        }

        public MediaAccount MediaAccount { get; private set; }

        public StorageAccount PrimaryStorage
        {
            get
            {
                MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
                return mediaService.StorageAccounts.Where(s => s.Type == StorageAccountType.Primary).Single();
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
            _clientCredential = new ClientCredential(mediaAccount.ClientApplicationId, mediaAccount.ClientApplicationKey);
        }

        public async override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string settingKey = Constant.AppSettingKey.AzureResourceManagementAudienceUrl;
            string audienceUrl = AppSetting.GetValue(settingKey);

            AuthenticationResult authResult = await _authContext.AcquireTokenAsync(audienceUrl, _clientCredential);
            request.Headers.Authorization = new AuthenticationHeaderValue(authResult.AccessTokenType, authResult.AccessToken);
            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}