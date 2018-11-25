using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
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
    }
}