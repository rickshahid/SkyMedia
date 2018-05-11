using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class ContentProtection
    {
        public ContentProtection()
        {
            string settingKey = Constant.AppSettingKey.DirectoryDefaultId;
            string directoryId = AppSetting.GetValue(settingKey);
            BindDirectory(directoryId);
        }

        public ContentProtection(string directoryId)
        {
            BindDirectory(directoryId);
        }

        private void BindDirectory(string directoryId)
        {
            string settingKey = Constant.AppSettingKey.DirectoryClientId;
            settingKey = string.Format(settingKey, directoryId);
            this.DirectoryId = directoryId;
            this.Audience = AppSetting.GetValue(settingKey);
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string DirectoryId { get; set; }

        public string Audience { get; set; }

        public bool Aes { get; set; }

        public bool DrmPlayReady { get; set; }

        public bool DrmWidevine { get; set; }

        public bool DrmFairPlay { get; set; }

        public bool DrmOffline { get; set; }

        public bool ContentAuthTypeToken { get; set; }

        public bool ContentAuthTypeAddress { get; set; }

        public string ContentAuthAddressRange { get; set; }
    }
}