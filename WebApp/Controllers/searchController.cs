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
            IndexerClient indexerClient = new IndexerClient(mediaClient.MediaAccount);
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