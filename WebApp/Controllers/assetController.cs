using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        public JsonResult nodes(string assetId, bool getFiles)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = mediaClient.GetAssets(authToken, assetId, getFiles);
            return Json(assets);
        }

        public IActionResult sprite()
        {
            return View();
        }

        public IActionResult index()
        {
            return View();
        }
    }
}