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

        public IActionResult Index()
        {
            string accountMessage = string.Empty;
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
            bool streamLastPage = false;

            try
            {
                string authToken = GetAuthToken(Request, Response);

                if (Request.HasFormContentType)
                {
                    RedirectToActionResult redirectAction = Startup.OnSignIn(this, authToken);
                    if (redirectAction != null)
                    {
                        return redirectAction;
                    }
                }

                if (string.IsNullOrEmpty(authToken))
                {
                    mediaStreams = Media.GetSampleStreams();
                    streamLastPage = true;
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
                            mediaStreams = Media.GetAccountStreams(authToken, mediaClient, streamNumber, streamTunerPageSize, out streamSkipCount, out streamLastPage);
                        }
                    }
                }
            }
            catch (ApiErrorException apiEx)
            {
                accountMessage = string.Concat(apiEx.Response.Content, apiEx.ToString());
            }
            catch (Exception ex)
            {
                accountMessage = ex.ToString();
            }

            ViewData["mediaStreams"] = mediaStreams;
            ViewData["streamNumber"] = streamNumber;

            ViewData["streamTunerPageSize"] = streamTunerPageSize;
            ViewData["streamSkipCount"] = streamSkipCount;
            ViewData["streamLastPage"] = streamLastPage ? 1 : 0;

            ViewData["accountMessage"] = accountMessage;

            return View();
        }
    }
}