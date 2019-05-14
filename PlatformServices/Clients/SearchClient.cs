using System;

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal class SearchClient : IDisposable
    {
        private SearchServiceClient _search;

        public SearchClient(string authToken, bool readWrite)
        {
            string[] accountNames = AuthToken.GetClaimValues(authToken, Constant.UserAttribute.SearchAccountName);
            if (accountNames != null)
            {
                string userAttribute = readWrite ? Constant.UserAttribute.SearchAccountKeyReadWrite : Constant.UserAttribute.SearchAccountKeyReadOnly;
                string accountKey = AuthToken.GetClaimValue(authToken, userAttribute);
                SearchCredentials credentials = new SearchCredentials(accountKey);
                _search = new SearchServiceClient(accountNames[0], credentials);
            }
        }

        public string AccountName
        {
            get { return _search == null ? null : _search.SearchServiceName; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _search != null)
            {
                _search.Dispose();
                _search = null;
            }
        }
    }
}