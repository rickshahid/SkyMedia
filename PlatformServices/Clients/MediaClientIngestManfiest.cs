using System;
using System.IO;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        public string CreateManifest(string storageAccount, bool storageEncryption, string[] fileNames)
        {
            string manifestName = Guid.NewGuid().ToString();
            IIngestManifest manifest = _media.IngestManifests.Create(manifestName, storageAccount);
            AssetCreationOptions assetOptions = storageEncryption ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            foreach (string fileName in fileNames)
            {
                string assetName = Path.GetFileNameWithoutExtension(fileName);
                IAsset asset = _media.Assets.Create(assetName, storageAccount, assetOptions);
                IIngestManifestAsset manifestAsset = manifest.IngestManifestAssets.Create(asset, new string[] { fileName });
            }
            return Path.GetFileName(manifest.BlobStorageUriForUpload);
        }
    }
}
