using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal class Asset
    {
        private MediaClient _mediaClient;
        private IAsset _asset;
        private IAssetFile _file;
        private string _storageCdnUrl;

        //private MediaProtection[] _contentProtection;
        //private string _locatorUrl;
        //private string _webVttUrl;

        private Asset(MediaClient mediaClient)
        {
            _mediaClient = mediaClient;
            string settingKey = Constant.AppSettingKey.StorageCdnUrl;
            _storageCdnUrl = AppSetting.GetValue(settingKey);

            //_contentProtection = new MediaProtection[] { };
            //_locatorUrl = string.Empty;
            //_webVttUrl = string.Empty;
        }

        public Asset(MediaClient mediaClient, IAsset asset) : this(mediaClient)
        {
            _asset = asset;
        }

        public Asset(MediaClient mediaClient, IAssetFile file) : this(mediaClient)
        {
            _file = file;
        }

        public string Id
        {
            get { return _file != null ? _file.Id : _asset.Id; }
        }

        public string ClientId
        {
            get { string id = this.Id; return id.Replace(":", "").Replace("-", ""); }
        }

        public bool IsStreamable
        {
            get { return _file != null ? false : _asset.IsStreamable; }
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
                    long assetBytes = Storage.GetAssetBytes(_asset, out fileCount);
                    string assetSize = Storage.MapByteCount(assetBytes);
                    string filesLabel = fileCount == 1 ? " File" : " Files";
                    string assetInfo = string.Concat(" (", fileCount, filesLabel, ", ", assetSize, ")");
                    if (_asset.Options == AssetCreationOptions.StorageEncrypted)
                    {
                        assetInfo = string.Concat(assetInfo, " <img id='", this.ClientId, "' class='mediaLock' src='", _storageCdnUrl, "/MediaLock.png' />");
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

        //public MediaProtection[] ContentProtection
        //{
        //    get
        //    {
        //        if (_contentProtection.Length == 0 && _asset != null)
        //        {
        //            _contentProtection = _mediaClient.GetContentProtection(_asset);
        //        }
        //        return _contentProtection;
        //    }
        //}

        public string ContentProtectionTip
        {
            get
            {
                string protectionTip = string.Empty;
                if (_asset != null && _asset.Options == AssetCreationOptions.StorageEncrypted)
                {
                    protectionTip = "Storage";
                    MediaProtection[] contentProtection = _mediaClient.GetContentProtection(_asset);
                    if (contentProtection.Length > 0)
                    {
                        switch (contentProtection[0])
                        {
                            case MediaProtection.AES:
                                protectionTip = string.Concat(protectionTip, " & Envelope (AES)");
                                break;
                            case MediaProtection.PlayReady:
                                protectionTip = string.Concat(protectionTip, " & DRM (PlayReady)");
                                break;
                            case MediaProtection.Widevine:
                                protectionTip = string.Concat(protectionTip, " & DRM (Widevine)");
                                break;
                            case MediaProtection.FairPlay:
                                protectionTip = string.Concat(protectionTip, " & DRM (FairPlay)");
                                break;
                        }
                    }
                    protectionTip = string.Concat(protectionTip, " Encryption");
                }
                return protectionTip;
            }
        }

        //public string LocatorUrl
        //{
        //    get
        //    {
        //        if (_asset != null)
        //        {
        //            _locatorUrl = _mediaClient.GetLocatorUrl(_asset);
        //        }
        //        return _locatorUrl;
        //    }
        //}

        //public string WebVttUrl
        //{
        //    get
        //    {
        //        if (_asset != null)
        //        {
        //            _webVttUrl = _mediaClient.GetWebVttUrl(_asset);
        //        }
        //        return _webVttUrl;
        //    }
        //}

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
                    string cssClass = _file.IsPrimary ? "mediaFile primary" : "mediaFile";
                    return new { @class = cssClass, isStreamable = false, };
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
                return new {
                    clientId = this.ClientId,
                    //contentProtection = this.ContentProtection,
                    contentProtectionTip = this.ContentProtectionTip,
                    //locatorUrl = this.LocatorUrl,
                    //webVttUrl = this.WebVttUrl
                };
            }
        }
    }
}