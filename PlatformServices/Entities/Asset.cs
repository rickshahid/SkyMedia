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
            BlobClient blobClient = new BlobClient(mediaAccount, asset.StorageAccountName);
            Files = GetAssetFiles(blobClient, asset.Container, null);
        }

        internal static string GetAssetName(BlobClient blobClient, string containerName, string directoryPath)
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

        internal static MediaFile[] GetAssetFiles(BlobClient blobClient, string containerName, string directoryPath)
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
                    MediaFile file = new MediaFile()
                    {
                        Name = fileName,
                        Size = blobClient.GetBlobSize(containerName, fileName),
                        DownloadUrl = blobClient.GetDownloadUrl(containerName, fileName, false)
                    };
                    files.Add(file);
                }
                continuationToken = blobList.ContinuationToken;
            } while (continuationToken != null);
            return files.ToArray();
        }

        public MediaFile[] Files { get; internal set; }
    }

    public class MediaFile
    {
        public string Name { get; set; }

        public string Size { get; set; }

        public string DownloadUrl { get; set; }
    }
}