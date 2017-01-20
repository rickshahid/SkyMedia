using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Analytics;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.ServiceBroker
{
    public static class Storage
    {
        private static CloudStorageAccount GetAccount(string authToken, string accountName, out string accountKey)
        {
            accountKey = string.Empty;
            string storageAccount = string.Empty;
            string[] accountNames = AuthToken.GetClaimValues(authToken, Constants.UserAttribute.StorageAccountName);
            string[] accountKeys = AuthToken.GetClaimValues(authToken, Constants.UserAttribute.StorageAccountKey);
            if (string.IsNullOrEmpty(accountName))
            {
                accountKey = accountKeys[0];
                storageAccount = string.Format(Constants.Storage.Account.Connection, accountNames[0], accountKey);
            }
            else
            {
                for (int i = 0; i < accountNames.Length; i++)
                {
                    if (string.Equals(accountNames[i], accountName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        accountKey = accountKeys[i];
                        storageAccount = string.Format(Constants.Storage.Account.Connection, accountName, accountKeys[i]);
                    }
                }
            }
            return string.IsNullOrEmpty(storageAccount) ? null : CloudStorageAccount.Parse(storageAccount);
        }

        private static SharedAccessBlobPolicy GetBlobReadAccess(TimeSpan policyDuration, TimeSpan? startDelay)
        {
            SharedAccessBlobPolicy accessPolicy = new SharedAccessBlobPolicy();
            accessPolicy.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List;
            accessPolicy.SharedAccessExpiryTime = DateTime.UtcNow.Add(policyDuration);
            if (startDelay.HasValue)
            {
                accessPolicy.SharedAccessStartTime = DateTime.UtcNow.Add(startDelay.Value);
            }
            return accessPolicy;
        }

        private static int OrderByLatest(CapacityEntity leftSide, CapacityEntity rightSide)
        {
            return DateTimeOffset.Compare(rightSide.Time, leftSide.Time);
        }

        public static string GetCapacityUsed(string authToken, string accountName)
        {
            string capacityUsed = null;
            CloudStorageAccount storageAccount = GetAccount(authToken, accountName);
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
                        CapacityEntity latestUsage = capacityEntities.First();
                        capacityUsed = Storage.MapByteCount(latestUsage.Capacity);
                    }
                }
                catch
                {
                    capacityUsed = Constants.Storage.Analytics.NotAvailable;
                }
            }
            return capacityUsed;
        }

        public static string GetAccountKey(string authToken, string accountName)
        {
            string accountKey;
            GetAccount(authToken, accountName, out accountKey);
            return accountKey;
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

        internal static IRetryPolicy GetRetryPolicy()
        {
            return new ExponentialRetry();
        }

        internal static Uri GetAccessSignatureUri(CloudBlobContainer container, TimeSpan policyDuration)
        {
            SharedAccessBlobPolicy readAccess = GetBlobReadAccess(policyDuration, null);
            string accessSignature = container.GetSharedAccessSignature(readAccess);
            string signatureUrl = string.Concat(container.Uri.AbsoluteUri, accessSignature);
            return new Uri(signatureUrl);
        }

        internal static Uri GetAccessSignatureUri(ICloudBlob blob, TimeSpan policyDuration)
        {
            SharedAccessBlobPolicy readAccess = GetBlobReadAccess(policyDuration, null);
            string accessSignature = blob.GetSharedAccessSignature(readAccess);
            string signatureUrl = string.Concat(blob.Uri.AbsoluteUri, accessSignature);
            return new Uri(signatureUrl);
        }

        internal static CloudStorageAccount GetAccount(string authToken, string accountName)
        {
            string accountKey;
            return GetAccount(authToken, accountName, out accountKey);
        }

        internal static CloudStorageAccount GetAccount()
        {
            string settingKey = Constants.ConnectionStrings.AzureStorage;
            string storageAccount = AppSetting.GetValue(settingKey);
            return CloudStorageAccount.Parse(storageAccount);
        }
    }
}
