﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Rest;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;

using MediaStorageAccount = Microsoft.Azure.Management.Media.Models.StorageAccount;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Storage
    {
        private static string GetAccountInfo(string authToken, string accountName)
        {
            TokenCredentials azureToken = AuthToken.AcquireToken(authToken, out string subscriptionId);
            StorageManagementClient storageClient = new StorageManagementClient(azureToken)
            {
                SubscriptionId = subscriptionId
            };

            IEnumerable<StorageAccount> storageAccounts = storageClient.StorageAccounts.List();
            storageAccounts = storageAccounts.Where(s => s.Name.Equals(accountName, StringComparison.OrdinalIgnoreCase));
            StorageAccount storageAccount = storageAccounts.SingleOrDefault();

            string accountType = "N/A";
            string replicationType = "N/A";
            if (storageAccount != null)
            {
                switch (storageAccount.Kind)
                {
                    case Kind.Storage:
                        accountType = "General v1";
                        break;
                    case Kind.StorageV2:
                        accountType = "General v2";
                        break;
                    case Kind.BlobStorage:
                        accountType = string.Concat("Blob ", storageAccount.AccessTier.ToString());
                        break;
                }
                replicationType = storageAccount.Sku.Name.ToString();
                replicationType = Regex.Replace(replicationType, Constant.TextFormatter.SpacePattern, Constant.TextFormatter.SpaceReplacement);
                replicationType = string.Concat(replicationType, " (", storageAccount.PrimaryLocation, ")");
            }

            string accountInfo = string.Concat("Account: ", accountName);
            accountInfo = string.Concat(accountInfo, ", Type: ", accountType);
            return string.Concat(accountInfo, ", Replication: ", replicationType);
        }

        public static Dictionary<string, string> GetAccounts(string authToken)
        {
            Dictionary<string, string> storageAccounts = new Dictionary<string, string>();
            MediaClient mediaClient = new MediaClient(authToken);
            IList<MediaStorageAccount> mediaStorageAccounts = mediaClient.StorageAccounts;
            foreach (MediaStorageAccount mediaStorageAccount in mediaStorageAccounts)
            {
                string accountName = Path.GetFileName(mediaStorageAccount.Id);
                string accountInfo = GetAccountInfo(authToken, accountName);
                storageAccounts.Add(accountName, accountInfo);
            }
            return storageAccounts;
        }

        public static CloudStorageAccount GetAccount(MediaAccount mediaAccount, string accountName)
        {
            CloudStorageAccount storageAccount;
            if (mediaAccount == null)
            {
                string settingKey = Constant.AppSettingKey.AzureStorage;
                string systemStorage = AppSetting.GetValue(settingKey);
                storageAccount = CloudStorageAccount.Parse(systemStorage);
            }
            else
            {
                string accountKey = mediaAccount.StorageAccounts[accountName];
                StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);
                storageAccount = new CloudStorageAccount(storageCredentials, true);
            }
            return storageAccount;
        }

        public static CloudStorageAccount GetAccount()
        {
            return GetAccount(null, null);
        }

        public static void UploadBlock(string authToken, string storageAccount, string containerName, Stream blockStream,
                                       string fileName, int chunkIndex, int chunksCount, string contentType)
        {
            User authUser = new User(authToken);
            BlobClient blobClient = new BlobClient(authUser.MediaAccount, storageAccount);
            blobClient.UploadBlock(blockStream, containerName, fileName, chunkIndex, chunksCount, contentType);
        }
    }
}