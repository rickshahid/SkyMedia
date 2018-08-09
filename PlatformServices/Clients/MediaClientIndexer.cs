using System.IO;
using System.Web;
using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetRequestUrl(string relativePath, bool accessToken)
        {
            string settingKey = Constant.AppSettingKey.MediaIndexerAuthUrl;
            string requestUrl = AppSetting.GetValue(settingKey);
            requestUrl = requestUrl.Replace("/auth", string.Empty);
            requestUrl = requestUrl.Split('?')[0];
            requestUrl = string.Concat(requestUrl, "/", _indexerAccountId, relativePath);
            if (accessToken)
            {
                requestUrl = string.Concat(requestUrl, "?accessToken=", _indexerAccountToken);
            }
            return requestUrl;
        }

        public void IndexerSetAccountContext()
        {
            if (!string.IsNullOrEmpty(MediaAccount.VideoIndexerKey))
            {
                using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
                {
                    string settingKey = Constant.AppSettingKey.MediaIndexerAuthUrl;
                    string indexerAuthUrl = AppSetting.GetValue(settingKey);
                    HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, indexerAuthUrl);
                    JArray indexerAccounts = webClient.GetResponse<JArray>(webRequest);
                    if (indexerAccounts != null)
                    {
                        _indexerAccountId = indexerAccounts[0]["id"].ToString();
                        _indexerAccountToken = indexerAccounts[0]["accessToken"].ToString();
                    }
                }
            }
        }

        public bool IndexerIsEnabled()
        {
            return !string.IsNullOrEmpty(_indexerAccountId);
        }

        public string IndexerUploadVideo(MediaAccount mediaAccount, string videoUrl, string videoName, string videoDescription, string videoMetadata, bool audioOnly)
        {
            string indexId = null;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true);
            string settingKey = Constant.AppSettingKey.MediaPublishUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            if (string.IsNullOrEmpty(videoName))
            {
                videoName = Path.GetFileNameWithoutExtension(videoUrl);
            }
            requestUrl = string.Concat(requestUrl, "&name=", HttpUtility.UrlEncode(videoName));
            if (!string.IsNullOrEmpty(videoDescription))
            {
                requestUrl = string.Concat(requestUrl, "&description=", HttpUtility.UrlEncode(videoDescription));
            }
            if (!string.IsNullOrEmpty(videoMetadata))
            {
                requestUrl = string.Concat(requestUrl, "&metadata=", HttpUtility.UrlEncode(videoMetadata));
            }
            requestUrl = string.Concat(requestUrl, "&videoUrl=", HttpUtility.UrlEncode(videoUrl));
            requestUrl = string.Concat(requestUrl, "&callbackUrl=", HttpUtility.UrlEncode(callbackUrl));
            requestUrl = string.Concat(requestUrl, "&externalUrl=", HttpUtility.UrlEncode(videoUrl));
            requestUrl = string.Concat(requestUrl, "&streamingPreset=NoStreaming");
            requestUrl = string.Concat(requestUrl, "&language=auto");
            if (audioOnly)
            {
                requestUrl = string.Concat(requestUrl, "&indexingPreset=AudioOnly");
            }
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Post, requestUrl);
                JObject index = webClient.GetResponse<JObject>(webRequest);
                if (index != null)
                {
                    indexId = index["id"].ToString();
                }
            }
            return indexId;
        }

        public void IndexerReindexVideo(string indexId)
        {
            string relativePath = string.Concat("/videos/", indexId, "reindex");
            string requestUrl = GetRequestUrl(relativePath, true);
            string settingKey = Constant.AppSettingKey.MediaPublishUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            callbackUrl = HttpUtility.UrlEncode(callbackUrl);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Put, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
        }

        public void IndexerDeleteVideo(string indexId, bool deleteIndex)
        {
            string relativePath = string.Concat("/videos/", indexId);
            string requestUrl = GetRequestUrl(relativePath, false);
            if (!deleteIndex)
            {
                requestUrl = string.Concat(requestUrl, "/sourceFile");
            }
            requestUrl = string.Concat(requestUrl, "?accessToken=", _indexerAccountToken);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Delete, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
            if (deleteIndex)
            {
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    string collectionId = Constant.Database.Collection.ContentInsight;
                    databaseClient.DeleteDocument(collectionId, indexId);
                }
            }
        }

        public JArray IndexerGetInsights()
        {
            JArray results;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                JObject insights = webClient.GetResponse<JObject>(webRequest);
                results = (JArray)insights["results"];
            }
            return results;
        }

        public JObject IndexerGetInsight(string indexId)
        {
            JObject index;
            string relativePath = string.Concat("/videos/", indexId, "index");
            string requestUrl = GetRequestUrl(relativePath, false);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                index = webClient.GetResponse<JObject>(webRequest);
            }
            return index;

        }

        public string IndexerGetInsightsUrl(string indexId)
        {
            string insightUrl;
            string relativePath = string.Concat("/videos/", indexId, "insightsWidget");
            string requestUrl = GetRequestUrl(relativePath, true);
            requestUrl = string.Concat(requestUrl, "?accessToken=", _indexerAccountToken);
            requestUrl = string.Concat(requestUrl, "&allowEdit=true");
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                insightUrl = webClient.GetResponse<string>(webRequest);
            }
            return insightUrl;
        }

        public string IndexerGetCaptionsUrl(string indexId)
        {
            string insightUrl;
            string relativePath = string.Concat("/videos/", indexId, "captions");
            string requestUrl = GetRequestUrl(relativePath, true);
            requestUrl = string.Concat(requestUrl, "?accessToken=", _indexerAccountToken);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                insightUrl = webClient.GetResponse<string>(webRequest);
            }
            return insightUrl;
        }
    }
}