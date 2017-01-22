using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace AzureSkyMedia.Services
{
    internal class SearchClient
    {
        private SearchServiceClient _search;

        public SearchClient(string authToken)
        {
            string accountName = AuthToken.GetClaimValue(authToken, Constants.UserAttribute.SearchAccountName);
            string accountKey = AuthToken.GetClaimValue(authToken, Constants.UserAttribute.SearchAccountKey);
            if (!string.IsNullOrEmpty(accountName))
            {
                SearchCredentials credentials = new SearchCredentials(accountKey);
                _search = new SearchServiceClient(accountName, credentials);

                string indexName = AuthToken.GetClaimValue(authToken, Constants.UserAttribute.SearchIndexName);
                if (!string.IsNullOrEmpty(indexName) && !_search.Indexes.Exists(indexName))
                {
                    //Index searchIndex = new Index(indexName, null);
                    //searchIndex = _search.Indexes.Create(searchIndex);
                }
            }
        }
    }
}
