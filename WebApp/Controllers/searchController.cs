using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}