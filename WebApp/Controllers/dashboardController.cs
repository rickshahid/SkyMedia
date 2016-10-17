using Microsoft.AspNetCore.Mvc;

using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.ServiceBroker;

namespace SkyMedia.WebApp.Controllers
{
    public class dashboardController : Controller
    {
        public IActionResult processors()
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            ViewData["mediaProcessors"] = mediaClient.GetEntities(EntityType.Processor) as IMediaProcessor[];
            return View();
        }

        public IActionResult index()
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);
            ViewData["entityCounts"] = accountController.GetEntityCounts(authToken);
            ViewData["id"] = this.Request.Query["id"];
            ViewData["name"] = this.Request.Query["name"];
            return View();
        }
    }
}
