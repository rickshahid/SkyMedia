using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaAsset
    {
        private MediaClient _mediaClient;
        private string _cdnUrl;
        private IAsset _asset;
        private IAssetFile _file;
        private MediaProtection[] _protectionTypes;

        private MediaAsset(MediaClient mediaClient)
        {
            _mediaClient = mediaClient;
            string settingKey = Constant.AppSettingKey.StorageCdnUrl;
            _cdnUrl = AppSetting.GetValue(settingKey);
        }

        private long GetAssetBytes(out int fileCount)
        {
            fileCount = 0;
            long assetBytes = 0;
            foreach (IAssetFile file in _asset.AssetFiles)
            {
                fileCount = fileCount + 1;
                assetBytes = assetBytes + file.ContentFileSize;
            }
            return assetBytes;
        }

        public MediaAsset(MediaClient mediaClient, IAsset asset) : this(mediaClient)
        {
            _asset = asset;
            _protectionTypes = mediaClient.GetProtectionTypes(asset);
        }

        public MediaAsset(MediaClient mediaClient, IAssetFile file) : this(mediaClient)
        {
            _file = file;
            _protectionTypes = new MediaProtection[] { };
        }

        public string Id
        {
            get { return (_file != null) ? _file.Id : _asset.Id; }
        }

        public IAsset Asset
        {
            get { return (_file != null) ? null : _asset; }
        }
        public bool IsStreamable
        {
            get { return (_file != null) ? false : _asset.IsStreamable; }
        }

        public string Text
        {
            get
            {
                if (_file != null)
                {
                    string fileSize = Storage.MapByteCount(_file.ContentFileSize);
                    return string.Concat(_file.Name, " (", fileSize, ")");
                }
                else
                {
                    int fileCount;
                    long assetBytes = GetAssetBytes(out fileCount);
                    string assetSize = Storage.MapByteCount(assetBytes);
                    string filesLabel = (fileCount == 1) ? " File" : " Files";
                    string assetInfo = string.Concat(" (", fileCount, filesLabel, ", ", assetSize, ")");
                    if (_asset.Options == AssetCreationOptions.StorageEncrypted)
                    {
                        string protectionLabel = " Storage";
                        if (_protectionTypes.Length > 0)
                        {
                            switch (_protectionTypes[0])
                            {
                                case MediaProtection.AES:
                                    protectionLabel = string.Concat(protectionLabel, " & Envelope (AES)");
                                    break;
                                case MediaProtection.PlayReady:
                                    protectionLabel = string.Concat(protectionLabel, " & DRM (PlayReady)");
                                    break;
                                case MediaProtection.Widevine:
                                    protectionLabel = string.Concat(protectionLabel, " & DRM (Widevine)");
                                    break;
                            }
                        }
                        protectionLabel = string.Concat(protectionLabel, " Encryption");
                        assetInfo = string.Concat(assetInfo, " <img title='", protectionLabel, "' src='", _cdnUrl, "/MediaLock.png' />");
                    }
                    return string.Concat(_asset.Name, assetInfo);
                }
            }
        }

        public string Icon
        {
            get
            {
                string treeIcon = (_file != null) ? Constant.Media.TreeIcon.MediaFile : Constant.Media.TreeIcon.MediaAsset;
                return string.Concat(_cdnUrl, treeIcon);
            }
        }

        public string Url
        {
            get { return (_file != null) ? string.Empty : _mediaClient.GetLocatorUrl(_asset, LocatorType.OnDemandOrigin, null); }
        }

        public bool Children
        {
            get { return _asset != null; }
        }

        public object A_attr
        {
            get
            {
                if (_file != null)
                {
                    return new { @class = "mediaFile", isStreamable = false };
                }
                else
                {
                    return new { @class = "mediaAsset", isStreamable = _asset.IsStreamable };
                }
            }
        }

        public object Data
        {
            get
            {
                return new { protectionTypes = _protectionTypes };
            }
        }
    }

    public class MediaAssetInput
    {
        public string AssetId { get; set; }

        public string AssetName { get; set; }

        public string PrimaryFile { get; set; }

        public string MarkIn { get; set; }

        public string MarkOut { get; set; }

        public string ClipDuration { get; set; }
    }
}
