using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class searchController : Controller
    {
        public JsonResult insight(MediaSearchCriteria searchCriteria)
        {
            JObject results = null;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
            results = videoAnalyzer.Search(searchCriteria);
            return Json(results);
        }

        public IActionResult index()
        {
            ViewData["indexerLanguages"] = homeController.GetSpokenLanguages(true, true);
            return View();
        }
    }
}