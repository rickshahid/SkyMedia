using System;
using System.Net;
using System.Collections.Generic;

using Microsoft.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Media;
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

        public JsonResult CreateEvent(string eventName, string eventDescription, string eventTags, string inputStreamId,
                                      LiveEventInputProtocol inputProtocol, LiveEventEncodingType encodingType, string encodingPresetName,
                                      string streamingPolicyName, bool lowLatency, bool autoStart)
        {
            try
            {
                LiveEvent liveEvent;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveEvent = mediaClient.CreateLiveEvent(eventName, eventDescription, eventTags, inputStreamId, inputProtocol, encodingType, encodingPresetName, streamingPolicyName, lowLatency, autoStart);
                }
                return Json(liveEvent);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult CreateOutput(string eventName, string eventOutputName, string eventOutputDescription, string outputAssetName,
                                       string streamingPolicyName, int archiveWindowMinutes)
        {
            try
            {
                LiveOutput liveOutput;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveOutput = mediaClient.CreateLiveOutput(eventName, eventOutputName, eventOutputDescription, null, outputAssetName, archiveWindowMinutes);
                    mediaClient.CreateLocator(outputAssetName, outputAssetName, streamingPolicyName, null);
                }
                return Json(liveOutput);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
                    LiveEvent liveEvent = mediaClient.GetEntity<LiveEvent>(MediaEntity.LiveEvent, eventName);
                    MediaLiveEvent mediaLiveEvent = new MediaLiveEvent(mediaClient, liveEvent);
                    if (mediaLiveEvent.Outputs.Length == 0)
                    {
                        string eventOutputName = string.Concat(eventName, Constant.Media.LiveEvent.OutputNameSuffix);
                        string eventOutputAssetName = Guid.NewGuid().ToString();
                        string streamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly;
                        int archiveWindowMinutes = Constant.Media.LiveEvent.OutputArchiveWindowMinutes;
                        CreateOutput(eventName, eventOutputName, null, eventOutputAssetName, streamingPolicyName, archiveWindowMinutes);
                    }
                    mediaClient.StartLiveEvent(eventName);
                }
                return Json(eventName);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
                    LiveEvent liveEvent = mediaClient.GetEntity<LiveEvent>(MediaEntity.LiveEvent, eventName);
                    MediaLiveEvent mediaLiveEvent = new MediaLiveEvent(mediaClient, liveEvent);
                    foreach (LiveOutput liveOutput in mediaLiveEvent.Outputs)
                    {
                        mediaClient.DeleteEntity(MediaEntity.LiveEventOutput, liveOutput.Name, mediaLiveEvent.Name);
                    }
                }
                return Json(eventName);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult InsertSignal(string eventName, string signalId, int signalDurationSeconds)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.InsertLiveEventSignal(eventName, signalId, signalDurationSeconds);
                }
                return Json(eventName);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            string liveEventName = string.Empty;
            bool liveEventEncoding = false;
            bool liveEventLowLatency = false;
            string liveEventPreviewUrl = string.Empty;
            string liveEventOutputUrl = string.Empty;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                LiveEvent[] liveEvents = mediaClient.GetAllEntities<LiveEvent>(MediaEntity.LiveEvent);
                foreach (LiveEvent liveEvent in liveEvents)
                {
                    if (liveEvent.ResourceState == LiveEventResourceState.Running && string.IsNullOrEmpty(liveEventPreviewUrl))
                    {
                        liveEventName = liveEvent.Name;
                        if (liveEvent.Encoding.EncodingType.HasValue && liveEvent.Encoding.EncodingType != LiveEventEncodingType.None)
                        {
                            liveEventEncoding = true;
                        }
                        if (liveEvent.StreamOptions.Contains(StreamOptionsFlag.LowLatency))
                        {
                            liveEventLowLatency = true;
                        }
                        liveEventPreviewUrl = liveEvent.Preview.Endpoints[0].Url;
                        liveEventOutputUrl = mediaClient.GetLiveOutputUrl(liveEvent);
                    }
                }
            }
            ViewData["liveEventName"] = liveEventName;
            ViewData["liveEventEncoding"] = liveEventEncoding;
            ViewData["liveEventLowLatency"] = liveEventLowLatency;
            ViewData["liveEventPreviewUrl"] = liveEventPreviewUrl;
            ViewData["liveEventOutputUrl"] = liveEventOutputUrl;
            return View();
        }
    }
}