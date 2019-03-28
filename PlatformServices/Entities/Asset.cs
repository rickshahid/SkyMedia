using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal class MediaAsset : Asset
    {
        public MediaAsset(MediaClient mediaClient, Asset asset) : base(asset.Id, asset.Name, asset.Type, asset.AssetId, asset.Created, asset.LastModified, asset.AlternateId, asset.Description, asset.Container, asset.StorageAccountName, asset.StorageEncryptionFormat)
        {
            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
            Files = MediaClient.GetAssetFiles(blobClient, asset.Container, null, out bool assetStreamable);
            Filters = mediaClient.GetAllEntities<AssetFilter>(MediaEntity.FilterAsset, null, asset.Name);
            StreamingUrls = mediaClient.GetStreamingUrls(asset.Name);
            Streamable = assetStreamable;
            Published = mediaClient.GetLocators(asset.Name).Length > 0;
        }

        public MediaFile[] Files { get; }

        public AssetFilter[] Filters { get; }

        public string Size
        {
            get
            {
                long byteCount = 0;
                foreach (MediaFile assetFile in Files)
                {
                    byteCount = byteCount + assetFile.ByteCount;
                }
                return StorageBlobClient.MapByteCount(byteCount);
            }
        }

        public string[] StreamingUrls { get; }

        public bool Streamable { get; }

        public bool Published { get; }
    }

    internal class MediaFile
    {
        public string Name { get; set; }

        public string Size { get; set; }

        public long ByteCount { get; set; }

        public string ContentType { get; set; }

        public string DownloadUrl { get; set; }
    }
}