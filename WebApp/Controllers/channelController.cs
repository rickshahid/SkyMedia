using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class channelController : Controller
    {
        [HttpPost]
        [Route("/channel/create")]
        public string Create(string channelName, MediaEncoding channelEncoding, MediaProtocol inputProtocol,
                             string inputAddressAuthorized, int? inputSubnetPrefixLength,
                             string previewAddressAuthorized, int? previewSubnetPrefixLength,
                             int? archiveWindowMinutes, bool? archiveEncryption)
        {
            if (!archiveEncryption.HasValue) archiveEncryption = false;
            string channelId = string.Empty;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                channelId = mediaClient.CreateChannel(channelName, channelEncoding, inputProtocol, inputAddressAuthorized, inputSubnetPrefixLength, previewAddressAuthorized, previewSubnetPrefixLength, archiveWindowMinutes, archiveEncryption.Value);
            }
            return channelId;
        }
    }
}