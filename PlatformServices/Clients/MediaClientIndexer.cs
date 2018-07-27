using System.Web;
using System.Net.Http;

using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetDownloadUrl(MediaAccount mediaAccount, string storageAccount, Asset asset)
        {
            MediaAsset mediaAsset = new MediaAsset(mediaAccount, asset);
            BlobClient blobClient = new BlobClient(mediaAccount, storageAccount);
            return blobClient.GetDownloadUrl(asset.Container, mediaAsset.FileNames[0], false);
        }

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

        public bool IndexerIsEnabled()
        {
            return !string.IsNullOrEmpty(_indexerAccountId);
        }

        public JArray IndexerGetInsights()
        {
            JArray results;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true);
            WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
            using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl))
            {
                JObject insights = webClient.GetResponse<JObject>(webRequest);
                results = (JArray)insights["results"];
            }
            return results;
        }

        public string IndexerGetCaptionsUrl(string indexId)
        {
            string insightUrl;
            string relativePath = string.Concat("/videos/", indexId, "captions");
            string requestUrl = GetRequestUrl(relativePath, true);
            requestUrl = string.Concat(requestUrl, "?accessToken=", _indexerAccountToken);
            WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
            using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl))
            {
                insightUrl = webClient.GetResponse<string>(webRequest);
            }
            return insightUrl;
        }

        public string IndexerGetInsightsUrl(string indexId)
        {
            string insightUrl;
            string relativePath = string.Concat("/videos/", indexId, "insightsWidget");
            string requestUrl = GetRequestUrl(relativePath, true);
            requestUrl = string.Concat(requestUrl, "?accessToken=", _indexerAccountToken);
            requestUrl = string.Concat(requestUrl, "&allowEdit=true");
            WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
            using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl))
            {
                insightUrl = webClient.GetResponse<string>(webRequest);
            }
            return insightUrl;
        }

        public string IndexerUploadVideo(MediaAccount mediaAccount, string storageAccount, Asset asset, string assetMetadata, bool audioOnly)
        {
            string indexId = string.Empty;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true);
            string videoUrl = GetDownloadUrl(mediaAccount, storageAccount, asset);
            string settingKey = Constant.AppSettingKey.MediaPublishUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            requestUrl = string.Concat(requestUrl, "&externalId=", HttpUtility.UrlEncode(asset.Id));
            requestUrl = string.Concat(requestUrl, "&name=", HttpUtility.UrlEncode(asset.Name));
            if (!string.IsNullOrEmpty(asset.Description))
            {
                requestUrl = string.Concat(requestUrl, "&description=", HttpUtility.UrlEncode(asset.Description));
            }
            if (!string.IsNullOrEmpty(assetMetadata))
            {
                requestUrl = string.Concat(requestUrl, "&metadata=", HttpUtility.UrlEncode(assetMetadata));
            }
            requestUrl = string.Concat(requestUrl, "&videoUrl=", HttpUtility.UrlEncode(videoUrl));
            requestUrl = string.Concat(requestUrl, "&callbackUrl=", HttpUtility.UrlEncode(callbackUrl));
            requestUrl = string.Concat(requestUrl, "&externalUrl=", HttpUtility.UrlEncode(videoUrl));
            requestUrl = string.Concat(requestUrl, "&streamingPreset=NoStreaming");
            if (audioOnly)
            {
                requestUrl = string.Concat(requestUrl, "&indexingPreset=AudioOnly");
            }
            WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
            using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Post, requestUrl))
            {
                JObject index = webClient.GetResponse<JObject>(webRequest);
                if (index != null)
                {
                    indexId = index["id"].ToString();
                }
            }
            return indexId;
        }

        public JObject IndexerGetVideoIndex(string indexId)
        {
            JObject index;
            string relativePath = string.Concat("/videos/", indexId, "index");
            string requestUrl = GetRequestUrl(relativePath, false);
            WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
            using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl))
            {
                index = webClient.GetResponse<JObject>(webRequest);
            }
            return index;

        }

        public void IndexerReindexVideo(string indexId)
        {
            string relativePath = string.Concat("/videos/", indexId, "reindex");
            string requestUrl = GetRequestUrl(relativePath, true);
            string settingKey = Constant.AppSettingKey.MediaPublishUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            callbackUrl = HttpUtility.UrlEncode(callbackUrl);
            WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
            using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Put, requestUrl))
            {
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
            WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey);
            using (HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Delete, requestUrl))
            {
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
            if (deleteIndex)
            {
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    string collectionId = Constant.Database.Collection.OutputInsight;
                    databaseClient.DeleteDocument(collectionId, indexId);
                }
            }
        }
    }
}