using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            IndexerClient.UploadVideo uploadVideo = (IndexerClient.UploadVideo)asyncResult.AsyncDelegate;
            IAsset asset = (IAsset)asyncResult.AsyncState;
            asset.AlternateId = uploadVideo.EndInvoke(result);
            asset.Update();
            foreach (ILocator locator in asset.Locators)
            {
                locator.Delete();
            }
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

        public IAsset CreateAsset(string authToken, string assetName, string storageAccount, bool storageEncryption, string[] fileNames)
        {
            AssetCreationOptions assetOptions = storageEncryption ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            IAsset asset = _media.Assets.Create(assetName, storageAccount, assetOptions);

            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            string containerName = Constant.Storage.Blob.Container.Upload;

            if (fileNames.Length == 1)
            {
                string fileName = fileNames[0];
                CloudBlockBlob sourceBlob = blobClient.GetBlob(containerName, null, fileName, false);
                Stream sourceStream = sourceBlob.OpenRead();

                IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                assetFile.Upload(sourceStream);
            }
            else
            {
                List<Task> uploadTasks = new List<Task>();
                BlobTransferClient transferClient = new BlobTransferClient();
                ILocator sasLocator = CreateLocator(LocatorType.Sas, asset, true);
                foreach (string fileName in fileNames)
                {
                    CloudBlockBlob sourceBlob = blobClient.GetBlob(containerName, null, fileName, false);
                    Stream sourceStream = sourceBlob.OpenRead();

                    IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                    Task uploadTask = assetFile.UploadAsync(sourceStream, transferClient, sasLocator, CancellationToken.None);
                    uploadTasks.Add(uploadTask);
                }
                Task.WaitAll(uploadTasks.ToArray());
                sasLocator.Delete();
            }

            SetPrimaryFile(asset);

            string attributeName = Constant.UserAttribute.VideoIndexerKey;
            string indexerKey = AuthToken.GetClaimValue(authToken, attributeName);
            if (!string.IsNullOrEmpty(indexerKey) && asset.AssetFiles.Count() == 1)
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                AsyncCallback indexerCallback = new AsyncCallback(SetIndexId);
                IndexerClient.UploadVideo uploadVideo = indexerClient.GetIndexId;
                string locatorUrl = GetLocatorUrl(LocatorType.Sas, asset, null, true);
                uploadVideo.BeginInvoke(asset.Name, MediaPrivacy.Private, locatorUrl, indexerCallback, asset);
            }

            return asset;
        }
    }
}
