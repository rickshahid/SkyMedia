using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class channelController : Controller
    {
        [HttpPost]
        [Route("/create")]
        public void Create(string channelName)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                mediaClient.CreateChannel(channelName);
            }
        }

        [HttpPost]
        [Route("/signal")]
        public void Signal(string channelName, int cueId)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                mediaClient.SignalChannel(channelName, cueId);
            }
        }
    }
}
