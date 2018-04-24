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
                             int? archiveWindowMinutes, bool archiveEncryptionClear,
                             bool archiveEncryptionAes, bool archiveEncryptionDrm)
        {
            string channelId = string.Empty;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                channelId = mediaClient.CreateChannel(channelName, channelEncoding, inputProtocol, inputAddressAuthorized, inputSubnetPrefixLength, previewAddressAuthorized, previewSubnetPrefixLength, archiveWindowMinutes, archiveEncryptionClear, archiveEncryptionAes, archiveEncryptionDrm);
            }
            return channelId;
        }

        [HttpPost]
        [Route("/channel/mark")]
        public JsonResult Mark(string channelName, int durationSeconds, int cueId, bool showSlate)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            bool inserted = mediaClient.InsertMarker(channelName, durationSeconds, cueId, showSlate);
            return Json(inserted);
        }
    }
}