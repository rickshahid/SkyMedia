using System.Threading.Tasks;

using Microsoft.Rest.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
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

        public Asset CreateAsset(string storageAccount, string name, string description, string alternateId)
        {
            Asset asset = new Asset(name: name)
            {
                StorageAccountName = storageAccount,
                Description = description,
                AlternateId = alternateId
            };
            Task<AzureOperationResponse<Asset>> task = _media.Assets.CreateOrUpdateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, asset.Name, asset);
            AzureOperationResponse<Asset> response = task.Result;
            return response.Body;
        }

        public Asset CreateAsset(string storageAccount, string name, string description, string alternateId, string blobContainer, string[] fileNames)
        {
            Asset asset = CreateAsset(storageAccount, name, description, alternateId);
            SetContainer(asset);
            BlobClient blobClient = new BlobClient(MediaAccount, storageAccount);
            foreach (string fileName in fileNames)
            {
                CloudBlockBlob sourceBlob = blobClient.GetBlockBlob(blobContainer, null, fileName);
                CloudBlockBlob assetBlob = blobClient.GetBlockBlob(asset.Container, null, fileName);
                assetBlob.StartCopyAsync(sourceBlob);
            }
            return asset;
        }
    }
}