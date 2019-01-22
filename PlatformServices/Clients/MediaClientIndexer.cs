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
        private string GetRequestUrl(string relativePath, bool authToken, string insightId)
        {
            string accessToken = string.Empty;
            string settingKey = Constant.AppSettingKey.MediaIndexerApiUrl;
            string requestUrl = AppSetting.GetValue(settingKey);
            if (authToken)
            {
                if (string.IsNullOrEmpty(insightId))
                {
                    accessToken = _indexerAccountToken;
                }
                else
                {
                    string authUrl = string.Concat(requestUrl, "auth/", MediaAccount.VideoIndexerRegion, "/accounts/", _indexerAccountId, "/videos/", insightId, "/accessToken?allowEdit=true");
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

        private void SetJobAccount(string insightId)
        {
            MediaJobAccount jobAccount = new MediaJobAccount()
            {
                JobName = insightId,
                MediaAccount = MediaAccount
            };
            using (DatabaseClient databaseClient = new DatabaseClient(true))
            {
                string collectionId = Constant.Database.Collection.MediaJobAccount;
                databaseClient.UpsertDocument(collectionId, jobAccount);
            }
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
                    string accountLocation = indexerAccount["location"].ToString();
                    if (string.Equals(accountLocation, MediaAccount.VideoIndexerRegion, StringComparison.OrdinalIgnoreCase))
                    {
                        _indexerAccountId = indexerAccount["id"].ToString();
                        _indexerAccountToken = indexerAccount["accessToken"].ToString();
                    }
                }
            }
        }

        public string IndexerUploadVideo(MediaAccount mediaAccount, Asset inputAsset, string inputFileUrl, Priority jobPriority,
                                         bool indexingOnly, bool audioOnly, bool videoOnly)
        {
            string insightId = null;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            string settingKey = Constant.AppSettingKey.MediaEventGridPublishUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            if (inputAsset != null)
            {
                requestUrl = string.Concat(requestUrl, "&name=", HttpUtility.UrlEncode(inputAsset.Name));
                requestUrl = string.Concat(requestUrl, "&assetId=", inputAsset.AssetId);
            }
            else
            {
                Uri inputFileUri = new Uri(inputFileUrl);
                string videoName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
                requestUrl = string.Concat(requestUrl, "&name=", HttpUtility.UrlEncode(videoName));
                requestUrl = string.Concat(requestUrl, "&videoUrl=", HttpUtility.UrlEncode(inputFileUrl));
            }
            requestUrl = string.Concat(requestUrl, "&callbackUrl=", HttpUtility.UrlEncode(callbackUrl));
            requestUrl = string.Concat(requestUrl, "&priority=", jobPriority.ToString());
            requestUrl = string.Concat(requestUrl, "&language=auto");
            requestUrl = string.Concat(requestUrl, "&streamingPreset=");
            requestUrl = string.Concat(requestUrl, indexingOnly ? "NoStreaming" : "AdaptiveBitrate");
            if (audioOnly)
            {
                requestUrl = string.Concat(requestUrl, "&indexingPreset=AudioOnly");
            }
            else if (videoOnly)
            {
                requestUrl = string.Concat(requestUrl, "&indexingPreset=VideoOnly");
            }
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Post, requestUrl);
                JObject insight = webClient.GetResponse<JObject>(webRequest);
                if (insight != null)
                {
                    insightId = insight["id"].ToString();
                }
            }
            SetJobAccount(insightId);
            return insightId;
        }

        public void IndexerReindexVideo(string insightId, Priority jobPriority)
        {
            string relativePath = string.Concat("/videos/", insightId, "/reindex");
            string requestUrl = GetRequestUrl(relativePath, true, insightId);
            string settingKey = Constant.AppSettingKey.MediaEventGridPublishUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            callbackUrl = HttpUtility.UrlEncode(callbackUrl);
            requestUrl = string.Concat(requestUrl, "&callbackUrl=", HttpUtility.UrlEncode(callbackUrl));
            requestUrl = string.Concat(requestUrl, "&priority=", jobPriority.ToString());
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Put, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
            SetJobAccount(insightId);
        }

        public void IndexerDeleteVideo(string insightId)
        {
            string relativePath = string.Concat("/videos/", insightId);
            string requestUrl = GetRequestUrl(relativePath, true, insightId);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Delete, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
            using (DatabaseClient databaseClient = new DatabaseClient(true))
            {
                string collectionId = Constant.Database.Collection.MediaJobAccount;
                databaseClient.DeleteDocument(collectionId, insightId);
                collectionId = Constant.Database.Collection.MediaContentInsight;
                databaseClient.DeleteDocument(collectionId, insightId);
            }
        }

        public JObject IndexerSearch(string searchQuery)
        {
            JObject searchResults;
            string relativePath = "/videos/search";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            requestUrl = string.Concat(requestUrl, "&query=", HttpUtility.UrlEncode(searchQuery));
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                searchResults = webClient.GetResponse<JObject>(webRequest);
            }
            return searchResults;
        }

        public JObject IndexerGetInsights(int? pageSize, int? skipPageCount)
        {
            JObject insights;
            string relativePath = "/videos";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            if (pageSize.HasValue)
            {
                requestUrl = string.Concat(requestUrl, "&pageSize=", pageSize.Value);
            }
            if (skipPageCount.HasValue)
            {
                requestUrl = string.Concat(requestUrl, "&skip=", skipPageCount.Value);
            }
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                insights = webClient.GetResponse<JObject>(webRequest);
            }
            return insights;
        }

        public JArray IndexerGetInsights()
        {
            bool lastPage;
            JArray allInsights = new JArray();
            do
            {
                JObject insights = IndexerGetInsights(null, null);
                lastPage = (bool)insights["nextPage"]["done"];
                foreach (JToken insight in insights["results"])
                {
                    allInsights.Add(insight);
                }
            } while (!lastPage);
            return allInsights;
        }

        public JObject IndexerGetInsight(string insightId)
        {
            JObject index;
            string relativePath = string.Concat("/videos/", insightId, "/index");
            string requestUrl = GetRequestUrl(relativePath, true, insightId);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                index = webClient.GetResponse<JObject>(webRequest);
                index["processingProgress"] = index["videos"][0]["processingProgress"];
            }
            return index;
        }

        public string IndexerGetInsightUrl(string insightId)
        {
            string relativePath = string.Concat("/videos/", insightId, "/insightsWidget");
            string requestUrl = GetRequestUrl(relativePath, true, insightId);
            return string.Concat(requestUrl, "&allowEdit=true&version=2");
        }

        public string IndexerGetCaptionsUrl(string insightId)
        {
            string relativePath = string.Concat("/videos/", insightId, "/captions");
            return GetRequestUrl(relativePath, true, insightId);
        }

        public JArray IndexerGetBrands()
        {
            JArray brands;
            string relativePath = "/customization/brands";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                brands = webClient.GetResponse<JArray>(webRequest);
            }
            return brands;
        }

        public JObject IndexerGetBrandSettings()
        {
            JObject brandSettings;
            string relativePath = "/customization/brandsModelSettings";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                brandSettings = webClient.GetResponse<JObject>(webRequest);
            }
            return brandSettings;
        }

        public JArray IndexerGetLanguages()
        {
            JArray languages;
            string relativePath = "/customization/language";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                languages = webClient.GetResponse<JArray>(webRequest);
            }
            return languages;
        }

        public JArray IndexerGetPersons()
        {
            JArray persons;
            string relativePath = "/customization/personModels";
            string requestUrl = GetRequestUrl(relativePath, true, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                persons = webClient.GetResponse<JArray>(webRequest);
            }
            return persons;
        }
    }
}