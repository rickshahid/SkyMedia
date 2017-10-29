using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Analytics;
using Microsoft.WindowsAzure.MediaServices.Client;

using Microsoft.IdentityModel.Clients.ActiveDirectory;

using Microsoft.Rest;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class StorageEntity : TableEntity
    {
        public string UserId { get; set; }

        public string MobileNumber { get; set; }

        public DateTime? CreatedOn { get; set; }
    }

    internal static class Storage
    {
        public static string GetAccountType(string tenantId, string clientId, string clientKey, string accountName)
        {
            //var authContext = new AuthenticationContext(string.Format("https://login.windows.net/{0}", tenantId));
            //var credential = new ClientCredential(clientId, clientKey);
            //AuthenticationResult authResult = authContext.AcquireTokenAsync("https://management.core.windows.net/", credential).Result;
            //string token = authResult.CreateAuthorizationHeader().Substring(Constant.HttpHeader.AuthPrefix.Length);

            //TokenCredentials credentials = new TokenCredentials(token);

            //StorageManagementClient storageClient = new StorageManagementClient(credentials);
            //storageClient.SubscriptionId = "3d07cfbc-17aa-41b4-baa1-488fef85a1d3";
            //IEnumerable<StorageAccount> storageAccounts = storageClient.StorageAccounts.List();

            //StorageAccountListKeysResult keys = storageClient.StorageAccounts.ListKeys("SkyMedia-USWest-Data", "skymediauswest0");

            //foreach (StorageAccount storageAccount in storageAccounts)
            //{
            //    string x = storageAccount.Sku.ToString();
            //    string y = storageAccount.Kind.ToString();
            //    string z = storageAccount.AccessTier.ToString();
            //}

            string accountType = "Type: General";
            string storageTier = "";
            if (!string.IsNullOrEmpty(storageTier))
            {
                accountType = string.Concat(accountType, ", ", storageTier);
            }
            return accountType;
        }

        private static string GetCapacityUsed(string authToken, string accountName)
        {
            string capacityUsed = string.Empty;
            CloudStorageAccount storageAccount = GetUserAccount(authToken, accountName);
            if (storageAccount != null)
            {
                CloudAnalyticsClient storageAnalytics = storageAccount.CreateCloudAnalyticsClient();
                TableQuery<CapacityEntity> tableQuery = storageAnalytics.CreateCapacityQuery();
                IQueryable<CapacityEntity> capacityQuery = tableQuery.Where(x => x.RowKey == "data");
                try
                {
                    List<CapacityEntity> capacityEntities = capacityQuery.ToList();
                    if (capacityEntities.Count == 0)
                    {
                        capacityUsed = Storage.MapByteCount(0);
                    }
                    else
                    {
                        capacityEntities.Sort(OrderByLatest);
                        CapacityEntity latestAnalytics = capacityEntities.First();
                        capacityUsed = Storage.MapByteCount(latestAnalytics.Capacity);
                    }
                }
                catch
                {
                    capacityUsed = Constant.NotAvailable;
                }
            }
            return capacityUsed;
        }

        private static string GetAccountInfo(string authToken, IStorageAccount storageAccount)
        {
            User authUser = new User(authToken);
            string tenantId = authUser.MediaAccountDomainName;
            string clientId = authUser.MediaAccountClientId;
            string clientKey = authUser.MediaAccountClientKey;

            string accountType = GetAccountType(tenantId, clientId, clientKey, storageAccount.Name);
            string storageUsed = GetCapacityUsed(authToken, storageAccount.Name);
            return string.Concat(storageAccount.Name, ": ", accountType, ", ", storageUsed, ")");
        }

        private static int OrderByLatest(CapacityEntity leftSide, CapacityEntity rightSide)
        {
            return DateTimeOffset.Compare(rightSide.Time, leftSide.Time);
        }

        private static int GetAccountIndex(string[] accountNames, string accountName)
        {
            int accountIndex = 0;
            if (!string.IsNullOrEmpty(accountName))
            {
                for (int i = 0; i < accountNames.Length; i++)
                {
                    if (string.Equals(accountNames[i], accountName, StringComparison.OrdinalIgnoreCase))
                    {
                        accountIndex = i;
                    }
                }
            }
            return accountIndex;
        }

        internal static string GetAccountKey(string authToken, string accountName)
        {
            Storage.GetAccount(authToken, accountName, out string accountKey);
            return accountKey;
        }

        internal static CloudStorageAccount GetAccount(string authToken, string accountName, out string accountKey)
        {
            accountKey = string.Empty;
            User authUser = new User(authToken);
            string[] accountNames = authUser.StorageAccountNames;
            string[] accountKeys = authUser.StorageAccountKeys;
            int accountIndex = GetAccountIndex(accountNames, accountName);
            if (string.IsNullOrEmpty(accountName))
            {
                accountName = accountNames[accountIndex];
            }
            accountKey = accountKeys[accountIndex];
            string storageAccount = string.Format(Constant.Storage.Account.Connection, accountName, accountKeys[accountIndex]);
            return CloudStorageAccount.Parse(storageAccount);
        }

        internal static CloudStorageAccount GetSystemAccount()
        {
            string settingKey = Constant.AppSettingKey.AzureStorage;
            string storageAccount = AppSetting.GetValue(settingKey);
            return CloudStorageAccount.Parse(storageAccount);
        }

        internal static CloudStorageAccount GetUserAccount(string authToken, string accountName)
        {
            return GetAccount(authToken, accountName, out string accountKey);
        }

        internal static long GetAssetBytes(IAsset asset, out int fileCount)
        {
            fileCount = 0;
            long assetBytes = 0;
            foreach (IAssetFile file in asset.AssetFiles)
            {
                fileCount = fileCount + 1;
                assetBytes = assetBytes + file.ContentFileSize;
            }
            return assetBytes;
        }

        internal static string MapByteCount(long byteCount)
        {
            string mappedCount;
            if (byteCount >= 1099511627776)
            {
                mappedCount = (byteCount / 1099511627776.0).ToString(Constant.TextFormatter.Numeric) + " TB";
            }
            else if (byteCount >= 1073741824)
            {
                mappedCount = (byteCount / 1073741824.0).ToString(Constant.TextFormatter.Numeric) + " GB";
            }
            else if (byteCount >= 1048576)
            {
                mappedCount = (byteCount / 1048576.0).ToString(Constant.TextFormatter.Numeric) + " MB";
            }
            else if (byteCount >= 1024)
            {
                mappedCount = (byteCount / 1024.0).ToString(Constant.TextFormatter.Numeric) + " KB";
            }
            else if (byteCount == 1)
            {
                mappedCount = byteCount + " Byte";
            }
            else
            {
                mappedCount = byteCount + " Bytes";
            }
            return mappedCount;
        }

        public static void CreateContainer(string authToken, string storageAccount, string containerName)
        {
            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            CloudBlobContainer container = blobClient.GetContainer(containerName);
            container.CreateIfNotExists();
        }

        public static NameValueCollection GetAccounts(string authToken)
        {
            NameValueCollection storageAccounts = new NameValueCollection();
            MediaClient mediaClient = new MediaClient(authToken);
            IStorageAccount storageAccount = mediaClient.DefaultStorageAccount;
            string accountInfo = GetAccountInfo(authToken, storageAccount);
            storageAccounts.Add(accountInfo, storageAccount.Name);
            IStorageAccount[] accounts = mediaClient.GetEntities(MediaEntity.StorageAccount) as IStorageAccount[];
            foreach (IStorageAccount account in accounts)
            {
                if (!account.IsDefault)
                {
                    accountInfo = GetAccountInfo(authToken, account);
                    storageAccounts.Add(accountInfo, account.Name);
                }
            }
            return storageAccounts;
        }

        public static void UploadFile(string authToken, string storageAccount, string containerName,
                                      Stream inputStream, string fileName, int chunkIndex, int chunksCount)
        {
            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            if (chunksCount == 0)
            {
                blobClient.UploadFile(inputStream, containerName, null, fileName);
            }
            else
            {
                User authUser = new User(authToken);
                string partitionKey = authUser.Id;
                bool lastBlock = (chunkIndex == chunksCount - 1);
                blobClient.UploadBlock(inputStream, containerName, null, fileName, partitionKey, chunkIndex, lastBlock);
            }
        }
    }
}