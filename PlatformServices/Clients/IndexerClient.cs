using System.Net;
using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class IndexerClient
    {
        private WebClient _indexer;
        private string _serviceUrl;
        private string _accountId;

        public IndexerClient(string authToken, string accountId, string indexerKey)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                User authUser = new User(authToken);
                accountId = authUser.MediaAccountId;
                indexerKey = authUser.VideoIndexerKey;
            }

            if (!string.IsNullOrEmpty(indexerKey))
            {
                _indexer = new WebClient(indexerKey);
            }

            string settingKey = Constant.AppSettingKey.MediaIndexerServiceUrl;
            _serviceUrl = AppSetting.GetValue(settingKey);
            _accountId = accountId;
        }

        private string GetPrivacy(bool publicVideo)
        {
            return publicVideo ? "public" : "private";
        }

        private string GetCallbackUrl()
        {
            string settingKey = Constant.AppSettingKey.MediaPublishInsightsUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            callbackUrl = string.Format(callbackUrl, _accountId);
            return WebUtility.UrlEncode(callbackUrl);
        }

        public JArray GetAccounts()
        {
            JArray accounts = null;
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Accounts");
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
                {
                    accounts = _indexer.GetResponse<JArray>(request);
                }
            }
            return accounts;
        }

        public string GetInsightsUrl(string indexId)
        {
            string url = string.Empty;
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId, "/InsightsWidgetUrl");
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
                {
                    url = _indexer.GetResponse<string>(request);
                }
            }
            return url;
        }

        public string GetWebVttUrl(string indexId, string spokenLanguage)
        {
            string url = string.Empty;
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId, "/VttUrl");
                if (!string.IsNullOrEmpty(spokenLanguage))
                {
                    requestUrl = string.Concat(requestUrl, "?", spokenLanguage);
                }
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
                {
                    url = _indexer.GetResponse<string>(request);
                }
            }
            return url;
        }

        public string GetIndexId(string assetId)
        {
            string indexId = string.Empty;
            if (_indexer != null)
            {
                MediaSearchCriteria searchCriteria = new MediaSearchCriteria()
                {
                    AssetId = assetId
                };
                JObject results = Search(searchCriteria);
                if (results != null)
                {

                }
            }
            return indexId;
        }

        public JObject GetIndex(string indexId, string spokenLanguage, bool processingState)
        {
            JObject index = null;
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId);
                if (!string.IsNullOrEmpty(spokenLanguage))
                {
                    requestUrl = string.Concat(requestUrl, "?", spokenLanguage);
                }
                else if (processingState)
                {
                    requestUrl = string.Concat(requestUrl, "/State");
                }
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
                {
                    index = _indexer.GetResponse<JObject>(request);
                }
            }
            return index;
        }

        public void ResetIndex(string indexId)
        {
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/reindex/", indexId);
                requestUrl = string.Concat(requestUrl, "?callbackUrl=", GetCallbackUrl());
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Put, requestUrl))
                {
                    _indexer.GetResponse<object>(request);
                }
            }
        }

        public string UploadVideo(string displayName, bool publicVideo, string transcriptLanguage, string searchPartition, string externalId, string locatorUrl)
        {
            string indexId = string.Empty;
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns");
                requestUrl = string.Concat(requestUrl, "?name=", WebUtility.UrlEncode(displayName));
                requestUrl = string.Concat(requestUrl, "&privacy=", GetPrivacy(publicVideo));
                requestUrl = string.Concat(requestUrl, "&language=", transcriptLanguage);
                requestUrl = string.Concat(requestUrl, "&partition=", searchPartition);
                requestUrl = string.Concat(requestUrl, "&externalId=", WebUtility.UrlEncode(externalId));
                requestUrl = string.Concat(requestUrl, "&videoUrl=", WebUtility.UrlEncode(locatorUrl));
                requestUrl = string.Concat(requestUrl, "&callbackUrl=", GetCallbackUrl());
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Post, requestUrl))
                {
                    indexId = _indexer.GetResponse<string>(request);
                }
            }
            return indexId;
        }

        public void DeleteVideo(string indexId, bool deleteInsights)
        {
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId);
                if (deleteInsights)
                {
                    requestUrl = string.Concat(requestUrl, "?deleteInsights=true");
                }
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Delete, requestUrl))
                {
                    _indexer.GetResponse<object>(request);
                }
            }
        }

        public JObject Search(MediaSearchCriteria serviceCriteria)
        {
            JObject results = null;
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/Search?privacy=", GetPrivacy(serviceCriteria.PublicVideo));
                if (!string.IsNullOrEmpty(serviceCriteria.IndexId))
                {
                    requestUrl = string.Concat(requestUrl, "&id=", serviceCriteria.IndexId);
                }
                if (!string.IsNullOrEmpty(serviceCriteria.AssetId))
                {
                    requestUrl = string.Concat(requestUrl, "&externalId=", WebUtility.UrlEncode(serviceCriteria.AssetId));
                }
                if (!string.IsNullOrEmpty(serviceCriteria.SearchPartition))
                {
                    requestUrl = string.Concat(requestUrl, "&partition=", serviceCriteria.SearchPartition);
                }
                if (!string.IsNullOrEmpty(serviceCriteria.TextScope))
                {
                    requestUrl = string.Concat(requestUrl, "&textScope=", serviceCriteria.TextScope);
                }
                if (!string.IsNullOrEmpty(serviceCriteria.TextQuery))
                {
                    requestUrl = string.Concat(requestUrl, "&query=", serviceCriteria.TextQuery);
                }
                if (!string.IsNullOrEmpty(serviceCriteria.Owner))
                {
                    requestUrl = string.Concat(requestUrl, "&owner=", serviceCriteria.Owner);
                }
                if (!string.IsNullOrEmpty(serviceCriteria.Face))
                {
                    requestUrl = string.Concat(requestUrl, "&face=", serviceCriteria.Face);
                }
                if (!string.IsNullOrEmpty(serviceCriteria.Language))
                {
                    requestUrl = string.Concat(requestUrl, "&language=", serviceCriteria.Language);
                }
                if (serviceCriteria.PageSize > 0)
                {
                    requestUrl = string.Concat(requestUrl, "&pageSize=", serviceCriteria.PageSize.ToString());
                }
                if (serviceCriteria.SkipCount > 0)
                {
                    requestUrl = string.Concat(requestUrl, "&skip=", serviceCriteria.SkipCount.ToString());
                }
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
                {
                    results = _indexer.GetResponse<JObject>(request);
                }
            }
            return results;
        }
    }
}