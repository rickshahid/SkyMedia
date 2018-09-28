using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            SearchClient searchClient = new SearchClient();
            return View();
        }
    }
}