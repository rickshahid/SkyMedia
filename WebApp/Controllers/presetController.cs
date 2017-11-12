using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class presetController : Controller
    {
        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["mediaProcessor"] = homeController.GetMediaProcessors(authToken, true);
            ViewData["mediaProcessorPreset"] = new SelectListItem[] { };
            return View();
        }
    }
}