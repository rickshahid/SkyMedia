using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Models
{
    public class SiteButton : ClientControl
    {
        private string _imageFile;
        private string _relativeUrl;
        private string _absoluteUrl;
        private string _onClick;
        private string _styleClass;

        public string ImageFile
        {
            get
            {
                string settingKey = Constant.AppSettingKey.ContentDeliveryEndpointStorage;
                string storageCdnUrl = AppSetting.GetValue(settingKey);
                return string.Concat(storageCdnUrl, "/", _imageFile);
            }

            set { _imageFile = value; }
        }

        public string RelativeUrl
        {
            set { _relativeUrl = value; }
        }

        public string AbsoluteUrl
        {
            set { _absoluteUrl = value; }
        }

        public string OnClick
        {
            get
            {
                string onClick = _onClick;
                if (!string.IsNullOrEmpty(_relativeUrl))
                {
                    onClick = string.Concat("window.location.href = '", _relativeUrl, "'");
                }
                else if (!string.IsNullOrEmpty(_absoluteUrl))
                {
                    onClick = string.Concat("window.open('", _absoluteUrl, "')");
                }
                return onClick;
            }

            set { _onClick = value; }
        }

        public string StyleClass
        {
            get
            {
                string styleClass = "siteButton";
                if (!string.IsNullOrEmpty(_styleClass))
                {
                    styleClass = string.Concat(styleClass, " ", _styleClass);
                }
                return styleClass;
            }

            set { _styleClass = value; }
        }
    }
}