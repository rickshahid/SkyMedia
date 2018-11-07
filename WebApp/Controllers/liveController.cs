using Microsoft.AspNetCore.Mvc;

using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class LiveController : Controller
    {
        public JsonResult Create(string eventName, string eventDescription, LiveEventEncodingType encodingType = LiveEventEncodingType.None,
                                 string encodingPresetName = null, LiveEventInputProtocol inputProtocol = LiveEventInputProtocol.FragmentedMP4,
                                 bool lowLatency = false, bool autoStart = false)
        {
            try
            {
                LiveEvent liveEvent;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    liveEvent = mediaClient.CreateLiveEvent(eventName, eventDescription, encodingType, encodingPresetName, inputProtocol, lowLatency, autoStart);
                    string outputName = string.Concat(eventName, Constant.Media.Live.EventOutputNameSuffix);
                    string assetName = string.Concat(eventName, Constant.Media.Live.EventAssetNameSuffix);
                    mediaClient.CreateLiveEventOutput(eventName, outputName, assetName);
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

        public JsonResult Update(string eventName, string eventDescription)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                if (!string.IsNullOrEmpty(authToken))
                {
                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        mediaClient.UpdateLiveEvent(eventName, eventDescription);
                    }
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

        public JsonResult Start(string eventName)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                if (!string.IsNullOrEmpty(authToken))
                {
                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        mediaClient.StartLiveEvent(eventName);
                    }
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
                if (!string.IsNullOrEmpty(authToken))
                {
                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        mediaClient.StopLiveEvent(eventName);
                    }
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
                if (!string.IsNullOrEmpty(authToken))
                {
                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        mediaClient.ResetLiveEvent(eventName);
                    }
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
            return View();
        }
    }
}