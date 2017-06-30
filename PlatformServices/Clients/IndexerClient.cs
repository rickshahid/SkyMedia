using System.Net;
using System.Net.Http;

using Newtonsoft.Json.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public class IndexerClient
    {
        private WebClient _indexer;
        private string _serviceUri;

        public IndexerClient(string indexerKey)
        {
            _indexer = new WebClient(indexerKey);
            string settingKey = Constant.AppSettingKey.AzureMediaIndexer;
            _serviceUri = AppSetting.GetValue(settingKey);
        }

        public static string PublishIndex(string indexId)
        {
            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.IndexPublish;
            string partitionKey = Constant.Storage.TableProperty.PartitionKey;
            MediaIndexPublish indexPublish = entityClient.GetEntity<MediaIndexPublish>(tableName, partitionKey, indexId);

            IndexerClient indexerClient = new IndexerClient(indexPublish.IndexerAccountKey);
            JObject index = indexerClient.GetIndex(indexId, null, false);
            JToken externalId = index["breakdowns"]["externalId"];

            string assetId = string.Empty;
            if (externalId != null)
            {
                assetId = externalId.ToString();
                MediaClient mediaClient = new MediaClient(indexPublish.MediaAccountName, indexPublish.MediaAccountKey);
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                if (asset != null)
                {
                    asset.AlternateId = indexId;
                    asset.Update();
                }
            }
            return assetId;
        }

        public JArray GetAccounts()
        {
            JArray accounts;
            string requestUri = string.Concat(_serviceUri, "/Accounts");
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                accounts = _indexer.GetResponse<JArray>(request);
            }
            return accounts;
        }

        public string GetInsightsUrl(string indexId, string assetId, MediaInsight? insightType)
        {
            string insightsUrl;
            string requestUri = _serviceUri;
            if (!string.IsNullOrEmpty(assetId))
            {
                requestUri = string.Concat(requestUri, "/Breakdowns/GetInsightsWidgetUrlByExternalId?", assetId, "&");
            }
            else
            {
                requestUri = string.Concat(requestUri, "/Breakdowns/", indexId, "/InsightsWidgetUrl?");
            }
            if (insightType.HasValue)
            {
                requestUri = string.Concat(requestUri, insightType.ToString());
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                insightsUrl = _indexer.GetResponse<string>(request);
            }
            return insightsUrl;
        }

        public string GetWebVttUrl(string indexId, string spokenLanguage)
        {
            string webVttUrl;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/", indexId, "/VttUrl");
            if (!string.IsNullOrEmpty(spokenLanguage))
            {
                requestUri = string.Concat(requestUri, "?", spokenLanguage);
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                webVttUrl = _indexer.GetResponse<string>(request);
            }
            return webVttUrl;
        }

        public JObject GetIndex(string indexId, string spokenLanguage, bool processingState)
        {
            JObject index;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/", indexId);
            if (!string.IsNullOrEmpty(spokenLanguage))
            {
                requestUri = string.Concat(requestUri, "?", spokenLanguage);
            }
            else if (processingState)
            {
                requestUri = string.Concat(requestUri, "/State");
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                index = _indexer.GetResponse<JObject>(request);
            }
            return index;
        }

        public void ResetIndex(string indexId, string assetId, string callbackUrl)
        {
            string requestUri = _serviceUri;
            if (!string.IsNullOrEmpty(assetId))
            {
                requestUri = string.Concat(requestUri, "/Breakdowns/reindexbyexternalid/", assetId);
            }
            else
            {
                requestUri = string.Concat(requestUri, "/Breakdowns/reindex/", indexId);
            }
            requestUri = string.Concat(requestUri, "?", callbackUrl);
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Put, requestUri))
            {
                _indexer.GetResponse<JObject>(request);
            }
        }

        public string UploadVideo(string videoName, MediaPrivacy videoPrivacy, string indexerLanguage, string locatorUrl, string callbackUrl, string externalId)
        {
            string indexId;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns");
            requestUri = string.Concat(requestUri, "?name=", videoName);
            requestUri = string.Concat(requestUri, "&privacy=", videoPrivacy.ToString());
            requestUri = string.Concat(requestUri, "&language=", indexerLanguage);
            requestUri = string.Concat(requestUri, "&videoUrl=", WebUtility.UrlEncode(locatorUrl));
            requestUri = string.Concat(requestUri, "&callbackUrl=", WebUtility.UrlEncode(callbackUrl));
            requestUri = string.Concat(requestUri, "&externalId=", externalId);
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Post, requestUri))
            {
                indexId = _indexer.GetResponse<string>(request);
            }
            return indexId;
        }

        public void DeleteVideo(string indexId, bool deleteInsights)
        {
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/", indexId);
            if (deleteInsights)
            {
                requestUri = string.Concat(requestUri, "?deleteInsights");
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Delete, requestUri))
            {
                _indexer.GetResponse<string>(request);
            }
        }

        public JArray Search(MediaPrivacy privacy, string indexId)
        {
            JArray searchResults;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/Search?privacy=", privacy.ToString());
            if (!string.IsNullOrEmpty(indexId))
            {
                requestUri = string.Concat(requestUri, "&id=", indexId);
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                searchResults = _indexer.GetResponse<JArray>(request);
            }
            return searchResults;
        }
    }
}
