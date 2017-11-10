using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
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

        public Asset[] GetAssets(string assetId)
        {
            List<Asset> assets = new List<Asset>();
            if (string.IsNullOrEmpty(assetId))
            {
                foreach (IAsset asset in _media.Assets)
                {
                    if (asset.ParentAssets.Count == 0)
                    {
                        Asset assetNode = new Asset(this, asset);
                        assets.Add(assetNode);
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
                        Asset fileNode = new Asset(this, assetFile);
                        assets.Add(fileNode);
                    }
                }
                foreach (IAsset asset in _media.Assets)
                {
                    foreach (IAsset parentAsset in asset.ParentAssets)
                    {
                        if (string.Equals(parentAsset.Id, rootAsset.Id, StringComparison.OrdinalIgnoreCase))
                        {
                            Asset assetNode = new Asset(this, asset);
                            assets.Add(assetNode);
                        }
                    }
                }
            }
            return assets.ToArray();
        }

        public IAsset CreateAsset(string authToken, string assetName, string storageAccount, bool storageEncryption, string[] fileNames)
        {
            AssetCreationOptions assetEncryption = storageEncryption ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            IAsset asset = _media.Assets.Create(assetName, storageAccount, assetEncryption);

            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            string sourceContainer = Constant.Storage.Blob.Container.FileUpload;

            if (fileNames.Length == 1)
            {
                string fileName = fileNames[0];
                IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainer, null, fileName, true);
                Stream sourceStream = sourceBlob.OpenRead();
                assetFile.Upload(sourceStream);
            }
            else
            {
                BlobTransferClient transferClient = new BlobTransferClient();
                ILocator sasLocator = CreateLocator(LocatorType.Sas, asset, true);
                List<Task> uploadTasks = new List<Task>();
                foreach (string fileName in fileNames)
                {
                    IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                    CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainer, null, fileName, true);
                    Stream sourceStream = sourceBlob.OpenRead();
                    Task uploadTask = assetFile.UploadAsync(sourceStream, transferClient, sasLocator, CancellationToken.None);
                    uploadTasks.Add(uploadTask);
                }
                Task.WaitAll(uploadTasks.ToArray());
                sasLocator.Delete();
            }

            SetPrimaryFile(asset);
            return asset;
        }
    }
}