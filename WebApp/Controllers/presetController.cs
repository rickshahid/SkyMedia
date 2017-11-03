using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class presetController : Controller
    {
        public IActionResult index()
        {
            return View();
        }
    }
}