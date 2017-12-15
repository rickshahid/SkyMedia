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
            IndexerClient indexerClient = new IndexerClient(authToken);
            if (indexerClient.IndexerEnabled)
            {
                results = indexerClient.Search(searchCriteria);
            }
            return Json(results);
        }

        public IActionResult index()
        {
            ViewData["indexerLanguages"] = homeController.GetSpokenLanguages(true, true);
            return View();
        }
    }
}