using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class TrainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}