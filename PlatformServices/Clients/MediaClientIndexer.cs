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
        private JObject GetInsight(string insightId)
        {
            JObject insight;
            string requestUrl = GetRequestUrl("/videos/", insightId, "/index");
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                int totalProgress = 0;
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                insight = webClient.GetResponse<JObject>(webRequest);
                JArray videos = (JArray)insight["videos"];
                foreach (JToken video in videos)
                {
                    string videoProcessing = video["processingProgress"].ToString().TrimEnd('%');
                    if (int.TryParse(videoProcessing, out int videoProgress))
                    {
                        totalProgress += videoProgress;
                    }
                }
                insight["processingProgress"] = totalProgress / videos.Count;
            }
            return insight;
        }

        private string GetAccessToken(string insightId)
        {
            string accessToken;
            string settingKey = Constant.AppSettingKey.AzureVideoIndexerApiUrl;
            string requestUrl = AppSetting.GetValue(settingKey);
            requestUrl = string.Concat(requestUrl, "auth/", MediaAccount.VideoIndexerRegion, "/accounts/", _indexerAccountId);
            requestUrl = string.Concat(requestUrl, "/videos/", insightId, "/accessToken?allowEdit=true");
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                accessToken = webClient.GetResponse<string>(webRequest);
            }
            return accessToken;
        }

        private string GetRequestUrl(string parentPath, string insightId, string childPath)
        {
            string settingKey = Constant.AppSettingKey.AzureVideoIndexerApiUrl;
            string requestUrl = AppSetting.GetValue(settingKey);
            requestUrl = string.Concat(requestUrl, MediaAccount.VideoIndexerRegion, "/accounts/", _indexerAccountId);
            if (!string.IsNullOrEmpty(parentPath))
            {
                requestUrl = string.Concat(requestUrl, parentPath);
            }
            if (!string.IsNullOrEmpty(insightId))
            {
                requestUrl = string.Concat(requestUrl, insightId);
            }
            if (!string.IsNullOrEmpty(childPath))
            {
                requestUrl = string.Concat(requestUrl, childPath);
            }
            requestUrl = string.Concat(requestUrl, "?accessToken=");
            if (string.IsNullOrEmpty(insightId))
            {
                requestUrl = string.Concat(requestUrl, _indexerAccountToken);
            }
            else
            {
                requestUrl = string.Concat(requestUrl, GetAccessToken(insightId));
            }
            return requestUrl;
        }

        private string AddRequestParameters(string requestUrl, Priority jobPriority, bool videoIndexing, bool audioIndexing)
        {
            string settingKey = Constant.AppSettingKey.EventGridJobStateFinalUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);
            bool videoIndexingOnly = videoIndexing && !audioIndexing;
            bool audioIndexingOnly = !videoIndexing && audioIndexing;
            requestUrl = string.Concat(requestUrl, "&callbackUrl=", HttpUtility.UrlEncode(callbackUrl));
            requestUrl = string.Concat(requestUrl, "&priority=", jobPriority.ToString());
            requestUrl = string.Concat(requestUrl, "&streamingPreset=NoStreaming");
            if (videoIndexingOnly)
            {
                requestUrl = string.Concat(requestUrl, "&indexingPreset=VideoOnly");
            }
            else if (audioIndexingOnly)
            {
                requestUrl = string.Concat(requestUrl, "&indexingPreset=AudioOnly");
            }
            requestUrl = string.Concat(requestUrl, "&sendSuccessEmail=true");
            return requestUrl;
        }

        public bool IndexerEnabled()
        {
            return !string.IsNullOrEmpty(_indexerAccountId) && !string.IsNullOrEmpty(_indexerAccountToken);
        }

        public void IndexerSetAccount()
        {
            string settingKey = Constant.AppSettingKey.AzureVideoIndexerApiUrl;
            string requestUrl = AppSetting.GetValue(settingKey);
            requestUrl = string.Concat(requestUrl, "auth/", MediaAccount.VideoIndexerRegion, "/accounts?generateAccessTokens=true&allowEdit=true");
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                JArray indexerAccounts = webClient.GetResponse<JArray>(webRequest);
                foreach (JToken indexerAccount in indexerAccounts)
                {
                    string accountId = indexerAccount["id"].ToString();
                    if (accountId.Equals(MediaAccount.VideoIndexerId, StringComparison.OrdinalIgnoreCase))
                    {
                        _indexerAccountId = accountId;
                        _indexerAccountToken = indexerAccount["accessToken"].ToString();
                    }
                }
            }
        }

        public bool IndexerInsightExists(string insightId, out JObject insight)
        {
            insight = null;
            bool insightExists;
            try
            {
                insight = GetInsight(insightId);
                insightExists = insight != null;
            }
            catch
            {
                insightExists = false;
            }
            return insightExists;
        }

        public string IndexerUploadVideo(string inputFileUrl, Asset inputAsset, Priority jobPriority, bool videoIndexing, bool audioIndexing)
        {
            string insightId;
            string requestUrl = GetRequestUrl("/videos", null, null);
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
            requestUrl = AddRequestParameters(requestUrl, jobPriority, videoIndexing, audioIndexing);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Post, requestUrl);
                JObject insight = webClient.GetResponse<JObject>(webRequest);
                insightId = insight["id"].ToString();
            }
            return insightId;
        }

        public void IndexerReindexVideo(string insightId)
        {
            if (IndexerInsightExists(insightId, out JObject insight))
            {
                Priority jobPriority = Priority.Normal;
                bool videoIndexing = true;
                bool audioIndexing = true;
                //string indexingPreset = insight["videos"][0]["indexingPreset"].ToString();
                //bool indexingAudioOnly = indexingPreset == "AudioOnly";
                //bool indexingVideoOnly = indexingPreset == "VideoOnly";
                string requestUrl = GetRequestUrl("/videos/", insightId, "/reindex");
                requestUrl = AddRequestParameters(requestUrl, jobPriority, videoIndexing, audioIndexing);
                using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
                {
                    HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Put, requestUrl);
                    HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
                }
            }
        }

        public void IndexerDeleteVideo(string insightId)
        {
            string requestUrl = GetRequestUrl("/videos/", insightId, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Delete, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
            using (DatabaseClient databaseClient = new DatabaseClient(true))
            {
                string collectionId = Constant.Database.Collection.MediaInsight;
                databaseClient.DeleteDocument(collectionId, insightId);
            }
        }

        public JObject IndexerGetInsights(int? pageSize, int? skipCount)
        {
            JObject insights;
            string requestUrl = GetRequestUrl("/videos", null, null);
            if (pageSize.HasValue)
            {
                requestUrl = string.Concat(requestUrl, "&pageSize=", pageSize.Value);
            }
            if (skipCount.HasValue)
            {
                requestUrl = string.Concat(requestUrl, "&skip=", skipCount.Value);
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
            JObject insights = null;
            JArray allInsights = new JArray();
            do
            {
                int skipCount = 0;
                if (insights != null)
                {
                    skipCount = (int)insights["nextPage"]["skip"];
                }
                insights = IndexerGetInsights(null, skipCount);
                lastPage = (bool)insights["nextPage"]["done"];
                foreach (JToken insight in insights["results"])
                {
                    allInsights.Add(insight);
                }
            } while (!lastPage);
            return allInsights;
        }

        public string IndexerGetInsightUrl(string insightId)
        {
            return GetRequestUrl("/videos/", insightId, "/insightsWidget");
        }

        public string IndexerGetCaptionsUrl(string insightId)
        {
            return GetRequestUrl("/videos/", insightId, "/captions");
        }
    }
}