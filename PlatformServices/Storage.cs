using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.Rest;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Analytics;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

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
        private static int OrderByLatest(CapacityEntity leftItem, CapacityEntity rightItem)
        {
            return DateTimeOffset.Compare(rightItem.Time, leftItem.Time);
        }

        private static string GetAccountType(string authToken, string accountName)
        {
            string accountType = string.Empty;
            string settingKey = Constant.AppSettingKey.AppSubscriptionId;
            string subscriptionId = AppSetting.GetValue(settingKey);
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                User authUser = new User(authToken);
                string mediaAccountDomain = authUser.MediaAccountDomainName;
                string mediaClientId = authUser.MediaAccountClientId;
                string mediaClientKey = authUser.MediaAccountClientKey;

                settingKey = Constant.AppSettingKey.MediaLoginUrl;
                string mediaLoginUrl = AppSetting.GetValue(settingKey);

                string mediaAuthority = string.Concat(mediaLoginUrl, mediaAccountDomain);
                AuthenticationContext authContext = new AuthenticationContext(mediaAuthority);

                settingKey = Constant.AppSettingKey.StorageManagementUrl;
                string storageManagementUrl = AppSetting.GetValue(settingKey);

                ClientCredential mediaClientCredential = new ClientCredential(mediaClientId, mediaClientKey);

                AuthenticationResult authResult = authContext.AcquireTokenAsync(storageManagementUrl, mediaClientCredential).Result;
                TokenCredentials tokenCredential = new TokenCredentials(authResult.AccessToken);

                StorageManagementClient storageClient = new StorageManagementClient(tokenCredential);
                storageClient.SubscriptionId = subscriptionId;

                IEnumerable<StorageAccount> storageAccounts = storageClient.StorageAccounts.List();
                StorageAccount storageAccount = storageAccounts.Where(sa => sa.Name == accountName).SingleOrDefault();

                if (storageAccount != null)
                {
                    switch (storageAccount.Kind)
                    {
                        case Kind.Storage:
                            accountType = "General";
                            break;
                        case Kind.BlobStorage:
                            accountType = string.Concat("Blob ", storageAccount.AccessTier.ToString());
                            break;
                    }
                }
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
                TableQuery<CapacityEntity> capacityQuery = storageAnalytics.CreateCapacityQuery();
                IQueryable<CapacityEntity> capacityData = capacityQuery.Where(x => x.RowKey == Constant.Storage.Table.Key.CapacityData);
                try
                {
                    List<CapacityEntity> capacities = capacityData.ToList();
                    if (capacities.Count == 0)
                    {
                        capacityUsed = Storage.MapByteCount(0);
                    }
                    else
                    {
                        capacities.Sort(OrderByLatest);
                        long latestCapacity = capacities.First().Capacity;
                        capacityUsed = Storage.MapByteCount(latestCapacity);
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
            string accountInfo = string.Concat("Account Name: ", storageAccount.Name);
            string accountType = GetAccountType(authToken, storageAccount.Name);
            if (!string.IsNullOrEmpty(accountType))
            {
                accountInfo = string.Concat(accountInfo, ", Account Type: ", accountType);
            }
            string capacityUsed = GetCapacityUsed(authToken, storageAccount.Name);
            if (!string.IsNullOrEmpty(capacityUsed))
            {
                accountInfo = string.Concat(accountInfo, ", Capacity Used: ", capacityUsed);
            }
            return accountInfo;
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

        private static CloudStorageAccount GetAccount(string authToken, string accountName, out string accountKey)
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

        public static CloudStorageAccount GetSystemAccount()
        {
            string settingKey = Constant.AppSettingKey.AzureStorage;
            string storageAccount = AppSetting.GetValue(settingKey);
            return CloudStorageAccount.Parse(storageAccount);
        }

        public static CloudStorageAccount GetUserAccount(string authToken, string accountName)
        {
            return GetAccount(authToken, accountName, out string accountKey);
        }

        public static string GetAccountKey(string authToken, string accountName)
        {
            Storage.GetAccount(authToken, accountName, out string accountKey);
            return accountKey;
        }

        public static long GetAssetBytes(IAsset asset, out int fileCount)
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

        public static string MapByteCount(long byteCount)
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

        public static NameValueCollection GetAccounts(string authToken)
        {
            NameValueCollection storageAccounts = new NameValueCollection();
            MediaClient mediaClient = new MediaClient(authToken);
            IStorageAccount defaultStorage = mediaClient.DefaultStorageAccount;
            string accountInfo = GetAccountInfo(authToken, defaultStorage);
            storageAccounts.Add(accountInfo, defaultStorage.Name);
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
                                      Stream readStream, string fileName, int chunkIndex, int chunksCount)
        {
            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            if (chunksCount == 0)
            {
                blobClient.UploadFile(readStream, containerName, null, fileName);
            }
            else
            {
                User authUser = new User(authToken);
                string partitionKey = authUser.Id;
                bool lastBlock = (chunkIndex == chunksCount - 1);
                blobClient.UploadBlock(readStream, containerName, null, fileName, partitionKey, chunkIndex, lastBlock);
            }
        }
    }
}