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
            BlobContinuationToken continuationToken = null;
            List<CloudBlockBlob> blockBlobs = new List<CloudBlockBlob>();
            BlobClient blobClient = new BlobClient(mediaAccount, asset.StorageAccountName);
            CloudBlobContainer assetContainer = blobClient.GetBlobContainer(asset.Container);
            List<CloudBlockBlob> blobs = new List<CloudBlockBlob>();
            do
            {
                BlobResultSegment resultSegment = assetContainer.ListBlobsSegmentedAsync(continuationToken).Result;
                continuationToken = resultSegment.ContinuationToken;
                IEnumerable<IListBlobItem> blobItems = resultSegment.Results;
                foreach (IListBlobItem blobItem in blobItems)
                {
                    string fileName = Path.GetFileName(blobItem.Uri.ToString());
                    CloudBlockBlob blob = blobClient.GetBlockBlob(asset.Container, fileName);
                    blobs.Add(blob);
                }
            } while (continuationToken != null);
            Files = blobs.ToArray();
        }

        public CloudBlockBlob[] Files { get; internal set; }
    }
}