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

        private IndexerClient()
        {
            string settingKey = Constant.AppSettingKey.MediaIndexerServiceUrl;
            _serviceUrl = AppSetting.GetValue(settingKey);
        }

        public IndexerClient(string authToken) : this()
        {
            User authUser = new User(authToken);
            if (!string.IsNullOrEmpty(authUser.MediaAccount.IndexerKey))
            {
                _indexer = new WebClient(authUser.MediaAccount.IndexerKey);
            }
            _accountId = authUser.MediaAccount.Id;
        }

        public IndexerClient(string accountId, string indexerKey) : this()
        {
            _indexer = new WebClient(indexerKey);
            _accountId = accountId;
        }

        public bool IndexerEnabled
        {
            get { return _indexer != null; }
        }

        private string GetPrivacy(bool videoPublic)
        {
            return videoPublic ? "public" : "private";
        }

        private string GetCallbackUrl()
        {
            string settingKey = Constant.AppSettingKey.MediaPublishInsightUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            callbackUrl = string.Format(callbackUrl, _accountId);
            return WebUtility.UrlEncode(callbackUrl);
        }

        private void PublishInsight(string authToken, string indexId)
        {
            User authUser = new User(authToken);
            MediaPublish insightPublish = new MediaPublish
            {
                PartitionKey = authUser.MediaAccount.Id,
                RowKey = indexId,
                MediaAccount = authUser.MediaAccount
            };
            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.Table.InsightPublish;
            tableClient.UpsertEntity(tableName, insightPublish);
        }

        private string IndexVideo(IAsset asset, string locatorUrl, ContentIndexer contentIndexer)
        {
            string indexId = string.Empty;
            string requestUrl = string.Concat(_serviceUrl, "/Breakdowns");
            requestUrl = string.Concat(requestUrl, "?name=", WebUtility.UrlEncode(asset.Name));
            requestUrl = string.Concat(requestUrl, "&description=", WebUtility.UrlEncode(contentIndexer.VideoDescription));
            requestUrl = string.Concat(requestUrl, "&metadata=", WebUtility.UrlEncode(contentIndexer.VideoMetadata));
            requestUrl = string.Concat(requestUrl, "&externalId=", WebUtility.UrlEncode(asset.Id));
            requestUrl = string.Concat(requestUrl, "&videoUrl=", WebUtility.UrlEncode(locatorUrl));
            requestUrl = string.Concat(requestUrl, "&language=", contentIndexer.LanguageId);
            requestUrl = string.Concat(requestUrl, "&partition=", contentIndexer.SearchPartition);
            requestUrl = string.Concat(requestUrl, "&callbackUrl=", GetCallbackUrl());
            requestUrl = string.Concat(requestUrl, "&privacy=", GetPrivacy(contentIndexer.VideoPublic));
            if (contentIndexer.AudioOnly)
            {
                requestUrl = string.Concat(requestUrl, "&indexingPreset=audioOnly");
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Post, requestUrl))
            {
                indexId = _indexer.GetResponse<string>(request);
            }
            return indexId;
        }

        public static string GetAssetId(JObject index)
        {
            string assetId = string.Empty;
            if (index["breakdowns"] != null && index["breakdowns"].HasValues)
            {
                JToken externalId = index["breakdowns"][0]["externalId"];
                if (externalId != null)
                {
                    assetId = externalId.ToString();
                }
            }
            return assetId;
        }

        public static string GetLanguageLabel(JObject index)
        {
            string languageLabel = string.Empty;
            if (index["breakdowns"] != null && index["breakdowns"].HasValues)
            {
                JToken language = index["breakdowns"][0]["language"];
                if (language != null)
                {
                    string languageId = language.ToString();
                    languageLabel = Language.GetLanguageLabel(languageId, true);
                }
            }
            return languageLabel;
        }

        public JArray GetAccounts()
        {
            JArray accounts = null;
            string requestUrl = string.Concat(_serviceUrl, "/Accounts");
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
            {
                accounts = _indexer.GetResponse<JArray>(request);
            }
            return accounts;
        }

        public string GetInsightUrl(string indexId, bool allowEdit)
        {
            string url = string.Empty;
            string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId, "/InsightsWidgetUrl");
            if (allowEdit)
            {
                requestUrl = string.Concat(requestUrl, "?allowEdit");
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
            {
                url = _indexer.GetResponse<string>(request);
            }
            return url;
        }

        public string GetWebVttUrl(string indexId, string languageId)
        {
            string url = string.Empty;
            string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId, "/VttUrl");
            if (!string.IsNullOrEmpty(languageId))
            {
                requestUrl = string.Concat(requestUrl, "?", languageId);
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
            {
                url = _indexer.GetResponse<string>(request);
            }
            return url;
        }

        public JObject GetIndex(string indexId, string languageId, bool processingState)
        {
            JObject index = null;
            string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId);
            if (!string.IsNullOrEmpty(languageId))
            {
                requestUrl = string.Concat(requestUrl, "?", languageId);
            }
            else if (processingState)
            {
                requestUrl = string.Concat(requestUrl, "/State");
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
            {
                index = _indexer.GetResponse<JObject>(request);
            }
            return index;
        }

        public void ResetIndex(string authToken, string indexId)
        {
            string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/reindex/", indexId);
            requestUrl = string.Concat(requestUrl, "?callbackUrl=", GetCallbackUrl());
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Put, requestUrl))
            {
                _indexer.GetResponse<object>(request);
            }
            PublishInsight(authToken, indexId);
        }

        public void IndexVideo(string authToken, MediaClient mediaClient, IAsset asset, ContentIndexer contentIndexer)
        {
            string locatorUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, asset, null, false);
            string indexId = IndexVideo(asset, locatorUrl, contentIndexer);
            if (!string.IsNullOrEmpty(indexId))
            {
                asset.AlternateId = string.Concat(MediaProcessor.VideoIndexer.ToString(), Constant.TextDelimiter.Identifier, indexId);
                asset.Update();
                PublishInsight(authToken, indexId);
            }
        }

        public void DeleteVideo(string indexId, bool deleteInsight)
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

        public JObject Search(MediaSearchCriteria searchCriteria)
        {
            JObject results = null;
            if (!string.IsNullOrEmpty(searchCriteria.IndexId))
            {
                results = GetIndex(searchCriteria.IndexId, searchCriteria.LanguageId, false);
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
                if (!string.IsNullOrEmpty(searchCriteria.LanguageId))
                {
                    requestUrl = string.Concat(requestUrl, "&language=", searchCriteria.LanguageId);
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
            return results;
        }
    }
}