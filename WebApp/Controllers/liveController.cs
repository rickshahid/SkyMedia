using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class LiveController : Controller
    {
        //[HttpPost]
        //[Route("/live/createEvent")]
        //public string CreateEvent(string channelName, MediaEncoding channelEncoding, MediaProtocol inputProtocol,
        //                          string inputAddressAuthorized, int? inputSubnetPrefixLength,
        //                          string previewAddressAuthorized, int? previewSubnetPrefixLength,
        //                          int? archiveWindowMinutes, bool archiveEncryptionClear,
        //                          bool archiveEncryptionAes, bool archiveEncryptionDrm)
        //{
        //    string channelId = string.Empty;
        //    string authToken = homeController.GetAuthToken(Request, Response);
        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        using MediaClient mediaClient = new MediaClient(authToken);
        //        channelId = mediaClient.CreateChannel(channelName, channelEncoding, inputProtocol, inputAddressAuthorized, inputSubnetPrefixLength, previewAddressAuthorized, previewSubnetPrefixLength, archiveWindowMinutes, archiveEncryptionClear, archiveEncryptionAes, archiveEncryptionDrm);
        //    }
        //    return channelId;
        //}

        //[HttpPost]
        //[Route("/live/insertCue")]
        //public JsonResult InsertCue(string channelName, int durationSeconds, int cueId, bool showSlate)
        //{
        //    string authToken = homeController.GetAuthToken(Request, Response);
        //    using MediaClient mediaClient = new MediaClient(authToken);
        //    bool inserted = mediaClient.InsertMarker(channelName, durationSeconds, cueId, showSlate);
        //    return Json(inserted);
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}