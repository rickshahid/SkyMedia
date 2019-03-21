using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Rest;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;

using StorageAccount = Microsoft.Azure.Management.Storage.Models.StorageAccount;
using MediaStorageAccount = Microsoft.Azure.Management.Media.Models.StorageAccount;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaStorage : MediaStorageAccount
    {
        private string _storageAccountName;
        private StorageAccount _storageAccount;

        internal MediaStorage(MediaAccount mediaAccount, MediaStorageAccount storageAccount) : base(storageAccount.Type, storageAccount.Id)
        {
            TokenCredentials authToken = AuthToken.AcquireToken(mediaAccount);
            StorageManagementClient storageClient = new StorageManagementClient(authToken)
            {
                SubscriptionId = mediaAccount.SubscriptionId
            };
            _storageAccountName = Path.GetFileName(storageAccount.Id);
            IEnumerable<StorageAccount> storageAccounts = storageClient.StorageAccounts.List();
            storageAccounts = storageAccounts.Where(s => s.Name.Equals(_storageAccountName, StringComparison.OrdinalIgnoreCase));
            _storageAccount = storageAccounts.SingleOrDefault();
        }

        public string Name
        {
            get { return _storageAccountName; }
        }

        public string AccountType
        {
            get
            {
                string accountType = Constant.NotAvailable;
                if (_storageAccount != null && _storageAccount.Kind.HasValue)
                {
                    switch (_storageAccount.Kind.Value)
                    {
                        case Kind.Storage:
                            accountType = "General v1";
                            break;
                        case Kind.StorageV2:
                            accountType = "General v2";
                            break;
                        case Kind.BlobStorage:
                            accountType = "Blob";
                            break;
                        case Kind.BlockBlobStorage:
                            accountType = "Premium Blob";
                            break;
                    }
                }
                return accountType;
            }
        }

        public string AccessTier
        {
            get
            {
                string accessTier = Constant.NotAvailable;
                if (_storageAccount != null && _storageAccount.AccessTier.HasValue)
                {
                    accessTier = _storageAccount.AccessTier.Value.ToString();
                }
                return accessTier;
            }
        }

        public string HttpsOnly
        {
            get
            {
                string httpsOnly = Constant.NotAvailable;
                if (_storageAccount != null)
                {
                    if (!_storageAccount.EnableHttpsTrafficOnly.HasValue)
                    {
                        _storageAccount.EnableHttpsTrafficOnly = false;
                    }
                    httpsOnly = _storageAccount.EnableHttpsTrafficOnly.Value.ToString();
                }
                return httpsOnly;
            }
        }

        public string Encryption
        {
            get
            {
                string encryption = Constant.NotAvailable;
                if (_storageAccount != null)
                {
                    if (_storageAccount.Encryption.Services.Blob.Enabled.HasValue &&
                        _storageAccount.Encryption.Services.Blob.Enabled.Value)
                    {
                        encryption = "Enabled";
                    }
                    else
                    {
                        encryption = "Not Enabled";
                    }
                }
                return encryption;
            }
        }

        public string Replication
        {
            get
            {
                string replication = Constant.NotAvailable;
                if (_storageAccount != null)
                {
                    replication = _storageAccount.Sku.Name.ToString();
                    replication = Constant.TextFormatter.FormatValue(replication);
                }
                return replication;
            }
        }

        public string PrimaryRegion
        {
            get
            {
                string primaryRegion = Constant.NotAvailable;
                if (_storageAccount != null)
                {
                    primaryRegion = _storageAccount.PrimaryLocation.ToUpperInvariant();
                }
                return primaryRegion;
            }
        }

        public string SecondaryRegion
        {
            get
            {
                string secondaryRegion = Constant.NotAvailable;
                if (_storageAccount != null && !string.IsNullOrEmpty(_storageAccount.SecondaryLocation))
                {
                    secondaryRegion = _storageAccount.SecondaryLocation.ToUpperInvariant();
                }
                return secondaryRegion;

            }
        }

        public override string ToString()
        {
            string mediaStorage;
            if (_storageAccount == null)
            {
                mediaStorage = Constant.Message.StorageAccountReadPermission;
            }
            else
            {
                mediaStorage = string.Concat(" (Storage Type: ", this.AccountType);
                mediaStorage = string.Concat(mediaStorage, ", Access Tier: ", this.AccessTier);
                mediaStorage = string.Concat(mediaStorage, ", Media Type: ", this.Type, ")");
            }
            return mediaStorage;
        }
    }
}