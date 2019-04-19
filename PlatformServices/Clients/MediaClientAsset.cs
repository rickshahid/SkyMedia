using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private Asset CreateAsset(string storageAccount, string assetName, string assetDescription, string assetAlternateId)
        {
            Asset asset = new Asset(name: assetName)
            {
                StorageAccountName = storageAccount,
                Description = assetDescription,
                AlternateId = assetAlternateId
            };
            asset = _media.Assets.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, asset.Name, asset);
            asset.Container = string.Concat(Constant.Media.Asset.ContainerPrefix, asset.AssetId);
            return asset;
        }

        public Asset CreateAsset(string storageAccount, string assetName)
        {
            return CreateAsset(storageAccount, assetName, string.Empty, string.Empty);
        }

        public async Task<Asset> CreateAsset(string storageAccount, string assetName, string fileName, Stream fileStream)
        {
            StorageBlobClient blobClient = new StorageBlobClient(this.MediaAccount, storageAccount);
            Asset asset = CreateAsset(storageAccount, assetName);
            CloudBlockBlob assetBlob = blobClient.GetBlockBlob(asset.Container, null, fileName);
            await assetBlob.UploadFromStreamAsync(fileStream);
            return asset;
        }

        public async Task<Asset> CreateAsset(string storageAccount, string assetName, string assetDescription, string assetAlternateId, string sourceContainer, string fileName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = Path.GetFileNameWithoutExtension(fileName);
            }
            StorageBlobClient blobClient = new StorageBlobClient(this.MediaAccount, storageAccount);
            Asset asset = CreateAsset(storageAccount, assetName, assetDescription, assetAlternateId);
            string sourceUrl = blobClient.GetDownloadUrl(sourceContainer, fileName);
            CloudBlockBlob assetBlob = blobClient.GetBlockBlob(asset.Container, null, fileName);
            await assetBlob.StartCopyAsync(new Uri(sourceUrl));
            return asset;
        }

        public static string GetAssetName(StorageBlobClient blobClient, string containerName, string directoryPath, out MediaFile[] assetFiles)
        {
            string assetName = null;
            assetFiles = GetAssetFiles(blobClient, containerName, directoryPath, out bool assetStreamable);
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

        public static MediaFile[] GetAssetFiles(StorageBlobClient blobClient, string containerName, string directoryPath, out bool assetStreamable)
        {
            assetStreamable = false;
            List<MediaFile> files = new List<MediaFile>();
            CloudBlob[] blobs = blobClient.ListBlobContainer(containerName, directoryPath);
            foreach (CloudBlob blob in blobs)
            {
                string fileName = blob.Name;
                string fileSize = blobClient.GetBlobSize(containerName, directoryPath, fileName, out long byteCount, out string contentType);
                MediaFile file = new MediaFile()
                {
                    Name = fileName,
                    Size = fileSize,
                    ByteCount = byteCount,
                    ContentType = contentType,
                    DownloadUrl = blobClient.GetDownloadUrl(containerName, fileName)
                };
                if (file.Name.EndsWith(Constant.FileExtension.StreamingManifest, StringComparison.OrdinalIgnoreCase))
                {
                    assetStreamable = true;
                }
                files.Add(file);
            }
            return files.ToArray();
        }

        public static string GetAssetFileUrl(MediaClient mediaClient, Asset asset)
        {
            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
            MediaAsset mediaAsset = new MediaAsset(mediaClient, asset);
            string fileName = mediaAsset.Files[0].Name;
            return blobClient.GetDownloadUrl(asset.Container, fileName);
        }
    }
}