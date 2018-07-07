using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class BrowseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}