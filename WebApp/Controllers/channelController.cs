using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    [Route("[controller]")]
    public class channelController : Controller
    {
        [HttpPut]
        [Route("/create")]
        public void Create(string channelName, MediaEncoding encodingType, string allowedAddresses)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                mediaClient.CreateChannel(channelName, encodingType, allowedAddresses);
            }
        }
    }
}
