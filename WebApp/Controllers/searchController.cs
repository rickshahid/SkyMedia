using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class SearchController : Controller
    {
        public JsonResult Query(string searchQuery)
        {
            try
            {
                JObject searchResults;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    searchResults = mediaClient.IndexerSearch(searchQuery);
                }
                return Json(searchResults);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Index()
        {
            SearchClient searchClient = new SearchClient();
            return View();
        }
    }
}