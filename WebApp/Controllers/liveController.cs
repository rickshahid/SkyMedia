using Microsoft.AspNetCore.Mvc;

using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class LiveController : Controller
    {
        [HttpPost]
        [Route("/live/event/create")]
        public LiveEvent CreateEvent(string eventName, string eventDescription, string inputAccessToken,
                                     LiveEventInputProtocol inputProtocol = LiveEventInputProtocol.FragmentedMP4, LiveEventEncodingType encodingType = LiveEventEncodingType.None,
                                     string encodingPresetName = "Default720p", string previewStreamingPolicyName = "Predefined_ClearStreamingOnly",
                                     string previewAllowedIpAddress = "0.0.0.0", int previewAllowedSubnetPrefixLength = 0, bool lowLatency = false, bool autoStart = false)
        {
            LiveEvent liveEvent = null;
            string authToken = HomeController.GetAuthToken(Request, Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveEvent = mediaClient.CreateLiveEvent(eventName, eventDescription, inputAccessToken, inputProtocol, encodingType, encodingPresetName, previewStreamingPolicyName, previewAllowedIpAddress, previewAllowedSubnetPrefixLength, lowLatency, autoStart);
                }
            }
            return liveEvent;
        }

        [HttpDelete]
        [Route("/live/event/delete")]
        public void DeleteEvent(string eventName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.DeleteEntity(MediaEntity.LiveEvent, eventName);
                }
            }
        }

        [HttpPost]
        [Route("/live/event/start")]
        public void StartEvent(string eventName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.StartLiveEvent(eventName);
                }
            }
        }

        [HttpPost]
        [Route("/live/event/stop")]
        public void StopEvent(string eventName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.StopLiveEvent(eventName);
                }
            }
        }

        [HttpPost]
        [Route("/live/event/output/create")]
        public LiveOutput CreateEventOutput(string eventName, string eventOutputName, string eventOutputDescription, string manifestName, string archiveAssetName, int archiveAssetWindowMinutes = 60)
        {
            LiveOutput liveEventOutput = null;
            string authToken = HomeController.GetAuthToken(Request, Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveEventOutput = mediaClient.CreateLiveEventOutput(eventName, eventOutputName, eventOutputDescription, manifestName, archiveAssetName, archiveAssetWindowMinutes);
                }
            }
            return liveEventOutput;
        }

        [HttpDelete]
        [Route("/live/event/output/delete")]
        public void DeleteEventOutput(string eventName, string eventOutputName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.DeleteEntity(MediaEntity.LiveEventOutput, eventOutputName, eventName);
                }
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}