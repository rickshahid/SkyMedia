using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal class BlobClient
    {
        private CloudBlobClient _storage;
        private EntityClient _tracker;

        public BlobClient(string authToken)
        {
            CloudStorageAccount storageAccount = Storage.GetAccount(authToken, null);
            BindContext(storageAccount);
            _tracker = new EntityClient(authToken);
        }

        public BlobClient(string authToken, string accountName)
        {
            CloudStorageAccount storageAccount = Storage.GetAccount(authToken, accountName);
            BindContext(storageAccount);
            _tracker = new EntityClient(authToken, accountName);
        }

        public BlobClient(ContentPublish contentPublish)
        {
            StorageCredentials storageCredentials = new StorageCredentials(contentPublish.StorageAccountName, contentPublish.StorageAccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
            BindContext(storageAccount);
        }

        private void BindContext(CloudStorageAccount storageAccount)
        {
            _storage = storageAccount.CreateCloudBlobClient();
            _storage.DefaultRequestOptions.RetryPolicy = Storage.GetRetryPolicy();

            string settingKey = Constants.AppSettings.StorageServerTimeoutSeconds;
            string serverTimeoutSeconds = AppSetting.GetValue(settingKey);

            settingKey = Constants.AppSettings.StorageMaxExecutionTimeSeconds;
            string maxExecutionTimeSeconds = AppSetting.GetValue(settingKey);

            settingKey = Constants.AppSettings.StorageMaxSingleBlobUploadBytes;
            string maxSingleBlobUploadBytes = AppSetting.GetValue(settingKey);

            settingKey = Constants.AppSettings.StorageParallelOperationThreadCount;
            string parallelOperationThreadCount = AppSetting.GetValue(settingKey);

            int settingValueInt;
            if (int.TryParse(serverTimeoutSeconds, out settingValueInt))
            {
                _storage.DefaultRequestOptions.ServerTimeout = new TimeSpan(0, 0, settingValueInt);
            }

            if (int.TryParse(maxExecutionTimeSeconds, out settingValueInt))
            {
                _storage.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan(0, 0, settingValueInt);
            }

            long settingValueLong;
            if (long.TryParse(maxSingleBlobUploadBytes, out settingValueLong))
            {
                _storage.DefaultRequestOptions.SingleBlobUploadThresholdInBytes = settingValueLong;
            }

            if (int.TryParse(parallelOperationThreadCount, out settingValueInt))
            {
                _storage.DefaultRequestOptions.ParallelOperationThreadCount = settingValueInt;
            }
        }

        private CloudBlobContainer GetContainer(string containerName, BlobContainerPublicAccessType? publicAccess)
        {
            CloudBlobContainer container = _storage.GetContainerReference(containerName);
            container.CreateIfNotExists();
            if (publicAccess.HasValue)
            {
                BlobContainerPermissions permissions = container.GetPermissions();
                permissions.PublicAccess = publicAccess.Value;
                container.SetPermissions(permissions);
            }
            return container;
        }

        public CloudBlobContainer GetContainer(string containerName)
        {
            return GetContainer(containerName, null);
        }

        private CloudBlobDirectory GetDirectory(CloudBlobContainer container, string directoryPath)
        {
            if (directoryPath.StartsWith("/"))
            {
                directoryPath = directoryPath.Remove(0, 1);
            }
            return container.GetDirectoryReference(directoryPath);
        }

        public IEnumerable<IListBlobItem> ListItems(string containerName, string directoryPath, bool flatListing, BlobListingDetails listingDetails)
        {
            IEnumerable<IListBlobItem> blobs;
            CloudBlobContainer container = GetContainer(containerName);
            if (string.IsNullOrEmpty(directoryPath))
            {
                blobs = container.ListBlobs(null, flatListing, listingDetails);
            }
            else
            {
                CloudBlobDirectory directory = GetDirectory(container, directoryPath);
                blobs = directory.ListBlobs(flatListing, listingDetails);
            }
            return blobs;
        }

        public IEnumerable<IListBlobItem> ListItems(string containerName, string directoryPath)
        {
            return ListItems(containerName, directoryPath, false, BlobListingDetails.Metadata);
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

        public string CopyBlob(CloudBlockBlob sourceBlob, CloudBlockBlob destinationBlob, bool asyncMode)
        {
            TimeSpan expirationTime = TimeSpan.FromMinutes(Constants.Storage.Blob.MaxCopyMinutes);
            Uri sourceBlobUri = Storage.GetAccessSignatureUri(sourceBlob, expirationTime);
            string operationId;
            if (asyncMode)
            {
                Task<string> operation = destinationBlob.StartCopyAsync(sourceBlobUri);
                operationId = operation.Id.ToString();
            }
            else
            {
                operationId = destinationBlob.StartCopy(sourceBlobUri);
            }
            return operationId;
        }

        public string MoveBlob(CloudBlockBlob sourceBlob, CloudBlockBlob destinationBlob)
        {
            string operationId = CopyBlob(sourceBlob, destinationBlob, false);
            sourceBlob.Delete();
            return operationId;
        }

        public string CopyFile(IAsset sourceAsset, IAsset destinationAsset, string sourceFileName, string destinationFileName, bool primaryFile)
        {
            string sourceContainerName = sourceAsset.Uri.Segments[1];
            CloudBlockBlob sourceBlob = GetBlob(sourceContainerName, string.Empty, sourceFileName, true);
            string destinationContainerName = destinationAsset.Uri.Segments[1];
            CloudBlockBlob destinationBlob = GetBlob(destinationContainerName, string.Empty, destinationFileName, false);
            string operationId = CopyBlob(sourceBlob, destinationBlob, false);
            IAssetFile assetFile = destinationAsset.AssetFiles.Create(destinationFileName);
            assetFile.ContentFileSize = sourceBlob.Properties.Length;
            assetFile.MimeType = sourceBlob.Properties.ContentType;
            assetFile.IsPrimary = primaryFile;
            assetFile.Update();
            return operationId;
        }

        public void UploadFile(Stream inputStream, string containerName, string directoryPath, string fileName)
        {
            CloudBlockBlob blob = GetBlob(containerName, directoryPath, fileName);
            blob.UploadFromStream(inputStream);
        }

        public void UploadBlock(string userId, Stream inputStream, string containerName, string directoryPath,
                                string fileName, int blockIndex, bool lastBlock)
        {
            string blockId = Convert.ToBase64String(BitConverter.GetBytes(blockIndex));

            CloudBlockBlob blob = GetBlob(containerName, directoryPath, fileName);
            blob.PutBlock(blockId, inputStream, null);

            string tableName = Constants.Storage.TableNames.FileUpload;
            string partitionKey = userId;
            string rowKey = blob.Name;

            if (blockIndex == 0)
            {
                BlockUpload blockUpload = new BlockUpload();
                blockUpload.PartitionKey = partitionKey;
                blockUpload.RowKey = rowKey;
                blockUpload.BlockIds = new string[] { blockId };
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
