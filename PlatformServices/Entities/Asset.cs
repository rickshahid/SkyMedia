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
            MediaClient.SetContainer(asset);
            List<CloudBlockBlob> blockBlobs = new List<CloudBlockBlob>();
            BlobClient blobClient = new BlobClient(mediaAccount, asset.StorageAccountName);
            CloudBlobContainer assetContainer = blobClient.GetBlobContainer(asset.Container);
            IEnumerable<IListBlobItem> blobItems = assetContainer.ListBlobsSegmentedAsync(null).Result.Results;
            List<string> fileNames = new List<string>();
            foreach (IListBlobItem blobItem in blobItems)
            {
                string fileName = Path.GetFileName(blobItem.Uri.ToString());
                fileNames.Add(fileName);
            }
            FileNames = fileNames.ToArray();
        }

        public string[] FileNames { get; internal set; }
    }
}