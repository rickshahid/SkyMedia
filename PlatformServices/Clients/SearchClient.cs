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

        public SearchClient(string authToken)
        {
            //string accountName = AuthToken.GetClaimValue(authToken, Constants.UserAttribute.SearchAccountName);
            //string accountKey = AuthToken.GetClaimValue(authToken, Constants.UserAttribute.SearchAccountKey);
            //if (!string.IsNullOrEmpty(accountName))
            //{
            //    SearchCredentials credentials = new SearchCredentials(accountKey);
            //    _search = new SearchServiceClient(accountName, credentials);

            //    string indexName = AuthToken.GetClaimValue(authToken, Constants.UserAttribute.SearchIndexName);
            //    if (!string.IsNullOrEmpty(indexName) && !_search.Indexes.Exists(indexName))
            //    {
            //        Index searchIndex = new Index(indexName, null);
            //        searchIndex = _search.Indexes.Create(searchIndex);
            //    }
            //}
        }
    }
}
