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
            ViewData["cssHost"] = string.Concat(this.Request.Scheme, "://", this.Request.Host.Value);
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

        public void DeleteEntities(bool liveOnly)
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Account.DeleteEntities(mediaClient, liveOnly);
            }
        }

        public void DeleteEntity(string gridId, string entityName, string parentEntityName)
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
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

        public IActionResult SignOut()
        {
            this.SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
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

        public IActionResult Assets()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["assets"] = mediaClient.GetAllEntities<Asset>(MediaEntity.Asset);
            }
            return View();
        }

        public IActionResult Transforms()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            }
            return View();
        }

        public IActionResult TransformJobs()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            }
            return View();
        }

        public IActionResult ContentKeyPolicies()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["contentKeyPolicies"] = mediaClient.GetAllEntities<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy);
            }
            return View();
        }

        public IActionResult StreamingPolicies()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingPolicies"] = mediaClient.GetAllEntities<StreamingPolicy>(MediaEntity.StreamingPolicy);
            }
            return View();
        }

        public IActionResult StreamingEndpoints()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingEndpoints"] = mediaClient.GetAllEntities<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            }
            return View();
        }

        public IActionResult StreamingLocators()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingLocators"] = mediaClient.GetAllEntities<StreamingLocator>(MediaEntity.StreamingLocator);
            }
            return View();
        }

        public IActionResult LiveEvents()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["liveEvents"] = mediaClient.GetAllEntities<LiveEvent>(MediaEntity.LiveEvent);
            }
            return View();
        }

        public IActionResult LiveEventOutputs()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["liveEventOutputs"] = mediaClient.GetAllEntities<LiveOutput, LiveEvent>(MediaEntity.LiveEventOutput, MediaEntity.LiveEvent);
            }
            return View();
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["entityCounts"] = Account.GetEntityCounts(mediaClient);
            }
            return View();
        }
    }
}