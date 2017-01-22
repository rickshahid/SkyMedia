using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.Services
{
    internal partial class MediaClient
    {
        private IAsset[] GetAssets(string[] assetIds)
        {
            List<IAsset> assets = new List<IAsset>();
            foreach (string assetId in assetIds)
            {
                IAsset asset = GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                assets.Add(asset);
            }
            return assets.ToArray();
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
                foreach (IAssetFile assetFile in rootAsset.AssetFiles)
                {
                    MediaAsset fileAsset = new MediaAsset(this, assetFile);
                    mediaAssets.Add(fileAsset);
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
            string sourceContainerName = Constants.Storage.ContainerNames.Upload;

            if (storageEncryption)
            {
                foreach (string fileName in fileNames)
                {
                    CloudBlockBlob sourceBlob = blobClient.GetBlob(sourceContainerName, null, fileName, false);
                    Stream sourceStream = sourceBlob.OpenRead();

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

        public IIngestManifest SetManifest(string manifestId, string assetName, string storageAccount, bool storageEncryption,
                                           bool multipleFileAsset, bool uploadBulkIngest, string[] fileNames)
        {
            IIngestManifest manifest = null;
            if (!string.IsNullOrEmpty(manifestId))
            {
                manifest = GetEntityById(MediaEntity.Manifest, manifestId) as IIngestManifest;
            }
            if (manifest == null && uploadBulkIngest)
            {
                string manifestName = Guid.NewGuid().ToString();
                manifest = _media.IngestManifests.Create(manifestName, storageAccount);
            }
            if (manifest != null)
            {
                foreach (IIngestManifestAsset manifestAsset in manifest.IngestManifestAssets)
                {
                    manifestAsset.Delete();
                }
                AssetCreationOptions creationOptions = storageEncryption ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
                if (multipleFileAsset)
                {
                    IAsset asset = _media.Assets.Create(assetName, storageAccount, creationOptions);
                    manifest.IngestManifestAssets.Create(asset, fileNames);
                }
                else
                {
                    foreach (string fileName in fileNames)
                    {
                        IAsset asset = _media.Assets.Create(fileName, storageAccount, creationOptions);
                        manifest.IngestManifestAssets.Create(asset, new string[] { fileName });
                    }
                }
            }
            return manifest;
        }
    }
}
