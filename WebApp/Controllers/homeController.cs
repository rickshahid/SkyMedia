using System;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private static bool IsStreamingEnabled(MediaClient mediaClient)
        {
            bool streamingEnabled = false;
            IPage<StreamingEndpoint> streamingEndpoints = mediaClient.GetEntities<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            foreach (StreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.ResourceState == StreamingEndpointResourceState.Starting ||
                    streamingEndpoint.ResourceState == StreamingEndpointResourceState.Running ||
                    streamingEndpoint.ResourceState == StreamingEndpointResourceState.Scaling)
                {
                    streamingEnabled = true;
                }
            }
            return streamingEnabled;
        }

        //public JsonResult Metadata(MediaProcessor mediaProcessor, string documentId, double timeSeconds)
        //{
        //    JObject metadata;
        //    string collectionId = Constant.Database.Collection.OutputInsight;
        //    string procedureId = Constant.Database.Procedure.TimecodeFragment;
        //    using (DatabaseClient databaseClient = new DatabaseClient())
        //    {
        //        if (mediaProcessor == MediaProcessor.VideoAnalyzer)
        //        {
        //            metadata = databaseClient.GetDocument(collectionId, documentId);
        //        }
        //        else
        //        {
        //            metadata = databaseClient.GetDocument(collectionId, procedureId, documentId, timeSeconds);
        //        }
        //    }
        //    return Json(metadata);
        //}

        internal static SelectListItem[] GetListItems(Dictionary<string, string> dictionary)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                SelectListItem listItem = new SelectListItem()
                {
                    Text = item.Value.ToString(),
                    Value = item.Key.ToString()
                };
                listItems.Add(listItem);
            }
            return listItems.ToArray();
        }

        internal static void SetViewData(string authToken, ViewDataDictionary viewData)
        {
        //    User authUser = new User(authToken);
        //    viewData["jobName"] = GetJobTemplates(authToken);
        //    viewData["mediaProcessor1"] = GetMediaProcessors(authToken, false);
        //    viewData["encoderConfig1"] = new List<SelectListItem>();
        //    viewData["encoderStandardPresets"] = presetController.GetProcessorPresets(MediaProcessor.EncoderStandard, authUser.MediaAccount.Name, true);
        //    viewData["encoderPremiumPresets"] = presetController.GetProcessorPresets(MediaProcessor.EncoderPremium, authUser.MediaAccount.Name, false);
        }

        public static string GetAuthToken(HttpRequest request, HttpResponse response)
        {
            string authToken = string.Empty;
            string cookieKey = Constant.HttpCookie.UserAuthToken;
            if (request.HasFormContentType)
            {
                authToken = request.Form[Constant.HttpForm.IdToken];
                if (!string.IsNullOrEmpty(authToken))
                {
                    response.Cookies.Append(cookieKey, authToken);
                }
            }
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = request.Cookies[cookieKey];
            }
            return authToken;
        }

        public static void SetAccountContext(string authToken, ViewDataDictionary viewData)
        {
            User authUser = new User(authToken);
            viewData["userId"] = authUser.Id;
            viewData["accountName"] = authUser.MediaAccount.Name;
        }

        public static string GetAppSetting(string settingKey)
        {
            return AppSetting.GetValue(settingKey);
        }

        public IActionResult Index()
        {
            string accountMessage = string.Empty;
            MediaStream[] mediaStreams = new MediaStream[] { };

            string authToken = GetAuthToken(Request, Response);
            string queryString = Request.QueryString.Value.ToLower();

            if (Request.HasFormContentType)
            {
                try
                {
                    RedirectToActionResult redirectAction = Startup.OnSignIn(this, authToken);
                    if (redirectAction != null)
                    {
                        return redirectAction;
                    }
                }
                catch (Exception ex)
                {
                    accountMessage = ex.ToString();
                }
            }

            int streamNumber = 1;
            string autoPlay = "false";
            if (queryString.Contains("stream"))
            {
                streamNumber = int.Parse(Request.Query["stream"]);
                autoPlay = "true";
            }

            int streamOffset = 0;
            int streamIndex = streamNumber - 1;
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    mediaStreams = Media.GetSampleStreams();
                }
                else
                {
                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        if (!IsStreamingEnabled(mediaClient))
                        {
                            accountMessage = Constant.Message.StreamingEndpointNotStarted;
                        }
                        else
                        {
                            mediaStreams = Media.GetAccountStreams(authToken, mediaClient, streamNumber, out streamOffset, out streamIndex, out bool endOfStreams);
                            if (endOfStreams)
                            {
                                streamNumber = streamNumber - 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                accountMessage = ex.ToString();
            }
            ViewData["accountMessage"] = accountMessage;

            ViewData["mediaStreams"] = mediaStreams;
            ViewData["streamNumber"] = streamNumber;

            ViewData["streamOffset"] = streamOffset;
            ViewData["streamIndex"] = streamIndex;

            ViewData["languageCode"] = Request.Query["language"];
            ViewData["autoPlay"] = autoPlay;

            return View();
        }
    }
}