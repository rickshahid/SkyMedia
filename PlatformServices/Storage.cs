using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Analytics;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Storage
    {
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
                    capacityUsed = Constants.NotAvailable;
                }
            }
            return capacityUsed;
        }

        private static string GetAccountInfo(string authToken, IStorageAccount storageAccount)
        {
            string accountInfo = string.Concat("Account: ", storageAccount.Name);
            string storageUsed = GetCapacityUsed(authToken, storageAccount.Name);
            if (!string.IsNullOrEmpty(storageUsed))
            {
                accountInfo = string.Concat(accountInfo, ", Usage: ", storageUsed, ")");
            }
            return accountInfo;
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
                    if (string.Equals(accountNames[i], accountName, StringComparison.InvariantCultureIgnoreCase))
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
            string storageAccount = string.Empty;
            string[] accountNames = AuthToken.GetClaimValues(authToken, Constants.UserAttribute.StorageAccountName);
            string[] accountKeys = AuthToken.GetClaimValues(authToken, Constants.UserAttribute.StorageAccountKey);
            int accountIndex = GetAccountIndex(accountNames, accountName);
            accountKey = accountKeys[accountIndex];
            storageAccount = string.Format(Constants.Storage.Account.Connection, accountName, accountKeys[accountIndex]);
            return string.IsNullOrEmpty(storageAccount) ? null : CloudStorageAccount.Parse(storageAccount);
        }

        internal static Uri GetAccessSignature(ICloudBlob storageBlob, TimeSpan policyDuration, bool readWrite)
        {
            SharedAccessBlobPolicy accessPolicy = new SharedAccessBlobPolicy();
            accessPolicy.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List;
            if (readWrite)
            {
                accessPolicy.Permissions = accessPolicy.Permissions | SharedAccessBlobPermissions.Write;
            }
            accessPolicy.SharedAccessExpiryTime = DateTime.UtcNow.Add(policyDuration);
            string accessSignature = storageBlob.GetSharedAccessSignature(accessPolicy);
            string signatureUrl = string.Concat(storageBlob.Uri.AbsoluteUri, accessSignature);
            return new Uri(signatureUrl);
        }

        internal static CloudStorageAccount GetSystemAccount()
        {
            string settingKey = Constants.AppSettingKey.AzureStorage;
            string storageAccount = AppSetting.GetValue(settingKey);
            return CloudStorageAccount.Parse(storageAccount);
        }

        internal static CloudStorageAccount GetUserAccount(string authToken, string accountName)
        {
            string accountKey;
            return GetAccount(authToken, accountName, out accountKey);
        }

        internal static string MapByteCount(long byteCount)
        {
            string mappedCount;
            if (byteCount >= 1099511627776)
            {
                mappedCount = (byteCount / 1099511627776.0).ToString(Constants.FormatNumber) + " TB";
            }
            else if (byteCount >= 1073741824)
            {
                mappedCount = (byteCount / 1073741824.0).ToString(Constants.FormatNumber) + " GB";
            }
            else if (byteCount >= 1048576)
            {
                mappedCount = (byteCount / 1048576.0).ToString(Constants.FormatNumber) + " MB";
            }
            else if (byteCount >= 1024)
            {
                mappedCount = (byteCount / 1024.0).ToString(Constants.FormatNumber) + " KB";
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

        public static string GetUserAccountKey(string authToken, string accountName)
        {
            string accountKey;
            GetAccount(authToken, accountName, out accountKey);
            return accountKey;
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
    }
}
