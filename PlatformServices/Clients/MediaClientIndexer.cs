using System.Net;
using System.Net.Http;

using Microsoft.WindowsAzure.MediaServices.Client;

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

        private string GetPrivacy(bool videoPublic)
        {
            return videoPublic ? "public" : "private";
        }

        private string GetAssetId(IAsset asset)
        {
            string assetId = asset.Id;
            if (asset.ParentAssets.Count > 0)
            {
                assetId = asset.ParentAssets[0].Id;
            }
            return assetId;
        }

        private string GetCallbackUrl()
        {
            string settingKey = Constant.AppSettingKey.MediaPublishInsightUrl;
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

        public string GetInsightUrl(string indexId)
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

        public string GetIndexId(IAsset asset)
        {
            string assetId = GetAssetId(asset);
            return GetIndexId(assetId);
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
                JObject searchResults = Search(searchCriteria);
                JArray results = searchResults["results"] as JArray;
                if (results.HasValues)
                {
                    indexId = results.First["id"].ToString();
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

        public string UploadVideo(IAsset asset, string description, string metadata, string spokenLanguage,
                                  string searchPartition, string locatorUrl, bool videoPublic, bool audioOnly)
        {
            string indexId = string.Empty;
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns");
                requestUrl = string.Concat(requestUrl, "?name=", WebUtility.UrlEncode(asset.Name));
                requestUrl = string.Concat(requestUrl, "&description=", WebUtility.UrlEncode(description));
                requestUrl = string.Concat(requestUrl, "&metadata=", WebUtility.UrlEncode(metadata));
                requestUrl = string.Concat(requestUrl, "&externalId=", WebUtility.UrlEncode(asset.Id));
                requestUrl = string.Concat(requestUrl, "&videoUrl=", WebUtility.UrlEncode(locatorUrl));
                requestUrl = string.Concat(requestUrl, "&language=", spokenLanguage);
                requestUrl = string.Concat(requestUrl, "&partition=", searchPartition);
                requestUrl = string.Concat(requestUrl, "&callbackUrl=", GetCallbackUrl());
                requestUrl = string.Concat(requestUrl, "&privacy=", GetPrivacy(videoPublic));
                if (audioOnly)
                {
                    requestUrl = string.Concat(requestUrl, "&indexingPreset=audioOnly");
                }
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Post, requestUrl))
                {
                    indexId = _indexer.GetResponse<string>(request);
                }
            }
            return indexId;
        }

        public void DeleteVideo(string indexId, bool deleteInsight)
        {
            if (_indexer != null)
            {
                string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId);
                if (deleteInsight)
                {
                    requestUrl = string.Concat(requestUrl, "?deleteInsights=true");
                }
                using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Delete, requestUrl))
                {
                    _indexer.GetResponse<object>(request);
                }
            }
        }

        public JObject Search(MediaSearchCriteria searchCriteria)
        {
            JObject results = null;
            if (_indexer != null)
            {
                if (!string.IsNullOrEmpty(searchCriteria.IndexId))
                {
                    results = GetIndex(searchCriteria.IndexId, searchCriteria.SpokenLanguage, false);
                }
                else
                {
                    string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/Search?privacy=", GetPrivacy(searchCriteria.VideoPublic));
                    if (!string.IsNullOrEmpty(searchCriteria.IndexId))
                    {
                        requestUrl = string.Concat(requestUrl, "&id=", searchCriteria.IndexId);
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.AssetId))
                    {
                        requestUrl = string.Concat(requestUrl, "&externalId=", WebUtility.UrlEncode(searchCriteria.AssetId));
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.SpokenLanguage))
                    {
                        requestUrl = string.Concat(requestUrl, "&language=", searchCriteria.SpokenLanguage);
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.TextScope))
                    {
                        requestUrl = string.Concat(requestUrl, "&textScope=", WebUtility.UrlEncode(searchCriteria.TextScope));
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.TextQuery))
                    {
                        requestUrl = string.Concat(requestUrl, "&query=", WebUtility.UrlEncode(searchCriteria.TextQuery));
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.SearchPartition))
                    {
                        requestUrl = string.Concat(requestUrl, "&partition=", searchCriteria.SearchPartition);
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.Owner))
                    {
                        requestUrl = string.Concat(requestUrl, "&owner=", searchCriteria.Owner);
                    }
                    if (!string.IsNullOrEmpty(searchCriteria.Face))
                    {
                        requestUrl = string.Concat(requestUrl, "&face=", searchCriteria.Face);
                    }
                    if (searchCriteria.PageSize > 0)
                    {
                        requestUrl = string.Concat(requestUrl, "&pageSize=", searchCriteria.PageSize.ToString());
                    }
                    if (searchCriteria.SkipCount > 0)
                    {
                        requestUrl = string.Concat(requestUrl, "&skip=", searchCriteria.SkipCount.ToString());
                    }
                    using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
                    {
                        results = _indexer.GetResponse<JObject>(request);
                    }
                }
            }
            return results;
        }
    }
}