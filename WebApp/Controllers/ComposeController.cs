using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class ComposeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}