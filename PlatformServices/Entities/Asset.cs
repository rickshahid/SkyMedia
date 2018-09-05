using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaAsset : Asset
    {
        internal MediaAsset(MediaAccount mediaAccount, Asset asset) : base(asset.Id, asset.Name, asset.Type, asset.AssetId, asset.Created, asset.LastModified, asset.AlternateId, asset.Description, asset.Container, asset.StorageAccountName, asset.StorageEncryptionFormat)
        {
            StorageBlobClient blobClient = new StorageBlobClient(mediaAccount, asset.StorageAccountName);
            Files = GetAssetFiles(blobClient, asset.Container, null);
        }

        internal static string GetAssetName(StorageBlobClient blobClient, string containerName, string directoryPath)
        {
            string assetName = null;
            MediaFile[] assetFiles = GetAssetFiles(blobClient, containerName, directoryPath);
            if (assetFiles.Length == 1)
            {
                assetName = assetFiles[0].Name;
            }
            else
            {
                foreach (MediaFile assetFile in assetFiles)
                {
                    if (assetFile.Name.EndsWith(Constant.Media.Stream.ManifestExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        assetName = assetFile.Name;
                    }
                }
            }
            if (!string.IsNullOrEmpty(assetName))
            {
                assetName = Path.GetFileNameWithoutExtension(assetName);
            }
            return assetName;
        }

        internal static MediaFile[] GetAssetFiles(StorageBlobClient blobClient, string containerName, string directoryPath)
        {
            BlobContinuationToken continuationToken = null;
            List<MediaFile> files = new List<MediaFile>();
            CloudBlobContainer blobContainer = blobClient.GetBlobContainer(containerName);
            do
            {
                BlobResultSegment blobList;
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    CloudBlobDirectory blobDirectory = blobContainer.GetDirectoryReference(directoryPath);
                    blobList = blobDirectory.ListBlobsSegmentedAsync(continuationToken).Result;
                }
                else
                {
                    blobList = blobContainer.ListBlobsSegmentedAsync(continuationToken).Result;
                }
                foreach (IListBlobItem blobItem in blobList.Results)
                {
                    string fileName = Path.GetFileName(blobItem.Uri.ToString());
                    string fileSize = blobClient.GetBlobSize(containerName, fileName, out long byteCount);
                    MediaFile file = new MediaFile()
                    {
                        Name = fileName,
                        Size = fileSize,
                        ByteCount = byteCount,
                        DownloadUrl = blobClient.GetDownloadUrl(containerName, fileName, false)
                    };
                    files.Add(file);
                }
                continuationToken = blobList.ContinuationToken;
            } while (continuationToken != null);
            return files.ToArray();
        }

        public MediaFile[] Files { get; internal set; }

        public string Size
        {
            get
            {
                long byteCount = 0;
                foreach (MediaFile assetFile in Files)
                {
                    byteCount = byteCount + assetFile.ByteCount;
                }
                return StorageBlobClient.MapByteCount(byteCount);
            }
        }
    }

    public class MediaFile
    {
        public string Name { get; set; }

        public string Size { get; set; }

        public long ByteCount { get; set; }

        public string DownloadUrl { get; set; }
    }
}