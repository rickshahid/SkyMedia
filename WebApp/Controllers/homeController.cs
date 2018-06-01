using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class homeController : Controller
    {
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

        internal static SelectListItem[] GetMediaProcessors(string authToken, bool presetsView)
        {
            List<SelectListItem> mediaProcessors = new List<SelectListItem>();
            SelectListItem mediaProcessor = new SelectListItem()
            {
                Text = string.Empty,
                Value = string.Empty
            };
            mediaProcessors.Add(mediaProcessor);
            Dictionary<string, string> processors = Processor.GetMediaProcessors(authToken, presetsView, false);
            foreach (KeyValuePair<string, string> processor in processors)
            {
                mediaProcessor = new SelectListItem()
                {
                    Text = processor.Value.ToString(),
                    Value = processor.Key.ToString()
                };
                mediaProcessors.Add(mediaProcessor);
            }
            return mediaProcessors.ToArray();
        }

        internal static SelectListItem[] GetSpokenLanguages(bool videoIndexer, bool includeEmpty)
        {
            List<SelectListItem> spokenLanguages = new List<SelectListItem>();
            if (includeEmpty)
            {
                SelectListItem spokenLanguage = new SelectListItem()
                {
                    Text = string.Empty,
                    Value = string.Empty
                };
                spokenLanguages.Add(spokenLanguage);
            }
            Dictionary<string, string> languages = Language.GetLanguages(videoIndexer);
            foreach (KeyValuePair<string, string> language in languages)
            {
                SelectListItem spokenLanguage = new SelectListItem()
                {
                    Text = language.Value,
                    Value = language.Key
                };
                spokenLanguages.Add(spokenLanguage);
            }
            return spokenLanguages.ToArray();
        }

        internal static void SetViewData(string authToken, ViewDataDictionary viewData)
        {
        //    User authUser = new User(authToken);
        //    viewData["jobName"] = GetJobTemplates(authToken);
        //    viewData["mediaProcessor1"] = GetMediaProcessors(authToken, false);
        //    viewData["encoderConfig1"] = new List<SelectListItem>();
        //    viewData["encoderStandardPresets"] = presetController.GetProcessorPresets(MediaProcessor.EncoderStandard, authUser.MediaAccount.Name, true);
        //    viewData["encoderPremiumPresets"] = presetController.GetProcessorPresets(MediaProcessor.EncoderPremium, authUser.MediaAccount.Name, false);
        //    viewData["audioAnalyzerLanguages"] = GetSpokenLanguages(false, false);
        //    viewData["videoAnalyzerLanguages"] = GetSpokenLanguages(true, false);
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

        public IActionResult player()
        {
            return View();
        }

        public IActionResult index()
        {
            string accountMessage = string.Empty;
            MediaStream[] mediaStreams = new MediaStream[] { };

            string authToken = GetAuthToken(this.Request, this.Response);
            string queryString = this.Request.QueryString.Value.ToLower();

            if (this.Request.HasFormContentType)
            {
                RedirectToActionResult redirectAction = Startup.OnSignIn(this, authToken);
                if (redirectAction != null)
                {
                    return redirectAction;
                }
            }

            int streamNumber = 1;
            string autoPlay = "false";
            if (queryString.Contains("stream"))
            {
                streamNumber = int.Parse(this.Request.Query["stream"]);
                autoPlay = "true";
            }

            int streamOffset = 0;
            int streamIndex = streamNumber - 1;
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    mediaStreams = Media.GetMediaStreams();
                }
                else
                {
                    MediaClient mediaClient = new MediaClient(authToken);
                    if (!Media.IsStreamingEnabled(mediaClient))
                    {
                        accountMessage = Constant.Message.StreamingEndpointNotStarted;
                    }
                    else
                    {
                        mediaStreams = Media.GetMediaStreams(authToken, mediaClient, streamNumber, out streamOffset, out streamIndex, out bool endOfStreams);
                        if (endOfStreams)
                        {
                            streamNumber = streamNumber - 1;
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

            ViewData["languageCode"] = this.Request.Query["language"];
            ViewData["autoPlay"] = autoPlay;

            return View();
        }
    }
}