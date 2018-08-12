using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaAsset : Asset
    {
        internal static string GetAssetName(BlobClient blobClient, string containerName, string directoryPath)
        {
            string assetName = string.Empty;
            CloudBlockBlob[] assetFiles = GetAssetFiles(blobClient, containerName, directoryPath, false);
            if (assetFiles.Length == 1)
            {
                assetName = assetFiles[0].Name;
            }
            else
            {
                foreach (CloudBlockBlob assetFile in assetFiles)
                {
                    if (assetFile.Name.EndsWith(Constant.Media.Stream.ManifestExtension))
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

        internal static CloudBlockBlob[] GetAssetFiles(BlobClient blobClient, string containerName, string directoryPath, bool fetchAtributes)
        {
            BlobContinuationToken continuationToken = null;
            List<CloudBlockBlob> blobs = new List<CloudBlockBlob>();
            CloudBlobContainer blobContainer = blobClient.GetBlobContainer(containerName);
            do
            {
                BlobResultSegment blobSegment;
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    CloudBlobDirectory blobDirectory = blobContainer.GetDirectoryReference(directoryPath);
                    blobSegment = blobDirectory.ListBlobsSegmentedAsync(continuationToken).Result;
                }
                else
                {
                    blobSegment = blobContainer.ListBlobsSegmentedAsync(continuationToken).Result;
                }
                continuationToken = blobSegment.ContinuationToken;
                IEnumerable<IListBlobItem> blobItems = blobSegment.Results;
                foreach (IListBlobItem blobItem in blobItems)
                {
                    string fileName = Path.GetFileName(blobItem.Uri.ToString());
                    CloudBlockBlob blob = blobClient.GetBlockBlob(containerName, directoryPath, fileName, fetchAtributes);
                    blobs.Add(blob);
                }
            } while (continuationToken != null);
            return blobs.ToArray();
        }

        internal MediaAsset(MediaAccount mediaAccount, Asset asset, bool fetchAttributes) : base(asset.Id, asset.Name, asset.Type, asset.AssetId, asset.Created, asset.LastModified, asset.AlternateId, asset.Description, asset.Container, asset.StorageAccountName, asset.StorageEncryptionFormat)
        {
            BlobClient blobClient = new BlobClient(mediaAccount, asset.StorageAccountName);
            Files = GetAssetFiles(blobClient, asset.Container, null, fetchAttributes);
        }

        public CloudBlockBlob[] Files { get; internal set; }
    }
}