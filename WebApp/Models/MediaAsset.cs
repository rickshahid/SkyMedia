using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Models
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
            foreach (IListBlobItem blobItem in blobItems)
            {
                string blobName = Path.GetFileName(blobItem.Uri.ToString());
                CloudBlockBlob blockBlob = blobClient.GetBlockBlob(asset.Container, null, blobName);
                blockBlobs.Add(blockBlob);
            }
            Files = blockBlobs;
        }

        public IList<CloudBlockBlob> Files { get; internal set; }
    }
}