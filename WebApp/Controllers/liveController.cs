using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class LiveController : Controller
    {
        internal static SelectListItem[] GetLiveEvents(MediaClient mediaClient)
        {
            List<SelectListItem> liveEvents = new List<SelectListItem>
            {
                new SelectListItem()
            };
            LiveEvent[] mediaLiveEvents = mediaClient.GetAllEntities<LiveEvent>(MediaEntity.LiveEvent);
            foreach (LiveEvent mediaLiveEvent in mediaLiveEvents)
            {
                SelectListItem liveEvent = new SelectListItem()
                {
                    Text = Constant.TextFormatter.FormatValue(mediaLiveEvent.Name),
                    Value = mediaLiveEvent.Name
                };
                liveEvents.Add(liveEvent);
            }
            return liveEvents.ToArray();
        }

        public JsonResult CreateEvent(string eventName, string eventDescription, string eventTags, LiveEventInputProtocol inputProtocol = LiveEventInputProtocol.FragmentedMP4,
                                      LiveEventEncodingType encodingType = LiveEventEncodingType.None, string encodingPresetName = null, string streamingPolicyName = null,
                                      bool lowLatency = false, bool autoStart = false)
        {
            try
            {
                LiveEvent liveEvent;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveEvent = mediaClient.CreateLiveEvent(eventName, eventDescription, eventTags, inputProtocol, encodingType, encodingPresetName, streamingPolicyName, lowLatency, autoStart);
                    string eventOutputName = string.Concat(eventName, Constant.Media.LiveEvent.OutputNameSuffix);
                    string eventOutputAssetName = string.Concat(eventName, Constant.Media.LiveEvent.OutputAssetNameSuffix);
                    CreateOutput(eventName, eventOutputName, null, eventOutputAssetName, Constant.Media.LiveEvent.OutputArchiveWindowMinutes);
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

        public JsonResult CreateOutput(string eventName, string eventOutputName, string eventOutputDescription, string outputAssetName, int archiveWindowMinutes)
        {
            try
            {
                LiveOutput liveOutput;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveOutput = mediaClient.CreateLiveOutput(eventName, eventOutputName, eventOutputDescription, outputAssetName, archiveWindowMinutes);
                }
                return Json(liveOutput);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult UpdateEvent(string eventName, string eventDescription, string eventTags, LiveEventEncodingType encodingType, string encodingPresetName,
                                      string keyFrameIntervalDuration, CrossSiteAccessPolicies crossSiteAccessPolicies)
        {
            try
            {
                LiveEvent liveEvent;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveEvent = mediaClient.UpdateLiveEvent(eventName, eventDescription, eventTags, encodingType, encodingPresetName, keyFrameIntervalDuration, crossSiteAccessPolicies);
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

        public JsonResult StartEvent(string eventName)
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

        public JsonResult StopEvent(string eventName)
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

        public JsonResult ResetEvent(string eventName)
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

        public JsonResult InsertSignal(string eventName, int signalId, int durationSeconds)
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