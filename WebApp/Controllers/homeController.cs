﻿using System;
using System.Collections.Generic;

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
            viewData["accountName"] = currentUser.MediaAccount.Name;
            if (string.IsNullOrEmpty(currentUser.MediaAccount.VideoIndexerRegion) || string.IsNullOrEmpty(currentUser.MediaAccount.VideoIndexerKey))
            {
                viewData["indexerMessage"] = "DisplayMessage('Azure Video Indexer Account', 'Your Azure Media Services account does not have an Azure Video Indexer account.<br><br>Verify your " + viewData["appName"] + " user account profile is configured.')";
            }
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

            int streamSkipCount = 0;
            int streamTunerPageSize = 0;
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
                    //using (SearchClient searchClient = new SearchClient(authToken, false))
                    //{
                    //    string searchAccountName = searchClient.AccountName;
                    //    if (!string.IsNullOrEmpty(searchAccountName))
                    //    {
                    //        accountMessage = searchAccountName;
                    //    }
                    //}

                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        //string assetName = "AI Show - Art Exploration (ABR)";
                        
                        //string filterName = "Bitrate";
                        //int firstBitrate = 360000;

                        //string filterName = "Subclip";
                        //long startSeconds = 22;
                        //long endSeconds = 55;

                        //mediaClient.DeleteEntity(MediaEntity.StreamingFilterAccount, filterName);

                        //mediaClient.CreateFilter(assetName, filterName, firstBitrate);
                        //mediaClient.CreateFilter(assetName, filterName, startSeconds, endSeconds);

                        mediaStreams = Media.GetAccountStreams(authToken, mediaClient, streamNumber, out streamSkipCount, out streamTunerPageSize, out streamTunerLastPage);
                    }
                }
            }
            catch (ApiErrorException apiEx)
            {
                accountMessage = string.Concat(apiEx.Message, " (", apiEx.Response.Content, ")");
            }
            catch (Exception ex)
            {
                accountMessage = ex.Message;
            }

            ViewData["mediaStreams"] = mediaStreams;
            ViewData["streamNumber"] = streamNumber;

            ViewData["streamSkipCount"] = streamSkipCount;
            ViewData["streamTunerPageSize"] = streamTunerPageSize;
            ViewData["streamTunerLastPage"] = streamTunerLastPage;

            ViewData["accountMessage"] = accountMessage;

            return View();
        }
    }
}