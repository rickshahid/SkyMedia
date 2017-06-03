using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class analyticsController : Controller
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

        public JsonResult metadata(string documentId, double timeSeconds)
        {
            JObject metadataFragment;
            string collectionId = Constant.Database.Collection.Metadata;
            string procedureId = Constant.Database.Procedure.MetadataFragment;
            using (CosmosClient cosmosClient = new CosmosClient(true))
            {
                metadataFragment = cosmosClient.ExecuteProcedure(collectionId, procedureId, documentId, timeSeconds);
            }
            return Json(metadataFragment);
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
        public JObject GetIndex(string indexId, bool processingState)
        {
            JObject index = null;
            string indexerKey = GetIndexerKey();
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                index = indexerClient.GetIndex(indexId, processingState);
            }
            return index;
        }

        [HttpGet]
        [Route("/webvtt")]
        public string GetWebVttUrl(string indexId, string languageCode)
        {
            string webVttUrl = string.Empty;
            string indexerKey = GetIndexerKey();
            if (!string.IsNullOrEmpty(indexerKey))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                webVttUrl = indexerClient.GetWebVttUrl(indexId, languageCode);
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
