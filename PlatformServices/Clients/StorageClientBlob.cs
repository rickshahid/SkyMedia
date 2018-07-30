using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureSkyMedia.PlatformServices
{
    internal class BlobClient
    {
        private CloudBlobClient _storage;

        public BlobClient()
        {
            CloudStorageAccount storageAccount = Account.GetStorageAccount();
            _storage = storageAccount.CreateCloudBlobClient();
        }

        public BlobClient(MediaAccount mediaAccount, string accountName)
        {
            CloudStorageAccount storageAccount = Account.GetStorageAccount(mediaAccount, accountName);
            _storage = storageAccount.CreateCloudBlobClient();
        }

        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            CloudBlobContainer container = _storage.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();
            return container;
        }

        public CloudBlobDirectory GetBlobDirectory(string containerName, string directoryPath)
        {
            CloudBlobContainer container = GetBlobContainer(containerName);
            return container.GetDirectoryReference(directoryPath);
        }

        public CloudBlockBlob GetBlockBlob(string containerName, string directoryPath, string fileName, bool fetchAttributes)
        {
            CloudBlockBlob blob;
            CloudBlobContainer container = GetBlobContainer(containerName);
            if (string.IsNullOrEmpty(directoryPath))
            {
                blob = container.GetBlockBlobReference(fileName);
            }
            else
            {
                CloudBlobDirectory directory = container.GetDirectoryReference(directoryPath);
                blob = directory.GetBlockBlobReference(fileName);
            }
            if (fetchAttributes)
            {
                blob.FetchAttributesAsync().Wait();
            }
            return blob;
        }

        public CloudBlockBlob GetBlockBlob(string containerName, string fileName)
        {
            return GetBlockBlob(containerName, string.Empty, fileName, false);
        }

        public CloudAppendBlob GetAppendBlob(string containerName, string directoryPath, string fileName, bool fetchAttributes)
        {
            CloudAppendBlob blob;
            CloudBlobContainer container = GetBlobContainer(containerName);
            if (string.IsNullOrEmpty(directoryPath))
            {
                blob = container.GetAppendBlobReference(fileName);
            }
            else
            {
                CloudBlobDirectory directory = container.GetDirectoryReference(directoryPath);
                blob = directory.GetAppendBlobReference(fileName);
            }
            if (fetchAttributes)
            {
                blob.FetchAttributesAsync().Wait();
            }
            return blob;
        }

        public CloudAppendBlob GetAppendBlob(string containerName, string fileName)
        {
            return GetAppendBlob(containerName, string.Empty, fileName, false);
        }

        public string GetDownloadUrl(string containerName, string fileName, bool readWrite)
        {
            CloudBlockBlob blockBlob = GetBlockBlob(containerName, fileName);
            string settingKey = Constant.AppSettingKey.StorageSharedAccessMinutes;
            string sharedAccessMinutes = AppSetting.GetValue(settingKey);
            SharedAccessBlobPolicy accessPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(double.Parse(sharedAccessMinutes)),
                Permissions = SharedAccessBlobPermissions.Read
            };
            if (readWrite)
            {
                accessPolicy.Permissions = accessPolicy.Permissions | SharedAccessBlobPermissions.Write;
            }
            string accessSignature = blockBlob.GetSharedAccessSignature(accessPolicy);
            return string.Concat(blockBlob.Uri.ToString(), accessSignature);
        }

        public void UploadBlock(Stream blockStream, string containerName, string fileName, int blockIndex, int blocksCount, string contentType)
        {
            string blockId = Convert.ToBase64String(BitConverter.GetBytes(blockIndex));
            CloudBlockBlob blob = GetBlockBlob(containerName, fileName);
            blob.PutBlockAsync(blockId, blockStream, null).Wait();
            if (blockIndex == blocksCount - 1)
            {
                BlockListingFilter blockType = BlockListingFilter.Uncommitted;
                IEnumerable<ListBlockItem> blockList = blob.DownloadBlockListAsync(blockType, null, null, null).Result;
                List<string> blockIds = new List<string>();
                foreach (ListBlockItem blockItem in blockList)
                {
                    blockIds.Add(blockItem.Name);
                }
                blob.PutBlockListAsync(blockIds).Wait();
                blob.Properties.ContentType = contentType;
                blob.SetPropertiesAsync().Wait();
            }
        }
    }
}