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
        private void UploadAssetFiles(BlobClient blobClient, string sourceContainer, IAsset asset, string[] fileNames)
        {
            if (fileNames.Length == 1)
            {
                string fileName = fileNames[0];
                IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                CloudBlockBlob sourceBlob = blobClient.GetBlockBlob(sourceContainer, null, fileName, true);
                Stream sourceStream = sourceBlob.OpenRead();
                assetFile.Upload(sourceStream);
                foreach (ILocator locator in asset.Locators)
                {
                    locator.Delete();
                }
            }
            else
            {
                BlobTransferClient transferClient = new BlobTransferClient();
                ILocator sasLocator = CreateLocator(LocatorType.Sas, asset, true);
                List<Task> uploadTasks = new List<Task>();
                foreach (string fileName in fileNames)
                {
                    IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                    CloudBlockBlob sourceBlob = blobClient.GetBlockBlob(sourceContainer, null, fileName, true);
                    Stream sourceStream = sourceBlob.OpenRead();
                    Task uploadTask = assetFile.UploadAsync(sourceStream, transferClient, sasLocator, CancellationToken.None);
                    uploadTasks.Add(uploadTask);
                }
                Task.WaitAll(uploadTasks.ToArray());
                sasLocator.Delete();
            }
        }

        private static IAssetFile GetAssetFile(IAsset asset, string fileName)
        {
            IAssetFile assetFile = null;
            foreach (IAssetFile file in asset.AssetFiles)
            {
                if (string.Equals(file.Name, fileName, StringComparison.OrdinalIgnoreCase))
                {
                    assetFile = file;
                }
            }
            return assetFile;
        }

        private static IAssetFile[] GetAssetFiles(IAsset asset, string fileExtension)
        {
            List<IAssetFile> assetFiles = new List<IAssetFile>();
            foreach (IAssetFile file in asset.AssetFiles)
            {
                if (file.Name.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    assetFiles.Add(file);
                }
            }
            return assetFiles.ToArray();
        }

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

        public Asset[] GetAssets(string authToken, string assetId, bool getFiles)
        {
            List<Asset> assets = new List<Asset>();
            if (string.IsNullOrEmpty(assetId))
            {
                foreach (IAsset asset in _media.Assets)
                {
                    if (asset.ParentAssets.Count == 0)
                    {
                        Asset assetNode = new Asset(authToken, asset, getFiles);
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
                        Asset fileNode = new Asset(authToken, assetFile);
                        assets.Add(fileNode);
                    }
                }
                foreach (IAsset asset in _media.Assets)
                {
                    foreach (IAsset parentAsset in asset.ParentAssets)
                    {
                        if (string.Equals(parentAsset.Id, rootAsset.Id, StringComparison.OrdinalIgnoreCase))
                        {
                            Asset assetNode = new Asset(authToken, asset, getFiles);
                            assets.Add(assetNode);
                        }
                    }
                }
            }
            return assets.ToArray();
        }

        public string CreateAsset(string assetName, string[] fileNames)
        {
            string storageAccount = null;
            AssetCreationOptions assetEncryption = AssetCreationOptions.None;
            IAsset asset = _media.Assets.Create(assetName, storageAccount, assetEncryption);

            BlobClient blobClient = new BlobClient();
            string sourceContainer = Constant.Storage.Blob.Container.MediaProcess;

            UploadAssetFiles(blobClient, sourceContainer, asset, fileNames);
            SetPrimaryFile(asset);

            return asset.Id;
        }

        public IAsset CreateAsset(string authToken, string assetName, string storageAccount, bool storageEncryption, string[] fileNames)
        {
            AssetCreationOptions assetEncryption = storageEncryption ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            IAsset asset = _media.Assets.Create(assetName, storageAccount, assetEncryption);

            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            string sourceContainer = Constant.Storage.Blob.Container.FileUpload;

            UploadAssetFiles(blobClient, sourceContainer, asset, fileNames);
            SetPrimaryFile(asset);

            return asset;
        }

        public static string[] GetFileNames(IAsset asset, string fileExtension)
        {
            List<string> fileNames = new List<string>();
            foreach (IAssetFile file in asset.AssetFiles)
            {
                if (file.Name.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    fileNames.Add(file.Name);
                }
            }
            return fileNames.ToArray();
        }
    }
}