using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal class Asset
    {
        private string _authToken;
        private MediaClient _mediaClient;

        private IAsset _asset;
        private IAssetFile _file;

        private bool _getFiles;
        private string _storageCdnUrl;
        private string _contentProtectionTip;

        private string GetEntityId(bool clientId)
        {
            string entityId = _file != null ? _file.Id : _asset.Id;
            if (clientId)
            {
                entityId = entityId.Replace(":", "").Replace("-", "");
            }
            return entityId;
        }

        private string GetContentProtectionTip()
        {
            string tipText = string.Empty;
            if (_asset != null)
            {
                if (_asset.Options == AssetCreationOptions.StorageEncrypted)
                {
                    tipText = "Encryption: Storage";
                }
                StreamProtection[] streamProtections = _mediaClient.GetStreamProtections(_authToken, _asset);
                foreach (StreamProtection streamProtection in streamProtections)
                {
                    string protectionType = streamProtection.Type.ToString();
                    if (string.IsNullOrEmpty(tipText))
                    {
                        tipText = string.Concat("Encryption: ", protectionType);
                    }
                    else
                    {
                        tipText = string.Concat(tipText, " + ", protectionType);
                    }
                }
            }
            return tipText;
        }

        private Asset(string authToken)
        {
            _authToken = authToken;
            _mediaClient = new MediaClient(authToken);
            string settingKey = Constant.AppSettingKey.StorageContentDeliveryEndpointUrl;
            _storageCdnUrl = AppSetting.GetValue(settingKey);
        }

        public Asset(string authToken, IAsset asset, bool getFiles) : this(authToken)
        {
            _asset = asset;
            _getFiles = getFiles;
            _contentProtectionTip = GetContentProtectionTip();
        }

        public Asset(string authToken, IAssetFile file) : this(authToken)
        {
            _file = file;
        }

        public string Id
        {
            get { return GetEntityId(false); }
        }

        public string Text
        {
            get
            {
                if (_file != null)
                {
                    string fileSize = Storage.MapByteCount(_file.ContentFileSize);
                    string fileInfo = string.Concat(_file.Name, " (", fileSize, ")");
                    if (_file.IsPrimary)
                    {
                        fileInfo = string.Concat("Primary File: ", fileInfo);
                    }
                    return fileInfo;
                }
                else
                {
                    int fileCount;
                    long assetBytes = Storage.GetAssetBytes(_asset, out fileCount);
                    string assetSize = Storage.MapByteCount(assetBytes);
                    string filesLabel = fileCount == 1 ? " File" : " Files";
                    string assetInfo = string.Concat(" (", fileCount, filesLabel, ", ", assetSize, ")");
                    if (!string.IsNullOrEmpty(_contentProtectionTip))
                    {
                        assetInfo = string.Concat(assetInfo, " <img class='mediaLock' src='", _storageCdnUrl, "/MediaLock.png'>");
                    }
                    return string.Concat(_asset.Name, assetInfo);
                }
            }
        }

        public string Icon
        {
            get
            {
                string treeIcon = _file != null ? Constant.Media.TreeIcon.MediaFile : Constant.Media.TreeIcon.MediaAsset;
                return string.Concat(_storageCdnUrl, treeIcon);
            }
        }

        public object State
        {
            get
            {
                return new
                {
                    opened = _getFiles,
                    disabled = false,
                    selected = false
                };
            }
        }

        public object Data
        {
            get
            {
                return new
                {
                    entityId = GetEntityId(false),
                    clientId = GetEntityId(true),
                    contentProtectionTip = _contentProtectionTip
                };
            }
        }

        public bool Children
        {
            get { return _asset != null; }
        }

        public object A_attr
        {
            get
            {
                string clientId = GetEntityId(true);
                clientId = string.Concat(clientId, "-anchor");
                if (_file != null)
                {
                    string cssClass = _file.IsPrimary ? "mediaFile primary" : "mediaFile";
                    return new { id = clientId, @class = cssClass };
                }
                else
                {
                    return new { id = clientId, @class = "mediaAsset" };
                }
            }
        }
    }
}