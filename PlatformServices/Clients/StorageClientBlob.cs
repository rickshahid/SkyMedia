using System;
using System.IO;
using System.Web;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

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

        public CloudAppendBlob GetAppendBlob(string containerName, string fileName)
        {
            return GetAppendBlob(containerName, string.Empty, fileName, false);
        }

        public string GetBlobSize(string containerName, string directoryPath, string fileName, out long byteCount, out string contentType)
        {
            CloudBlockBlob blob = GetBlockBlob(containerName, directoryPath, fileName, true);
            byteCount = blob.Properties.Length;
            contentType = blob.Properties.ContentType;
            return MapByteCount(byteCount);
        }

        public string GetDownloadUrl(string containerName, string fileName, bool readWrite)
        {
            CloudBlockBlob blockBlob = GetBlockBlob(containerName, null, fileName);
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
            fileName = HttpUtility.UrlPathEncode(fileName);
            accessSignature = HttpUtility.UrlPathEncode(accessSignature);
            return string.Concat(blockBlob.Container.Uri.ToString(), "/", fileName, accessSignature);
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

        //static public async void CopyBlobsAsync(CloudBlobContainer sourceBlobContainer, CloudBlobContainer destinationBlobContainer, List<string> fileNames)
        //{
        //    if (fileNames != null)
        //    {
        //        foreach (var fileName in fileNames)
        //        {
        //            CloudBlob sourceBlob = sourceBlobContainer.GetBlockBlobReference(fileName);
        //            CloudBlob destinationBlob = destinationBlobContainer.GetBlockBlobReference(fileName);
        //            CopyBlobAsync(sourceBlob as CloudBlob, destinationBlob);
        //        }
        //    }
        //    else
        //    {
        //        string blobPrefix = null;
        //        BlobContinuationToken blobContinuationToken = null;
        //        do
        //        {
        //            var results = await sourceBlobContainer.ListBlobsSegmentedAsync(blobPrefix, blobContinuationToken);
        //            Get the value of the continuation token returned by the listing call.
        //            blobContinuationToken = results.ContinuationToken;
        //            foreach (IListBlobItem item in results.Results)
        //            {
        //                if (item.GetType() == typeof(CloudBlockBlob))
        //                {
        //                    CloudBlockBlob sourceBlob = (CloudBlockBlob)item;
        //                    CloudBlob destinationBlob = destinationBlobContainer.GetBlockBlobReference(sourceBlob.Name);
        //                    CopyBlobAsync(sourceBlob as CloudBlob, destinationBlob);
        //                }
        //            }
        //        } while (blobContinuationToken != null); // Loop while the continuation token is not null.
        //    }
        //}

        //static public async void CopyBlobAsync(CloudBlob sourceBlob, CloudBlob destinationBlob)
        //{
        //    var signature = sourceBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
        //    {
        //        Permissions = SharedAccessBlobPermissions.Read,
        //        SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
        //    });
        //    await destinationBlob.StartCopyAsync(new Uri(sourceBlob.Uri.AbsoluteUri + signature));
        //}
    }
}