using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private IAsset[] GetAssets(string[] assetIds)
        {
            List<IAsset> assets = new List<IAsset>();
            foreach (string assetId in assetIds)
            {
                IAsset asset = GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets.ToArray();
        }

        public MediaAsset[] GetAssets(string assetId)
        {
            List<MediaAsset> assets = new List<MediaAsset>();
            if (string.IsNullOrEmpty(assetId))
            {
                foreach (IAsset asset in _media.Assets)
                {
                    if (asset.ParentAssets.Count == 0)
                    {
                        MediaAsset mediaAsset = new MediaAsset(this, asset);
                        assets.Add(mediaAsset);
                    }
                }
            }
            else
            {
                IAsset rootAsset = GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                if (rootAsset != null)
                {
                    foreach (IAssetFile assetFile in rootAsset.AssetFiles)
                    {
                        MediaAsset mediaFile = new MediaAsset(this, assetFile);
                        assets.Add(mediaFile);
                    }
                }
                foreach (IAsset asset in _media.Assets)
                {
                    foreach (IAsset parentAsset in asset.ParentAssets)
                    {
                        if (string.Equals(parentAsset.Id, rootAsset.Id, StringComparison.OrdinalIgnoreCase))
                        {
                            MediaAsset mediaAsset = new MediaAsset(this, asset);
                            assets.Add(mediaAsset);
                        }
                    }
                }
            }
            return assets.ToArray();
        }

        private CloudBlockBlob CreateAssetFile(IAsset asset, BlobClient blobClient, string containerName, string fileName)
        {
            CloudBlockBlob blob = blobClient.GetBlob(containerName, null, fileName, true);
            IAssetFile assetFile = asset.AssetFiles.Create(fileName);
            assetFile.ContentFileSize = blob.Properties.Length;
            assetFile.Update();
            return blob;
        }

        public IAsset CreateAsset(string authToken, string assetName, string storageAccount, bool storageEncryption, string[] fileNames)
        {
            AssetCreationOptions assetOptions = storageEncryption ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            IAsset asset = _media.Assets.Create(assetName, storageAccount, assetOptions);

            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            string sourceContainerName = Constant.Storage.Blob.Container.Upload;
            string destinationContainerName = asset.Uri.Segments[1];

            if (fileNames.Length == 1)
            {
                string fileName = fileNames[0];
                CloudBlockBlob sourceBlob = CreateAssetFile(asset, blobClient, sourceContainerName, fileName);
                CloudBlockBlob destinationBlob = blobClient.GetBlob(destinationContainerName, null, fileName, false);
                destinationBlob.StartCopy(sourceBlob);
            }
            else
            {
                List<Task> copyTasks = new List<Task>();
                foreach (string fileName in fileNames)
                {
                    CloudBlockBlob sourceBlob = CreateAssetFile(asset, blobClient, sourceContainerName, fileName);
                    CloudBlockBlob destinationBlob = blobClient.GetBlob(destinationContainerName, null, fileName, false);
                    Task copyTask = destinationBlob.StartCopyAsync(sourceBlob);
                    copyTasks.Add(copyTask);
                }
                Task.WaitAll(copyTasks.ToArray());
            }

            SetPrimaryFile(asset);
            return asset;
        }
    }
}
