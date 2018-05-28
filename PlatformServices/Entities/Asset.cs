using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal class AssetNode
    {
        private string _authToken;
        private MediaClient _mediaClient;

        private Asset _asset;
        private CloudBlob _file;

        private bool _getFiles;
        private string _storageCdnUrl;
        private string _contentProtectionTip;

        private string GetEntityId(bool clientId)
        {
            string entityId = _file != null ? _file.Uri.ToString() : _asset.Id;
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
                if (_asset.StorageEncryptionFormat == AssetStorageEncryptionFormat.MediaStorageClientEncryption)
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

        private static long GetAssetBytes(Asset asset, out int fileCount)
        {
            fileCount = 0;
            long assetBytes = 0;
            //foreach (IAssetFile file in asset.AssetFiles)
            //{
            //    fileCount = fileCount + 1;
            //    assetBytes = assetBytes + file.ContentFileSize;
            //}
            return assetBytes;
        }

        private static string MapByteCount(long byteCount)
        {
            string mappedCount;
            if (byteCount >= 1099511627776)
            {
                mappedCount = (byteCount / 1099511627776.0).ToString(Constant.TextFormatter.Numeric) + " TB";
            }
            else if (byteCount >= 1073741824)
            {
                mappedCount = (byteCount / 1073741824.0).ToString(Constant.TextFormatter.Numeric) + " GB";
            }
            else if (byteCount >= 1048576)
            {
                mappedCount = (byteCount / 1048576.0).ToString(Constant.TextFormatter.Numeric) + " MB";
            }
            else if (byteCount >= 1024)
            {
                mappedCount = (byteCount / 1024.0).ToString(Constant.TextFormatter.Numeric) + " KB";
            }
            else if (byteCount == 1)
            {
                mappedCount = byteCount + " Byte";
            }
            else
            {
                mappedCount = byteCount + " Bytes";
            }
            return mappedCount;
        }

        private AssetNode(string authToken)
        {
            _authToken = authToken;
            _mediaClient = new MediaClient(authToken);
            string settingKey = Constant.AppSettingKey.StorageContentDeliveryEndpointUrl;
            _storageCdnUrl = AppSetting.GetValue(settingKey);
        }

        public AssetNode(string authToken, Asset asset, bool getFiles) : this(authToken)
        {
            _asset = asset;
            _getFiles = getFiles;
            _contentProtectionTip = GetContentProtectionTip();
        }

        public AssetNode(string authToken, CloudBlob file) : this(authToken)
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
                    string fileSize = MapByteCount(_file.Properties.Length);
                    return string.Concat(_file.Name, " (", fileSize, ")");
                }
                else
                {
                    int fileCount;
                    long assetBytes = GetAssetBytes(_asset, out fileCount);
                    string assetSize = MapByteCount(assetBytes);
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
                    return new { id = clientId, @class = "mediaFile" };
                }
                else
                {
                    return new { id = clientId, @class = "mediaAsset" };
                }
            }
        }
    }
}