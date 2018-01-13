using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        [HttpPut]
        [Route("/asset/publish")]
        public MediaPublished publish(bool? insightQueue, bool? poisonQueue)
        {
            if (!insightQueue.HasValue) insightQueue = false;
            if (!poisonQueue.HasValue) poisonQueue = false;
            string settingKey = insightQueue.Value ? Constant.AppSettingKey.MediaPublishInsightQueue : Constant.AppSettingKey.MediaPublishContentQueue;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue.Value)
            {
                queueName = string.Concat(queueName, Constant.Storage.Queue.PoisonSuffix);
            }
            return insightQueue.Value ? MediaClient.PublishInsight(queueName) : MediaClient.PublishContent(queueName);
        }

        public JsonResult parents()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = mediaClient.GetAssets(authToken, null);
            return Json(assets);
        }

        public JsonResult children(string assetId)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = mediaClient.GetAssets(authToken, assetId);
            return Json(assets);
        }

        //public JsonResult metadata(string documentId, double timeSeconds)
        //{
        //    JObject metadata;
        //    string collectionId = Constant.Database.Collection.ContentInsight;
        //    if (timeSeconds == 0)
        //    {
        //        using (DocumentClient documentClient = new DocumentClient())
        //        {
        //            metadata = documentClient.GetDocument(collectionId, documentId);
        //        }
        //    }
        //    else
        //    {
        //        string procedureId = Constant.Database.Procedure.MetadataFragment;
        //        using (DocumentClient documentClient = new DocumentClient())
        //        {
        //            metadata = documentClient.GetDocument(collectionId, procedureId, documentId, timeSeconds);
        //        }
        //    }
        //    return Json(metadata);
        //}

        //public JsonResult clip(int clipMode, string clipName, string sourceUrl, int markIn, int markOut)
        //{
        //    string authToken = homeController.GetAuthToken(this.Request, this.Response);
        //    MediaClient mediaClient = new MediaClient(authToken);
        //    if (string.IsNullOrEmpty(clipName))
        //    {
        //        clipName = string.Concat(markIn.ToString(), markOut.ToString());
        //    }
        //    object result;
        //    if (clipMode == Constant.Media.RenderedClipMode)
        //    {
        //        string directoryId = homeController.GetDirectoryId(this.Request);
        //        result = MediaClient.SubmitJob(directoryId, authToken, mediaClient, sourceUrl, markIn, markOut);
        //    }
        //    else
        //    {
        //        result = MediaClient.CreateFilter(clipName, mediaClient, sourceUrl, markIn, markOut);
        //    }
        //    return Json(result);
        //}

        public JsonResult streams()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            MediaStream[] streams = Media.GetMediaStreams(authToken, mediaClient);
            return Json(streams);
        }

        public IActionResult clipper()
        {
            return View();
        }

        public IActionResult index()
        {
            return View();
        }
    }
}