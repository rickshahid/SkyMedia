using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class indexController : Controller
    {
        private string GetIndexerKey()
        {
            string indexerKey = string.Empty;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                string attributeName = Constant.UserAttribute.VideoIndexerKey;
                indexerKey = AuthToken.GetClaimValue(authToken, attributeName);
            }
            return indexerKey;
        }

        [HttpGet]
        [Route("/accounts")]
        public JArray GetAccounts()
        {
            JArray accounts = null;
            string indexerKey = GetIndexerKey();
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                accounts = indexerClient.GetAccounts();
            }
            return accounts;
        }

        [HttpGet]
        [Route("/index")]
        public JObject GetIndex(string indexId, string spokenLanguage, bool processingState)
        {
            JObject index = null;
            string indexerKey = GetIndexerKey();
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                index = indexerClient.GetIndex(indexId, spokenLanguage, processingState);
            }
            return index;
        }

        [HttpGet]
        [Route("/publish")]
        public string PublishIndex(string indexId)
        {
            return MediaClient.PublishIndex(indexId);
        }

        [HttpGet]
        [Route("/webvtt")]
        public string GetWebVttUrl(string indexId, string spokenLanguage)
        {
            string webVttUrl = string.Empty;
            string indexerKey = GetIndexerKey();
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                webVttUrl = indexerClient.GetWebVttUrl(indexId, spokenLanguage);
            }
            return webVttUrl;
        }

        [HttpPost]
        [Route("/delete")]
        public void DeleteVideo(string indexId, bool deleteInsights)
        {
            string indexerKey = GetIndexerKey();
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                indexerClient.DeleteVideo(indexId, deleteInsights);
            }
        }

        [HttpPost]
        [Route("/search")]
        public JArray SearchVideo(string indexId, MediaPrivacy privacy)
        {
            JArray results = null;
            string indexerKey = GetIndexerKey();
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                results = indexerClient.Search(privacy, indexId);
            }
            return results;
        }
    }
}
