using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        // Remove this workaround when fixed in v3 API
        public static void SetContainer(Asset asset)
        {
            asset.Container = string.Concat("asset-", asset.AssetId);
        }

        public Asset CreateAsset(string storageAccount, string assetName, string description, string alternateId)
        {
            Asset asset = new Asset(name: assetName)
            {
                StorageAccountName = storageAccount,
                Description = description,
                AlternateId = alternateId
            };
            return _media.Assets.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, asset.Name, asset);
        }

        public Asset CreateAsset(string storageAccount, string assetName)
        {
            return CreateAsset(storageAccount, assetName, string.Empty, string.Empty);
        }

        public Asset CreateAsset(string storageAccount, string assetName, string description, string alternateId, string blobContainer, string[] fileNames)
        {
            Asset asset = CreateAsset(storageAccount, assetName, description, alternateId);
            SetContainer(asset);
            BlobClient blobClient = new BlobClient(MediaAccount, storageAccount);
            foreach (string fileName in fileNames)
            {
                CloudBlockBlob sourceBlob = blobClient.GetBlockBlob(blobContainer, fileName);
                CloudBlockBlob assetBlob = blobClient.GetBlockBlob(asset.Container, fileName);
                assetBlob.StartCopyAsync(sourceBlob);
            }
            return asset;
        }

        public Asset CreateAsset(string storageAccount, string assetName, string description, string alternateId, string blobContainer, string fileName)
        {
            return CreateAsset(storageAccount, assetName, description, alternateId, blobContainer, new string[] { fileName });
        }
    }
}