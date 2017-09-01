using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureSkyMedia.PlatformServices
{
    internal class BlobClient
    {
        private CloudBlobClient _storage;
        private EntityClient _tracker;

        public BlobClient(string authToken, string accountName)
        {
            CloudStorageAccount storageAccount = Storage.GetUserAccount(authToken, accountName);
            _storage = storageAccount.CreateCloudBlobClient();
            _tracker = new EntityClient(authToken, accountName);
        }

        public BlobClient(string[] accountCredentials)
        {
            string accountName = accountCredentials[0];
            string accountKey = accountCredentials[1];
            StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
            _storage = storageAccount.CreateCloudBlobClient();
            _tracker = new EntityClient();
        }

        private CloudBlobDirectory GetDirectory(CloudBlobContainer container, string directoryPath)
        {
            if (directoryPath.StartsWith("/"))
            {
                directoryPath = directoryPath.Remove(0, 1);
            }
            return container.GetDirectoryReference(directoryPath);
        }

        private CloudBlobContainer GetContainer(string containerName, BlobContainerPublicAccessType publicAccess)
        {
            CloudBlobContainer container = _storage.GetContainerReference(containerName);
            if (container.CreateIfNotExists())
            {
                BlobContainerPermissions permissions = container.GetPermissions();
                permissions.PublicAccess = publicAccess;
                container.SetPermissions(permissions);
            }
            return container;
        }

        public CloudBlobContainer GetContainer(string containerName)
        {
            return GetContainer(containerName, BlobContainerPublicAccessType.Off);
        }

        public CloudBlockBlob GetBlob(string containerName, string directoryPath, string fileName, bool fetchAttributes)
        {
            CloudBlockBlob blob;
            CloudBlobContainer container = GetContainer(containerName);
            if (string.IsNullOrEmpty(directoryPath))
            {
                blob = container.GetBlockBlobReference(fileName);
            }
            else
            {
                CloudBlobDirectory directory = GetDirectory(container, directoryPath);
                blob = directory.GetBlockBlobReference(fileName);
            }
            if (fetchAttributes)
            {
                blob.FetchAttributes();
            }
            return blob;
        }

        public CloudBlockBlob GetBlob(string containerName, string directoryPath, string fileName)
        {
            return GetBlob(containerName, directoryPath, fileName, false);
        }

        public void UploadFile(Stream inputStream, string containerName, string directoryPath, string fileName)
        {
            CloudBlockBlob blob = GetBlob(containerName, directoryPath, fileName);
            blob.UploadFromStream(inputStream);
        }

        public void UploadBlock(Stream inputStream, string containerName, string directoryPath, string fileName,
                                string partitionKey, int blockIndex, bool lastBlock)
        {
            string blockId = Convert.ToBase64String(BitConverter.GetBytes(blockIndex));

            CloudBlockBlob blob = GetBlob(containerName, directoryPath, fileName);
            blob.PutBlock(blockId, inputStream, null);

            string tableName = Constant.Storage.TableName.FileUpload;
            string rowKey = blob.Name;

            if (blockIndex == 0)
            {
                BlockUpload blockUpload = new BlockUpload()
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    BlockIds = new string[] { blockId }
                };
                _tracker.UpsertEntity(tableName, blockUpload);
            }
            else
            {
                BlockUpload blockUpload = _tracker.GetEntity<BlockUpload>(tableName, partitionKey, rowKey);
                List<string> blockIds = new List<string>(blockUpload.BlockIds);
                blockIds.Add(blockId);
                blockUpload.BlockIds = blockIds.ToArray();
                _tracker.UpdateEntity(tableName, blockUpload);
            }

            if (lastBlock)
            {
                BlockUpload blockUpload = _tracker.GetEntity<BlockUpload>(tableName, partitionKey, rowKey);
                blob.PutBlockList(blockUpload.BlockIds);
                _tracker.DeleteEntity(tableName, blockUpload);
            }
        }
    }
}