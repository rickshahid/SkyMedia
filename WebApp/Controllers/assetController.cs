using System;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        public JsonResult roots()
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

        public JsonResult filter(string sourceUrl, string filterName, int markIn, int markOut)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            if (string.IsNullOrEmpty(filterName))
            {
                filterName = Guid.NewGuid().ToString();
            }
            object clipFilter = mediaClient.CreateClip(sourceUrl, filterName, markIn, markOut);
            return Json(clipFilter);
        }

        public IActionResult index()
        {
            return View();
        }
    }
}
