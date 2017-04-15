using System;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        public JsonResult parents()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            MediaAsset[] assets = mediaClient.GetAssets(null);
            return Json(assets);
        }

        public JsonResult children(string assetId)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            MediaAsset[] assets = mediaClient.GetAssets(assetId);
            return Json(assets);
        }

        public JsonResult clip(int clipMode, string clipName, string sourceUrl, int markIn, int markOut)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            if (string.IsNullOrEmpty(clipName))
            {
                clipName = Guid.NewGuid().ToString();
            }
            object result;
            if (clipMode == Constant.Media.RenderedClipMode)
            {
                result = Editor.SubmitJob(authToken, mediaClient, sourceUrl, markIn, markOut);
            }
            else
            {
                result = Editor.CreateFilter(clipName, mediaClient, sourceUrl, markIn, markOut);
            }
            return Json(result);
        }

        public IActionResult index()
        {
            return View();
        }
    }
}
