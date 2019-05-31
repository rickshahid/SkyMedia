using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private void SetStyleHost()
        {
            UriBuilder uriBuilder = new UriBuilder()
            {
                Scheme = Request.Scheme,
                Host = Request.Host.Value
            };
            ViewData["cssHost"] = uriBuilder.ToString();
        }

        public void SignUpIn()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void ProfileEdit()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void PasswordReset()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public IActionResult SignOut()
        {
            SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        public IActionResult SignIn()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult Password()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult Profile()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult StorageAccounts()
        {
            List<MediaStorage> storageAccounts = new List<MediaStorage>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                foreach (StorageAccount storageAccount in mediaClient.StorageAccounts)
                {
                    MediaStorage mediaStorage = new MediaStorage(mediaClient.MediaAccount, storageAccount);
                    storageAccounts.Add(mediaStorage);
                }
                ViewData["storageAccounts"] = storageAccounts.ToArray();
            }
            return View();
        }

        public IActionResult ContentKeyPolicies()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["contentKeyPolicies"] = mediaClient.GetAllEntities<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy);
            }
            return View();
        }

        public IActionResult StreamingPolicies()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingPolicies"] = mediaClient.GetAllEntities<StreamingPolicy>(MediaEntity.StreamingPolicy);
            }
            return View();
        }

        public IActionResult StreamingEndpoints()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingEndpoints"] = mediaClient.GetAllEntities<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            }
            return View();
        }

        public IActionResult StreamingLocators()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingLocators"] = mediaClient.GetAllEntities<StreamingLocator>(MediaEntity.StreamingLocator);
            }
            return View();
        }

        public IActionResult StreamingFiltersAccount()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingFiltersAccount"] = mediaClient.GetAllEntities<AccountFilter>(MediaEntity.StreamingFilterAccount);
            }
            return View();
        }

        public IActionResult StreamingFiltersAsset()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingFiltersAsset"] = mediaClient.GetAllEntities<AssetFilter, Asset>(MediaEntity.StreamingFilterAsset, MediaEntity.Asset);
            }
            return View();
        }

        public IActionResult LiveEvents()
        {
            List<MediaLiveEvent> mediaLiveEvents = new List<MediaLiveEvent>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                LiveEvent[] liveEvents = mediaClient.GetAllEntities<LiveEvent>(MediaEntity.LiveEvent);
                foreach (LiveEvent liveEvent in liveEvents)
                {
                    MediaLiveEvent mediaLiveEvent = new MediaLiveEvent(mediaClient, liveEvent);
                    mediaLiveEvents.Add(mediaLiveEvent);
                }
                ViewData["streamingPolicies"] = JobController.GetStreamingPolicies(mediaClient);
            }
            ViewData["liveEvents"] = mediaLiveEvents.ToArray();
            ViewData["inputStreamId"] = AuthToken.GetClaimValue(authToken, Constant.UserAttribute.MediaAccountSubscriptionId);
            return View();
        }

        public IActionResult LiveEventOutputs()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["liveEvents"] = LiveController.GetLiveEvents(mediaClient);
                ViewData["liveEventOutputs"] = mediaClient.GetAllEntities<LiveOutput, LiveEvent>(MediaEntity.LiveEventOutput, MediaEntity.LiveEvent);
                ViewData["streamingPolicies"] = JobController.GetStreamingPolicies(mediaClient);
            }
            return View();
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["entityCounts"] = Account.GetEntityCounts(mediaClient);
            }
            return View();
        }

        public JsonResult DeleteEntities(bool skipLive)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    Account.DeleteEntities(mediaClient, skipLive);
                }
                return Json(true);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult DeleteEntity(string gridId, string entityName, string parentName)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    switch (gridId)
                    {
                        case "assets":
                            mediaClient.DeleteEntity(MediaEntity.Asset, entityName);
                            break;
                        case "transforms":
                            mediaClient.DeleteEntity(MediaEntity.Transform, entityName);
                            break;
                        case "transformJobs":
                            mediaClient.DeleteEntity(MediaEntity.TransformJob, entityName, parentName);
                            break;
                        case "contentKeyPolicies":
                            mediaClient.DeleteEntity(MediaEntity.ContentKeyPolicy, entityName);
                            break;
                        case "streamingPolicies":
                            mediaClient.DeleteEntity(MediaEntity.StreamingPolicy, entityName);
                            break;
                        case "streamingEndpoints":
                            mediaClient.DeleteEntity(MediaEntity.StreamingEndpoint, entityName);
                            break;
                        case "streamingLocators":
                            mediaClient.DeleteEntity(MediaEntity.StreamingLocator, entityName);
                            break;
                        case "filtersAccount":
                            mediaClient.DeleteEntity(MediaEntity.StreamingFilterAccount, entityName);
                            break;
                        case "filtersAsset":
                            mediaClient.DeleteEntity(MediaEntity.StreamingFilterAsset, entityName, parentName);
                            break;
                        case "liveEvents":
                            mediaClient.DeleteEntity(MediaEntity.LiveEvent, entityName);
                            break;
                        case "liveEventOutputs":
                            mediaClient.DeleteEntity(MediaEntity.LiveEventOutput, entityName, parentName);
                            break;
                        case "indexerInsights":
                            mediaClient.IndexerDeleteVideo(entityName);
                            break;
                        case "indexerProjects":
                            mediaClient.IndexerDeleteProject(entityName);
                            break;
                        case "indexerModelsPeople":
                            mediaClient.IndexerDeleteModel(MediaInsightModel.People, entityName);
                            break;
                        case "indexerModelsLanguage":
                            mediaClient.IndexerDeleteModel(MediaInsightModel.Language, entityName);
                            break;
                        case "indexerModelsBrand":
                            mediaClient.IndexerDeleteModel(MediaInsightModel.Brand, entityName);
                            break;
                    }
                }
                return Json(entityName);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }
    }
}