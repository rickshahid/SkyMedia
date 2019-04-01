using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.WindowsAzure.Storage.Blob;

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

        public Asset CreateAsset(string storageAccount, string assetName, string fileName, Stream fileStream)
        {
            StorageBlobClient blobClient = new StorageBlobClient(this.MediaAccount, storageAccount);
            Asset asset = CreateAsset(storageAccount, assetName);
            CloudBlockBlob assetBlob = blobClient.GetBlockBlob(asset.Container, null, fileName);
            assetBlob.UploadFromStreamAsync(fileStream).Wait();
            return asset;
        }

        public Asset CreateAsset(StorageBlobClient sourceBlobClient, StorageBlobClient assetBlobClient, string storageAccount, string assetName, string assetDescription, string assetAlternateId, string sourceContainer, string[] fileNames)
        {
            List<Task> copyTasks = new List<Task>();
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = Path.GetFileNameWithoutExtension(fileNames[0]);
            }
            Asset asset = CreateAsset(storageAccount, assetName, assetDescription, assetAlternateId);
            foreach (string fileName in fileNames)
            {
                string sourceUrl = sourceBlobClient.GetDownloadUrl(sourceContainer, fileName, false);
                CloudBlockBlob assetBlob = assetBlobClient.GetBlockBlob(asset.Container, null, fileName);
                Task copyTask = assetBlob.StartCopyAsync(new Uri(sourceUrl));
                copyTasks.Add(copyTask);
            }
            Task.WaitAll(copyTasks.ToArray());
            return asset;
        }

        public Asset CreateAsset(StorageBlobClient sourceBlobClient, StorageBlobClient assetBlobClient, string storageAccount, string assetName, string assetDescription, string assetAlternateId, string sourceContainer, string fileName)
        {
            return CreateAsset(sourceBlobClient, assetBlobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, new string[] { fileName });
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
                    DownloadUrl = blobClient.GetDownloadUrl(containerName, fileName, false)
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
            return blobClient.GetDownloadUrl(asset.Container, fileName, false);
        }
    }
}