using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class searchController : Controller
    {
        public JsonResult insights(MediaSearchCriteria searchCriteria)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            IndexerClient indexerClient = new IndexerClient(authToken, null, null);
            JObject results = indexerClient.Search(searchCriteria);
            return Json(results);
        }

        public IActionResult index()
        {
            ViewData["indexerLanguages"] = homeController.GetSpokenLanguages(true, true);
            return View();
        }
    }
}