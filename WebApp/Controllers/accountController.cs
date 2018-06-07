using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class accountController : Controller
    {
        private void SetStyleHost()
        {
            ViewData["cssHost"] = string.Concat(this.Request.Scheme, "://", this.Request.Host.Value);
        }

        public void signUpIn()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void profileEdit()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void passwordReset()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void deleteEntities(bool liveOnly)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Account.DeleteEntities(mediaClient, liveOnly);
            }
        }

        public void deleteEntity(string gridId, string entityName, string parentEntityName)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                switch (gridId)
                {
                    case "assets":
                        mediaClient.DeleteEntity(MediaEntity.Asset, entityName);
                        break;
                    case "tranforms":
                        mediaClient.DeleteEntity(MediaEntity.Transform, entityName);
                        break;
                    case "transformJobs":
                        mediaClient.DeleteEntity(MediaEntity.TransformJob, entityName, parentEntityName);
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
                    case "liveEvents":
                        mediaClient.DeleteEntity(MediaEntity.LiveEvent, entityName);
                        break;
                    case "liveEventOutputs":
                        mediaClient.DeleteEntity(MediaEntity.LiveEventOutput, entityName, parentEntityName);
                        break;
                }
            }
        }

        public IActionResult signOut()
        {
            this.SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        public IActionResult signIn()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult password()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult profile()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult assets()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["assets"] = mediaClient.GetAllEntities<Asset>(MediaEntity.Asset);
            }
            return View();
        }

        public IActionResult transforms()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            }
            return View();
        }

        public IActionResult transformJobs()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            }
            return View();
        }

        public IActionResult contentKeyPolicies()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["contentKeyPolicies"] = mediaClient.GetAllEntities<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy);
            }
            return View();
        }

        public IActionResult streamingPolicies()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingPolicies"] = mediaClient.GetAllEntities<StreamingPolicy>(MediaEntity.StreamingPolicy);
            }
            return View();
        }

        public IActionResult streamingEndpoints()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingEndpoints"] = mediaClient.GetAllEntities<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            }
            return View();
        }

        public IActionResult streamingLocators()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingLocators"] = mediaClient.GetAllEntities<StreamingLocator>(MediaEntity.StreamingLocator);
            }
            return View();
        }

        public IActionResult liveEvents()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["liveEvents"] = mediaClient.GetAllEntities<LiveEvent>(MediaEntity.LiveEvent);
            }
            return View();
        }

        public IActionResult liveEventOutputs()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["liveEventOutputs"] = mediaClient.GetAllEntities<LiveOutput, LiveEvent>(MediaEntity.LiveEventOutput, MediaEntity.LiveEvent);
            }
            return View();
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["entityCounts"] = Account.GetEntityCounts(mediaClient);
            }
            return View();
        }
    }
}