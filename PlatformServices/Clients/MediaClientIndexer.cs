using System;
using System.IO;
using System.Web;
using System.Net.Http;

using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetRequestUrl(string relativePath, bool authToken, string indexId)
        {
            string accessToken = string.Empty;
            string settingKey = Constant.AppSettingKey.MediaIndexerApiUrl;
            string requestUrl = AppSetting.GetValue(settingKey);
            if (authToken)
            {
                if (string.IsNullOrEmpty(indexId))
                {
                    accessToken = _indexerAccountToken;
                }
                else
                {
                    string authUrl = string.Concat(requestUrl, "auth/", MediaAccount.VideoIndexerRegion, "/accounts/", _indexerAccountId, "/videos/", indexId, "/accessToken?allowEdit=true");
                    using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
                    {
                        HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, authUrl);
                        accessToken = webClient.GetResponse<string>(webRequest);
                    }
                }
            }
            requestUrl = string.Concat(requestUrl, MediaAccount.VideoIndexerRegion, "/accounts/", _indexerAccountId, relativePath);
            if (authToken)
            {
                requestUrl = string.Concat(requestUrl, "?accessToken=", accessToken);
            }
            return requestUrl;
        }

        public bool IndexerEnabled()
        {
            return !string.IsNullOrEmpty(_indexerAccountId);
        }

        public void IndexerSetAccountContext()
        {
            string settingKey = Constant.AppSettingKey.MediaIndexerApiUrl;
            string requestUrl = AppSetting.GetValue(settingKey);
            requestUrl = string.Concat(requestUrl, "auth/", MediaAccount.VideoIndexerRegion, "/accounts?generateAccessTokens=true&allowEdit=true");
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                JArray indexerAccounts = webClient.GetResponse<JArray>(webRequest);
                foreach (JToken indexerAccount in indexerAccounts)
                {
                    if (string.Equals(indexerAccount["location"].ToString(), MediaAccount.VideoIndexerRegion, StringComparison.OrdinalIgnoreCase))
                    {
                        _indexerAccountId = indexerAccount["id"].ToString();
                        _indexerAccountToken = indexerAccount["accessToken"].ToString();
                    }
                }
            }
        }

        public string IndexerUploadVideo(MediaAccount mediaAccount, Asset inputAsset, string inputFileUrl, bool audioOnly)
        {
            string indexId = null;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            string settingKey = Constant.AppSettingKey.MediaPublishJobUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            if (!string.IsNullOrEmpty(inputFileUrl))
            {
                Uri inputFileUri = new Uri(inputFileUrl);
                string videoName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
                requestUrl = string.Concat(requestUrl, "&name=", HttpUtility.UrlEncode(videoName));
                requestUrl = string.Concat(requestUrl, "&videoUrl=", HttpUtility.UrlEncode(inputFileUrl));
            }
            else if (inputAsset != null)
            {
                requestUrl = string.Concat(requestUrl, "&name=", HttpUtility.UrlEncode(inputAsset.Name));
                requestUrl = string.Concat(requestUrl, "&assetId=", inputAsset.AssetId);
            }
            requestUrl = string.Concat(requestUrl, "&callbackUrl=", HttpUtility.UrlEncode(callbackUrl));
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
            string relativePath = string.Concat("/videos/", indexId, "/reindex");
            string requestUrl = GetRequestUrl(relativePath, true, indexId);
            string settingKey = Constant.AppSettingKey.MediaPublishJobUrl;
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
            if (!deleteIndex)
            {
                relativePath = string.Concat(relativePath, "/sourceFile");
            }
            string requestUrl = GetRequestUrl(relativePath, true, indexId);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Delete, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
            if (deleteIndex)
            {
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    string collectionId = Constant.Database.Collection.MediaInsight;
                    databaseClient.DeleteDocument(collectionId, indexId);
                }
            }
        }

        public JArray IndexerGetInsights()
        {
            JArray insights;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                JObject videos = webClient.GetResponse<JObject>(webRequest);
                insights = (JArray)videos["results"];
            }
            return insights;
        }

        public JObject IndexerGetInsight(string indexId)
        {
            JObject index;
            string relativePath = string.Concat("/videos/", indexId, "/index");
            string requestUrl = GetRequestUrl(relativePath, true, indexId);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                index = webClient.GetResponse<JObject>(webRequest);
            }
            return index;
        }

        public string IndexerGetInsightUrl(string indexId)
        {
            string relativePath = string.Concat("/videos/", indexId, "/insightsWidget");
            string requestUrl = GetRequestUrl(relativePath, true, indexId);
            return string.Concat(requestUrl, "&allowEdit=true&version=2");
        }

        public string IndexerGetCaptionsUrl(string indexId)
        {
            string relativePath = string.Concat("/videos/", indexId, "/captions");
            return GetRequestUrl(relativePath, true, indexId);
        }
    }
}