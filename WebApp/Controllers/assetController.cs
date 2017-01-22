using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.Services;

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

        public IActionResult index()
        {
            return View();
        }
    }
}
