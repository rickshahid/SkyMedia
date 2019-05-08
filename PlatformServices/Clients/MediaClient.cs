using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient : IDisposable
    {
        private AzureMediaServicesClient _media;

        private string _indexerAccountId;
        private string _indexerAccountToken;

        public MediaClient(string authToken)
        {
            User currentUser = new User(authToken);
            MediaAccount = currentUser.MediaAccount;
            UserAccount = new UserAccount()
            {
                MobilePhoneNumber = currentUser.MobilePhoneNumber
            };
            BindContext();
        }

        public MediaClient(MediaAccount mediaAccount, UserAccount userAccount)
        {
            MediaAccount = mediaAccount;
            UserAccount = userAccount;
            BindContext();
        }

        public MediaClient(MediaWorkflowManifest workflowManifest) :
            this(workflowManifest.MediaAccounts[0], workflowManifest.UserAccount)
        {
        }

        private void BindContext()
        {
            ServiceClientCredentials clientCredentials = ApplicationTokenProvider.LoginSilentAsync(MediaAccount.DirectoryTenantId, MediaAccount.ServicePrincipalId, MediaAccount.ServicePrincipalKey).Result;
            _media = new AzureMediaServicesClient(clientCredentials)
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

        public string StorageAccount
        {
            get
            {
                MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
                StorageAccount primaryStorage = mediaService.StorageAccounts.Where(s => s.Type == StorageAccountType.Primary).Single();
                return Path.GetFileName(primaryStorage.Id);
            }
        }

        public UserAccount UserAccount { get; }

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
}