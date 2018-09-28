using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal class SearchClient
    {
        public SearchClient()
        {
            string settingKey = Constant.AppSettingKey.SearchServiceName;
            string serviceName = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.SearchAdminKey;
            string adminKey = AppSetting.GetValue(settingKey);

            SearchCredentials searchCredentials = new SearchCredentials(serviceName);
            SearchServiceClient searchClient = new SearchServiceClient(adminKey, searchCredentials);
        }
    }
}