using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class homeController : Controller
    {
        private static SelectListItem[] GetStorageAccounts(string authToken)
        {
            List<SelectListItem> storageAccounts = new List<SelectListItem>();
            NameValueCollection accounts = Storage.GetAccounts(authToken);
            foreach (string accountName in accounts.Keys)
            {
                SelectListItem storageAccount = new SelectListItem()
                {
                    Text = accountName,
                    Value = accounts[accountName]
                };
                storageAccounts.Add(storageAccount);
            }
            return storageAccounts.ToArray();
        }

        private static SelectListItem[] GetJobTemplates(string authToken)
        {
            List<SelectListItem> jobTemplates = new List<SelectListItem>();
            NameValueCollection templates = MediaClient.GetJobTemplates(authToken);
            foreach (string templateName in templates.Keys)
            {
                SelectListItem jobTemplate = new SelectListItem()
                {
                    Text = templateName,
                    Value = templates[templateName]
                };
                jobTemplates.Add(jobTemplate);
            }
            return jobTemplates.ToArray();
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
            NameValueCollection processors = Processor.GetMediaProcessors(authToken, presetsView, false) as NameValueCollection;
            foreach (string processor in processors)
            {
                mediaProcessor = new SelectListItem()
                {
                    Text = processor,
                    Value = processors[processor]
                };
                mediaProcessors.Add(mediaProcessor);
            }
            return mediaProcessors.ToArray();
        }

        internal static SelectListItem[] GetSpokenLanguages(bool videoIndexer, bool defaultEmpty)
        {
            List<SelectListItem> spokenLanguages = new List<SelectListItem>();
            if (defaultEmpty)
            {
                SelectListItem spokenLanguage = new SelectListItem()
                {
                    Text = string.Empty,
                    Value = string.Empty
                };
                spokenLanguages.Add(spokenLanguage);
            }
            NameValueCollection languages = Language.GetSpokenLanguages(videoIndexer);
            foreach (string languageName in languages.Keys)
            {
                SelectListItem spokenLanguage = new SelectListItem()
                {
                    Text = languageName,
                    Value = languages[languageName]
                };
                spokenLanguages.Add(spokenLanguage);
            }
            return spokenLanguages.ToArray();
        }

        internal static void SetViewData(string authToken, ViewDataDictionary viewData)
        {
            User authUser = new User(authToken);
            viewData["storageAccount"] = GetStorageAccounts(authToken);
            viewData["jobName"] = GetJobTemplates(authToken);
            viewData["mediaProcessor1"] = GetMediaProcessors(authToken, false);
            viewData["encoderConfig1"] = new List<SelectListItem>();
            viewData["encoderStandardPresets"] = presetController.GetProcessorPresets(MediaProcessor.EncoderStandard, authUser.MediaAccount.Id, true);
            viewData["encoderPremiumPresets"] = presetController.GetProcessorPresets(MediaProcessor.EncoderPremium, authUser.MediaAccount.Id, false);
            viewData["indexerLanguages"] = GetSpokenLanguages(true, false);
            viewData["speechAnalyzerLanguages"] = GetSpokenLanguages(false, false);
        }

        public static string GetDirectoryId(HttpRequest request)
        {
            string settingKey = Constant.AppSettingKey.DirectoryDefaultId;
            string directoryId = AppSetting.GetValue(settingKey);
            if (request != null)
            {
                string activeDirectory = request.Query[Constant.HttpQueryString.ActiveDirectory];
                if (!string.IsNullOrEmpty(activeDirectory))
                {
                    directoryId = activeDirectory;
                }
                else if (request.Host.Value.StartsWith("B2", StringComparison.OrdinalIgnoreCase))
                {
                    directoryId = request.Host.Value.Substring(0, 3).ToUpper();
                }
            }
            return directoryId;
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

        public static string GetAppSetting(string settingKey)
        {
            return AppSetting.GetValue(settingKey);
        }

        public JsonResult endpoint()
        {
            string authToken = GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            string endpointName = Account.StartStreamingEndpoint(mediaClient);
            return Json(endpointName);
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

            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    mediaStreams = Media.GetMediaStreams();
                }
                else
                {
                    MediaClient mediaClient = new MediaClient(authToken);
                    if (!Account.IsStreamingEnabled(mediaClient, out bool endpointStarting))
                    {
                        accountMessage = endpointStarting ? Constant.Message.StreamingEndpointStarting : Constant.Message.StreamingEndpointNotRunning;
                    }
                    else
                    {
                        mediaStreams = Media.GetMediaStreams(authToken, mediaClient);
                    }
                }
            }
            catch (Exception ex)
            {
                accountMessage = ex.ToString();
            }
            ViewData["accountMessage"] = accountMessage;

            ViewData["mediaStreams"] = mediaStreams;
            ViewData["streamNumber"] = 1;
            ViewData["autoPlay"] = "false";
            if (queryString.Contains("stream"))
            {
                ViewData["streamNumber"] = this.Request.Query["stream"];
                ViewData["autoPlay"] = "true";
            }

            ViewData["languageCode"] = this.Request.Query["language"];
            return View();
        }
    }
}