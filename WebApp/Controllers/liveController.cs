using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class LiveController : Controller
    {
        public JsonResult Create(string eventName, string eventDescription, string eventTags, LiveEventInputProtocol inputProtocol = LiveEventInputProtocol.FragmentedMP4,
                                 LiveEventEncodingType encodingType = LiveEventEncodingType.None, string encodingPresetName = null,
                                 string streamingPolicyName = null, int dvrMinutes = 180, bool lowLatency = false, bool autoStart = false)
        {
            try
            {
                LiveEvent liveEvent;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {

                    liveEvent = mediaClient.CreateLiveEvent(eventName, eventDescription, eventTags, inputProtocol, encodingType, encodingPresetName, streamingPolicyName, lowLatency, autoStart);
                    string outputName = string.Concat(eventName, Constant.Media.Live.EventOutputNameSuffix);
                    string assetName = string.Concat(eventName, Constant.Media.Live.EventAssetNameSuffix);
                    mediaClient.CreateLiveEventOutput(eventName, outputName, assetName, dvrMinutes);
                }
                return Json(liveEvent);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Update(string eventName, string eventDescription, string eventTags, string encodingPresetName,
                                 string keyFrameIntervalDuration, CrossSiteAccessPolicies crossSiteAccessPolicies)
        {
            try
            {
                LiveEvent liveEvent;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveEvent = mediaClient.UpdateLiveEvent(eventName, eventDescription, eventTags, encodingPresetName, keyFrameIntervalDuration, crossSiteAccessPolicies);
                }
                return Json(liveEvent);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Start(string eventName)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.StartLiveEvent(eventName);
                }
                return Json(eventName);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Stop(string eventName)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.StopLiveEvent(eventName);
                }
                return Json(eventName);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Reset(string eventName)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.ResetLiveEvent(eventName);
                }
                return Json(eventName);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Signal(string eventName, int signalId, int durationSeconds)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.InsertLiveEventSignal(eventName, signalId, durationSeconds);
                }
                return Json(eventName);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Index()
        {
            bool liveEncoding = false;
            string liveEventUrl = string.Empty;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                LiveEvent[] liveEvents = mediaClient.GetAllEntities<LiveEvent>(MediaEntity.LiveEvent);
                foreach (LiveEvent liveEvent in liveEvents)
                {
                    if (liveEvent.Encoding.EncodingType.HasValue && liveEvent.Encoding.EncodingType != LiveEventEncodingType.None)
                    {
                        liveEncoding = true;
                    }
                    if (liveEvent.ResourceState == LiveEventResourceState.Running)
                    {
                        liveEventUrl = liveEvent.Preview.Endpoints[0].Url;
                    }

                }
            }
            ViewData["liveEncoding"] = liveEncoding;
            ViewData["liveEventUrl"] = liveEventUrl;
            return View();
        }
    }
}