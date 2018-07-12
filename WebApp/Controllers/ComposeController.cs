using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class ComposeController : Controller
    {
        public JsonResult Streams(string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            MediaStream[] streams = Media.GetClipperStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
            return Json(streams);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}