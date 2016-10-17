using Microsoft.Azure.Search;

namespace SkyMedia.ServiceBroker
{
    internal class SearchClient
    {
        private SearchServiceClient _admin;
        private SearchServiceClient _query;

        public SearchClient()
        {
            string settingKey = Constants.ConnectionStrings.AzureSearchReadWrite;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountName = accountCredentials[0];
            string accountKey = accountCredentials[1];

            SearchCredentials credentials = new SearchCredentials(accountKey);
            _admin = new SearchServiceClient(accountName, credentials);

            settingKey = Constants.ConnectionStrings.AzureSearchReadOnly;
            accountCredentials = AppSetting.GetValue(settingKey, true);
            accountName = accountCredentials[0];
            accountKey = accountCredentials[1];

            credentials = new SearchCredentials(accountKey);
            _query = new SearchServiceClient(accountName, credentials);
        }
    }
}
