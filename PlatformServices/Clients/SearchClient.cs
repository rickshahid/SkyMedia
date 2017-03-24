using System;

using Microsoft.Azure.Search;
//using Microsoft.Azure.Search.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class SearchClient : IDisposable
    {
        private SearchServiceClient _search;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _search != null)
            {
                _search.Dispose();
                _search = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public SearchClient()
        {
            string settingKey = Constant.AppSettingKey.AzureSearch;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountName = accountCredentials[0];
            string accountKey = accountCredentials[1];

            SearchCredentials credentials = new SearchCredentials(accountKey);
            _search = new SearchServiceClient(accountName, credentials);

        }
    }
}
