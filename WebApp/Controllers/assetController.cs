using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

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

        public JsonResult assets(string assetId, bool getFiles)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = mediaClient.GetAssets(authToken, assetId, getFiles);
            return Json(assets);
        }

        public JsonResult streams(string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaStream[] streams = Media.GetMediaStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
            return Json(streams);
        }

        public JsonResult clip(string clipData)
        {
            JObject clip = JObject.Parse(clipData);
            return Json(clip);
        }

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