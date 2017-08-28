using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        public JsonResult parents()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = mediaClient.GetAssets(null);
            return Json(assets);
        }

        public JsonResult children(string assetId)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = mediaClient.GetAssets(assetId);
            return Json(assets);
        }

        public JsonResult metadata(string documentId, double timeSeconds)
        {
            JObject metadata;
            string collectionId = Constant.Database.Collection.ContentInsight;
            if (timeSeconds == 0)
            {
                documentId = string.Concat(collectionId, Constant.TextDelimiter.Identifier, documentId);
                using (CosmosClient cosmosClient = new CosmosClient(false))
                {
                    metadata = cosmosClient.GetDocument(documentId);
                }
            }
            else
            {
                string procedureId = Constant.Database.Procedure.MetadataFragment;
                using (CosmosClient cosmosClient = new CosmosClient(true))
                {
                    metadata = cosmosClient.ExecuteProcedure(collectionId, procedureId, documentId, timeSeconds);
                }
            }
            return Json(metadata);
        }

        public JsonResult clip(int clipMode, string clipName, string sourceUrl, int markIn, int markOut)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            if (string.IsNullOrEmpty(clipName))
            {
                clipName = string.Concat(markIn.ToString(), markOut.ToString());
            }
            object result;
            if (clipMode == Constant.Media.RenderedClipMode)
            {
                result = MediaClient.SubmitJob(authToken, mediaClient, sourceUrl, markIn, markOut);
            }
            else
            {
                result = MediaClient.CreateFilter(clipName, mediaClient, sourceUrl, markIn, markOut);
            }
            return Json(result);
        }

        public IActionResult index()
        {
            return View();
        }
    }
}