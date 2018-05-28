using System.Threading.Tasks;

using Microsoft.Rest.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        // TODO: Remove this workaround when fixed in v3 REST API
        private void SetContainer(Asset asset)
        {
            asset.Container = string.Concat("asset-", asset.AssetId);
        }

        public Asset CreateAsset(string assetName, string storageAccount, bool storageEncryption, string sourceContainer, string[] fileNames)
        {
            Asset asset = new Asset(name: assetName, storageEncryptionFormat: storageEncryption ? AssetStorageEncryptionFormat.MediaStorageClientEncryption : AssetStorageEncryptionFormat.None)
            {
                StorageAccountName = storageAccount
            };
            Task<AzureOperationResponse<Asset>> createTask = _media.Assets.CreateOrUpdateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, asset.Name, asset);
            AzureOperationResponse<Asset> createResponse = createTask.Result;
            asset = createResponse.Body;
            SetContainer(asset);
            BlobClient blobClient = new BlobClient(MediaAccount, storageAccount);
            foreach (string fileName in fileNames)
            {
                CloudBlockBlob sourceBlob = blobClient.GetBlockBlob(sourceContainer, null, fileName);
                CloudBlockBlob assetBlob = blobClient.GetBlockBlob(asset.Container, null, fileName);
                assetBlob.StartCopyAsync(sourceBlob);
            }
            return asset;
        }
    }
}