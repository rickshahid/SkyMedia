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

        private static SelectListItem[] GetMediaProcessors(string authToken)
        {
            List<SelectListItem> mediaProcessors = new List<SelectListItem>();
            SelectListItem mediaProcessor = new SelectListItem()
            {
                Text = string.Empty,
                Value = string.Empty
            };
            mediaProcessors.Add(mediaProcessor);
            NameValueCollection processors = Processor.GetMediaProcessors(authToken, false) as NameValueCollection;
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

        internal static SelectListItem[] GetSpokenLanguages(bool videoIndexer)
        {
            List<SelectListItem> spokenLanguages = new List<SelectListItem>();
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
            viewData["signiantAccountKey"] = authUser.SigniantAccountKey;
            viewData["asperaAccountKey"] = authUser.AsperaAccountKey;
            viewData["storageAccount"] = GetStorageAccounts(authToken);
            viewData["jobName"] = GetJobTemplates(authToken);
            viewData["mediaProcessor1"] = GetMediaProcessors(authToken);
            viewData["encoderConfig1"] = new List<SelectListItem>();
            viewData["indexerLanguages"] = GetSpokenLanguages(true);
            viewData["speechToTextLanguages"] = GetSpokenLanguages(false);
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

        public IActionResult index()
        {
            string accountMessage = string.Empty;
            string queryString = this.Request.QueryString.Value.ToLower();
            MediaStream[] mediaStreams = new MediaStream[] { };
            try
            {
                string authToken = GetAuthToken(this.Request, this.Response);

                if (this.Request.HasFormContentType)
                {
                    RedirectToActionResult redirectAction = Startup.OnSignIn(authToken, this);
                    if (redirectAction != null)
                    {
                        return redirectAction;
                    }
                }

                if (this.Request.Host.Value.Contains("account."))
                {
                    return RedirectToAction("signin", "account");
                }

                MediaClient mediaClient = null;
                IndexerClient indexerClient = null;
                if (!string.IsNullOrEmpty(authToken))
                {
                    try
                    {
                        mediaClient = new MediaClient(authToken);
                        indexerClient = new IndexerClient(authToken, null, null);
                    }
                    catch (Exception ex)
                    {
                        accountMessage = ex.ToString();
                    }
                }

                if (mediaClient == null)
                {
                    mediaStreams = Media.GetMediaStreams();
                }
                else if (!Media.IsStreamingEnabled(mediaClient))
                {
                    accountMessage = Constant.Message.StreamingEndpointNotRunning;
                }
                else if (this.Request.Host.Value.Contains("live.") || queryString.Contains("mode=live"))
                {
                    mediaStreams = Media.GetLiveStreams(mediaClient, indexerClient);
                }
                else
                {
                    mediaStreams = Media.GetMediaStreams(mediaClient, indexerClient);
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