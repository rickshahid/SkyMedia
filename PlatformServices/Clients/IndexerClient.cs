using System.Net;
using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public class IndexerClient
    {
        private WebClient _indexer;
        private string _serviceUri;

        public delegate string UploadVideo(string name, MediaPrivacy privacy, string locatorUrl);

        public IndexerClient(string authKey)
        {
            _indexer = new WebClient(authKey);

            string settingKey = Constant.AppSettingKey.AzureMediaIndexer;
            _serviceUri = AppSetting.GetValue(settingKey);
        }

        public JArray GetAccounts()
        {
            JArray response;
            string requestUri = string.Concat(_serviceUri, "/Accounts");
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                response = _indexer.GetResponse<JArray>(request);
            }
            return response;
        }

        public string GetWidgetUrl(string indexId, MediaInsight? insightType)
        {
            string response;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/", indexId);
            if (!insightType.HasValue)
            {
                requestUri = string.Concat(requestUri, "/PlayerWidgetUrl");
            }
            else
            {
                requestUri = string.Concat(requestUri, "/InsightsWidgetUrl?", insightType.ToString());
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                response = _indexer.GetResponse<string>(request);
            }
            return response;
        }

        public string GetWebVttUrl(string indexId, string languageCode)
        {
            string response;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/", indexId, "/VttUrl");
            if (!string.IsNullOrEmpty(languageCode))
            {
                requestUri = string.Concat(requestUri, "?", languageCode);
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                response = _indexer.GetResponse<string>(request);
            }
            return response;
        }

        public JObject GetIndex(string indexId, bool processingState)
        {
            JObject response;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/", indexId);
            if (processingState)
            {
                requestUri = string.Concat(requestUri, "/State");
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                response = _indexer.GetResponse<JObject>(request);
            }
            return response;
        }

        public string GetIndexId(string name, MediaPrivacy privacy, string locatorUrl)
        {
            string response;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns");
            requestUri = string.Concat(requestUri, "?name=", name);
            requestUri = string.Concat(requestUri, "&privacy=", privacy.ToString());
            requestUri = string.Concat(requestUri, "&videoUrl=", WebUtility.UrlEncode(locatorUrl));
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Post, requestUri))
            {
                response = _indexer.GetResponse<string>(request);
            }
            return response;
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
            JArray response;
            string requestUri = string.Concat(_serviceUri, "/Breakdowns/Search?privacy=", privacy.ToString());
            if (!string.IsNullOrEmpty(indexId))
            {
                requestUri = string.Concat(requestUri, "&id=", indexId);
            }
            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUri))
            {
                response = _indexer.GetResponse<JArray>(request);
            }
            return response;
        }
    }
}
