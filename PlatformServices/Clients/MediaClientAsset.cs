using System;
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

        internal static string[] GetFileNames(IAsset asset, string fileExtension)
        {
            List<string> fileNames = new List<string>();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    fileNames.Add(assetFile.Name);
                }
            }
            return fileNames.ToArray();
        }

        public MediaAsset[] GetAssets(string assetId)
        {
            List<MediaAsset> mediaAssets = new List<MediaAsset>();
            if (string.IsNullOrEmpty(assetId))
            {
                foreach (IAsset asset in _media.Assets)
                {
                    if (asset.ParentAssets.Count == 0)
                    {
                        MediaAsset mediaAsset = new MediaAsset(this, asset);
                        mediaAssets.Add(mediaAsset);
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
                        MediaAsset fileAsset = new MediaAsset(this, assetFile);
                        mediaAssets.Add(fileAsset);
                    }
                }
                foreach (IAsset asset in _media.Assets)
                {
                    foreach (IAsset parentAsset in asset.ParentAssets)
                    {
                        if (string.Equals(parentAsset.Id, rootAsset.Id, StringComparison.InvariantCultureIgnoreCase))
                        {
                            MediaAsset mediaAsset = new MediaAsset(this, asset);
                            mediaAssets.Add(mediaAsset);
                        }
                    }
                }
            }
            return mediaAssets.ToArray();
        }

        public IAsset CreateAsset(string authToken, string assetName, string storageAccount, bool storageEncryption, string[] fileNames)
        {
            AssetCreationOptions creationOptions = storageEncryption ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            IAsset asset = _media.Assets.Create(assetName, storageAccount, creationOptions);

            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            string sourceContainerName = Constant.Storage.Blob.Container.Upload;

            if (storageEncryption)
            {
                foreach (string fileName in fileNames)
                {
                    CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, null, fileName, false);
                    System.IO.Stream sourceStream = sourceBlob.OpenRead();

                    IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                    assetFile.Upload(sourceStream);
                }
            }
            else
            {
                string destinationContainerName = asset.Uri.Segments[1];
                foreach (string fileName in fileNames)
                {
                    CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, null, fileName, true);
                    CloudBlockBlob destinationBlob = blobClient.GetBlob(destinationContainerName, null, fileName, false);
                    blobClient.CopyBlob(sourceBlob, destinationBlob, false);

                    IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                    assetFile.ContentFileSize = sourceBlob.Properties.Length;
                    assetFile.Update();
                }
            }

            SetPrimaryFile(asset);
            asset.Update();
            return asset;
        }
    }
}
