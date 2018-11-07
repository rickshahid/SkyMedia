using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class EngagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}