using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class InsightController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}