using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static void SetIndexId(IAsyncResult result)
        {
            AsyncResult asyncResult = (AsyncResult)result;
            IndexerClient.UploadVideo uploadVideo = asyncResult.AsyncDelegate as IndexerClient.UploadVideo;
            IAsset asset = asyncResult.AsyncState as IAsset;
            asset.AlternateId = uploadVideo.EndInvoke(result);
            asset.Update();
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
                        MediaAsset mediaFile = new MediaAsset(this, assetFile);
                        mediaAssets.Add(mediaFile);
                    }
                }
                foreach (IAsset asset in _media.Assets)
                {
                    foreach (IAsset parentAsset in asset.ParentAssets)
                    {
                        if (string.Equals(parentAsset.Id, rootAsset.Id, StringComparison.OrdinalIgnoreCase))
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

            string claimName = Constant.UserAttribute.VideoIndexerKey;
            string indexerKey = AuthToken.GetClaimValue(authToken, claimName);
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                AsyncCallback indexerCallback = new AsyncCallback(SetIndexId);
                if (asset.AssetFiles.Count() == 1)
                {
                    IndexerClient.UploadVideo uploadVideo = indexerClient.GetIndexId;
                    string locatorUrl = GetLocatorUrl(LocatorType.Sas, asset, null, true);
                    uploadVideo.BeginInvoke(asset.Name, MediaPrivacy.Private, locatorUrl, indexerCallback, asset);
                }
            }

            return asset;
        }
    }
}
