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
            CloudStorageAccount storageAccount = Storage.GetAccount();
            _storage = storageAccount.CreateCloudBlobClient();
        }

        public BlobClient(MediaAccount mediaAccount, string accountName)
        {
            CloudStorageAccount storageAccount = Storage.GetAccount(mediaAccount, accountName);
            _storage = storageAccount.CreateCloudBlobClient();
        }

        private CloudBlobContainer GetBlobContainer(string containerName)
        {
            CloudBlobContainer container = _storage.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();
            return container;
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

        public CloudBlockBlob GetBlockBlob(string containerName, string directoryPath, string fileName)
        {
            return GetBlockBlob(containerName, directoryPath, fileName, false);
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

        public CloudAppendBlob GetAppendBlob(string containerName, string directoryPath, string fileName)
        {
            return GetAppendBlob(containerName, directoryPath, fileName, false);
        }

        public void UploadBlock(Stream blockStream, string containerName, string fileName, int blockIndex, int blocksCount, string contentType)
        {
            string blockId = Convert.ToBase64String(BitConverter.GetBytes(blockIndex));
            CloudBlockBlob blob = GetBlockBlob(containerName, null, fileName);
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