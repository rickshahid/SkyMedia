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

        internal static SelectListItem[] GetListItems(Dictionary<string, string> dictionary)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                SelectListItem listItem = new SelectListItem()
                {
                    Text = item.Value,
                    Value = item.Key
                };
                listItems.Add(listItem);
            }
            return listItems.ToArray();
        }

        public static string GetAppSetting(string settingKey)
        {
            return AppSetting.GetValue(settingKey);
        }

        public static string GetAuthToken(HttpRequest request, HttpResponse response)
        {
            string authToken = string.Empty;
            string tokenKey = Constant.AuthIntegration.TokenKey;
            if (request.HasFormContentType)
            {
                authToken = request.Form[tokenKey];
                if (!string.IsNullOrEmpty(authToken))
                {
                    response.Cookies.Append(tokenKey, authToken);
                }
            }
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = request.Cookies[tokenKey];
            }
            return authToken;
        }

        public static void SetAccountContext(string authToken, ViewDataDictionary viewData)
        {
            User currentUser = new User(authToken);
            viewData["userId"] = currentUser.Id;
            viewData["accountName"] = currentUser.MediaAccountPrimary.Name;
            if (string.IsNullOrEmpty(currentUser.MediaAccountPrimary.VideoIndexerRegion) || string.IsNullOrEmpty(currentUser.MediaAccountPrimary.VideoIndexerKey))
            {
                viewData["indexerMessage"] = "DisplayMessage('Azure Video Indexer Account', 'Your Azure Media Services account does not have an Azure Video Indexer account.<br><br>Verify your " + viewData["appName"] + " user account profile is configured.')";
            }
        }

        public IActionResult Index()
        {
            string userMessage = string.Empty;
            MediaStream[] mediaStreams = new MediaStream[] { };

            int streamNumber = 1;
            if (Request.Query.ContainsKey("stream"))
            {
                streamNumber = int.Parse(Request.Query["stream"]);
            }

            string settingKey = Constant.AppSettingKey.MediaStreamTunerPageSize;
            string tunerPageSize = AppSetting.GetValue(settingKey);
            int streamTunerPageSize = int.Parse(tunerPageSize);

            int streamSkipCount = 0;
            bool streamTunerLastPage = true;

            try
            {
                string authToken = GetAuthToken(Request, Response);

                //CognitiveClient.GetSpeech("Hello Friend", false);
                //CognitiveClient.GetSpeech("Hello Friend", true);

                if (Request.HasFormContentType)
                {
                    RedirectToActionResult redirectAction = Startup.OnSignIn(this);
                    if (redirectAction != null)
                    {
                        return redirectAction;
                    }
                }

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
                            userMessage = string.Format(Constant.Message.StreamingEndpointNotStarted, mediaClient.MediaAccount.Name);
                        }
                        else
                        {
                            mediaStreams = Media.GetAccountStreams(authToken, mediaClient, streamNumber, streamTunerPageSize, out streamSkipCount, out streamTunerLastPage);
                        }
                    }
                }
            }
            catch (ApiErrorException apiEx)
            {
                userMessage = string.Concat(apiEx.Message, " (", apiEx.Response.Content, ")");
            }
            catch (Exception ex)
            {
                userMessage = ex.Message;
            }

            ViewData["mediaStreams"] = mediaStreams;
            ViewData["streamNumber"] = streamNumber;

            ViewData["streamSkipCount"] = streamSkipCount;
            ViewData["streamTunerPageSize"] = streamTunerPageSize;
            ViewData["streamTunerLastPage"] = streamTunerLastPage;

            ViewData["userMessage"] = userMessage;

            return View();
        }
    }
}