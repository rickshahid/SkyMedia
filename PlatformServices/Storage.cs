using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Storage
    {
        //    if (storageAccount == null)
        //    {
        //        accountInfo = string.Concat(accountInfo, Constant.Message.StorageAccountReadPermission);
        //    }
        //    else
        //    {
        //        string accountType = "N/A";
        //        if (storageAccount.Kind.HasValue)
        //        {
        //            switch (storageAccount.Kind.Value)
        //            {
        //                case Kind.Storage:
        //                    accountType = "General v1";
        //                    break;
        //                case Kind.StorageV2:
        //                    accountType = "General v2";
        //                    break;
        //                case Kind.BlobStorage:
        //                    accountType = "Blob";
        //                    if (storageAccount.AccessTier.HasValue)
        //                    {
        //                        accountType = string.Concat(accountType, " ", storageAccount.AccessTier.Value.ToString());
        //                    }
        //                    break;
        //            }
        //        }

        //        string accountEncryption = "Not Enabled";
        //        if (storageAccount.Encryption.Services.Blob.Enabled.HasValue && storageAccount.Encryption.Services.Blob.Enabled.Value)
        //        {
        //            accountEncryption = "Enabled";
        //        }

        //        string accountReplication = storageAccount.Sku.Name.ToString();
        //        accountReplication = Constant.TextFormatter.GetValue(accountReplication);

        //        accountInfo = string.Concat(accountInfo, " (Type: ", accountType);
        //        accountInfo = string.Concat(accountInfo, ", Encryption: ", accountEncryption);
        //        accountInfo = string.Concat(accountInfo, ", Replication: ", accountReplication);
        //        accountInfo = string.Concat(accountInfo, ", Primary: ", storageAccount.PrimaryLocation.ToUpperInvariant());
        //        accountInfo = string.Concat(accountInfo, ", Secondary: ", string.IsNullOrEmpty(storageAccount.SecondaryLocation) ? "N/A" : storageAccount.SecondaryLocation.ToUpperInvariant(), ")");
        //    }
        //    return accountInfo;
        //}

        public static Dictionary<string, string> GetAccounts(string authToken)
        {
            Dictionary<string, string> storageAccounts = new Dictionary<string, string>();
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                IList<StorageAccount> mediaStorageAccounts = mediaClient.StorageAccounts;
                foreach (StorageAccount mediaStorageAccount in mediaStorageAccounts)
                {
                    MediaStorage mediaStorage = new MediaStorage(authToken, mediaStorageAccount);
                    string accountName = Path.GetFileName(mediaStorageAccount.Id);
                    string accountInfo = string.Concat(accountName, mediaStorage.ToString());
                    storageAccounts.Add(accountName, accountInfo);
                }
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