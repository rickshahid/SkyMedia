using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class MonitorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}