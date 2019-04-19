using System;
using System.IO;
using System.Web;
using System.Collections.Generic;

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace AzureSkyMedia.PlatformServices
{
    internal class StorageBlobClient
    {
        private CloudBlobClient _blobClient;

        public StorageBlobClient()
        {
            CloudStorageAccount storageAccount = Account.GetStorageAccount();
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public StorageBlobClient(MediaAccount mediaAccount, string accountName)
        {
            CloudStorageAccount storageAccount = Account.GetStorageAccount(mediaAccount, accountName);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        private CloudBlobContainer GetBlobContainer(string containerName)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();
            return container;
        }

        public CloudBlob[] ListBlobContainer(string containerName, string directoryPath)
        {
            List<CloudBlob> blobs = new List<CloudBlob>();
            BlobContinuationToken continuationToken = null;
            CloudBlobContainer blobContainer = GetBlobContainer(containerName);
            do
            {
                BlobResultSegment resultSegment;
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    CloudBlobDirectory blobDirectory = blobContainer.GetDirectoryReference(directoryPath);
                    resultSegment = blobDirectory.ListBlobsSegmentedAsync(continuationToken).Result;
                }
                else
                {
                    resultSegment = blobContainer.ListBlobsSegmentedAsync(continuationToken).Result;
                }
                foreach (IListBlobItem blobItem in resultSegment.Results)
                {
                    CloudBlob blob = (CloudBlob)blobItem;
                    blobs.Add(blob);
                }
                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);
            return blobs.ToArray();
        }

        public CloudBlockBlob GetBlockBlob(string containerName, string directoryPath, string fileName, bool fetchAttributes)
        {
            CloudBlockBlob blob;
            CloudBlobContainer container = GetBlobContainer(containerName);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                CloudBlobDirectory directory = container.GetDirectoryReference(directoryPath);
                blob = directory.GetBlockBlobReference(fileName);
            }
            else
            {
                blob = container.GetBlockBlobReference(fileName);
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

        public string GetBlobSize(string containerName, string directoryPath, string fileName, out long byteCount, out string contentType)
        {
            CloudBlockBlob blob = GetBlockBlob(containerName, directoryPath, fileName, true);
            byteCount = blob.Properties.Length;
            contentType = blob.Properties.ContentType;
            return MapByteCount(byteCount);
        }

        public string GetDownloadUrl(string containerName, string fileName)
        {
            CloudBlockBlob blob = GetBlockBlob(containerName, null, fileName);
            string settingKey = Constant.AppSettingKey.StorageSharedAccessMinutes;
            string sharedAccessMinutes = AppSetting.GetValue(settingKey);
            SharedAccessBlobPolicy accessPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(double.Parse(sharedAccessMinutes)),
                Permissions = SharedAccessBlobPermissions.Read
            };
            string accessSignature = blob.GetSharedAccessSignature(accessPolicy);
            fileName = HttpUtility.UrlPathEncode(fileName);
            accessSignature = HttpUtility.UrlPathEncode(accessSignature);
            return string.Concat(blob.Container.Uri.ToString(), "/", fileName, accessSignature);
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
    }
}