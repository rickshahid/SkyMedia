using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class SearchController : Controller
    {
        public JsonResult Query(string searchQuery)
        {
            JObject searchResults;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                searchResults = mediaClient.IndexerSearch(searchQuery);
            }
            return Json(searchResults);
        }

        public IActionResult Index()
        {
            SearchClient searchClient = new SearchClient();
            return View();
        }
    }
}