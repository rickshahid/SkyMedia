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
            CloudStorageAccount storageAccount = Storage.GetSystemAccount();
            _storage = storageAccount.CreateCloudBlobClient();
        }

        public BlobClient(string authToken, string accountName)
        {
            CloudStorageAccount storageAccount = Storage.GetUserAccount(authToken, accountName);
            _storage = storageAccount.CreateCloudBlobClient();
        }

        public CloudBlockBlob GetBlockBlob(string containerName, string directoryPath, string fileName, bool fetchAttributes)
        {
            CloudBlockBlob blob;
            CloudBlobContainer container = _storage.GetContainerReference(containerName);
            container.CreateIfNotExists();
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
                blob.FetchAttributes();
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
            CloudBlobContainer container = _storage.GetContainerReference(containerName);
            container.CreateIfNotExists();
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
                blob.FetchAttributes();
            }
            return blob;
        }

        public CloudAppendBlob GetAppendBlob(string containerName, string directoryPath, string fileName)
        {
            return GetAppendBlob(containerName, directoryPath, fileName, false);
        }

        public void UploadFile(Stream fileStream, string containerName, string directoryPath, string fileName)
        {
            CloudBlockBlob blob = GetBlockBlob(containerName, directoryPath, fileName);
            blob.UploadFromStream(fileStream);
        }

        public void UploadBlock(Stream fileStream, string containerName, string directoryPath, string fileName, int blockIndex, bool lastBlock)
        {
            string blockId = Convert.ToBase64String(BitConverter.GetBytes(blockIndex));
            CloudBlockBlob blob = GetBlockBlob(containerName, directoryPath, fileName);
            blob.PutBlock(blockId, fileStream, null);
            if (lastBlock)
            {
                IEnumerable<ListBlockItem> blockList = blob.DownloadBlockList(BlockListingFilter.Uncommitted);
                List<string> blockIds = new List<string>();
                foreach (ListBlockItem blockItem in blockList)
                {
                    blockIds.Add(blockItem.Name);
                }
                blob.PutBlockList(blockIds);
            }
        }
    }
}