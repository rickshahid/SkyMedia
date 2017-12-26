using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class insightController : Controller
    {
        public static string GetProcessorName(MediaProcessor mediaProcessor)
        {
            return Processor.GetProcessorName(mediaProcessor);
        }

        public JsonResult metadata(MediaProcessor mediaProcessor, string documentId, double timeSeconds)
        {
            JObject metadata;
            string collectionId = Constant.Database.Collection.ContentInsight;
            using (DocumentClient documentClient = new DocumentClient())
            {
                metadata = documentClient.GetDocument(collectionId, documentId);
            }
            return Json(metadata);
        }

        [HttpGet]
        [Route("/insight/accounts")]
        public JArray GetAccounts()
        {
            JArray accounts = null;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                IndexerClient indexerClient = new IndexerClient(authToken);
                if (indexerClient.IndexerEnabled)
                {
                    accounts = indexerClient.GetAccounts();
                }
            }
            return accounts;
        }

        [HttpGet]
        [Route("/insight/get")]
        public JObject GetInsight(string indexId, string languageId, bool processingState)
        {
            JObject index = null;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                IndexerClient indexerClient = new IndexerClient(authToken);
                if (indexerClient.IndexerEnabled)
                {
                    index = indexerClient.GetIndex(indexId, languageId, processingState);
                }
            }
            return index;
        }

        [HttpDelete]
        [Route("/insight/delete")]
        public void DeleteInsight(string indexId)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                IndexerClient indexerClient = new IndexerClient(authToken);
                if (indexerClient.IndexerEnabled)
                {
                    indexerClient.DeleteVideo(indexId, true);
                }

                DocumentClient documentClient = new DocumentClient();
                string collectionId = Constant.Database.Collection.ContentInsight;
                documentClient.DeleteDocument(collectionId, indexId);
            }
        }
    }
}