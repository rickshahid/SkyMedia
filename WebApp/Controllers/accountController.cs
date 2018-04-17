using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class accountController : Controller
    {
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

        public void deleteEntities(bool allEntities, bool liveChannels)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            Account.DeleteEntities(authToken, allEntities, liveChannels);
        }

        public JsonResult deleteEntity(string entityGrid, string entityId)
        {
            MediaEntity entityType = MediaEntity.MonitoringConfiguration;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            switch (entityGrid)
            {
                case "contentKeys":
                    entityType = MediaEntity.ContentKey;
                    break;
                case "contentKeyAuthPolicies":
                    entityType = MediaEntity.ContentKeyAuthPolicy;
                    break;
                case "contentKeyAuthPolicyOptions":
                    entityType = MediaEntity.ContentKeyAuthPolicyOption;
                    break;
                case "notificationEndpoints":
                    entityType = MediaEntity.NotificationEndpoint;
                    break;
                case "assets":
                    entityType = MediaEntity.Asset;
                    break;
                case "jobs":
                    entityType = MediaEntity.Job;
                    break;
                case "deliveryPolicies":
                    entityType = MediaEntity.DeliveryPolicy;
                    break;
                case "accessPolicies":
                    entityType = MediaEntity.AccessPolicy;
                    break;
                case "streamingLocators":
                    entityType = MediaEntity.StreamingLocator;
                    break;
                case "streamingFilters":
                    entityType = MediaEntity.StreamingFilter;
                    break;
            }
            Account.DeleteEntity(authToken, entityType, entityId);
            return Json(true);
        }

        public IActionResult contentKeys()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["contentKeys"] = Account.GetEntities(authToken, MediaEntity.ContentKey);
            return View();
        }

        public IActionResult contentKeyAuthPolicies()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["contentKeyAuthPolicies"] = Account.GetEntities(authToken, MediaEntity.ContentKeyAuthPolicy);
            return View();
        }

        public IActionResult contentKeyAuthPolicyOptions()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["contentKeyAuthPolicyOptions"] = Account.GetEntities(authToken, MediaEntity.ContentKeyAuthPolicyOption);
            return View();
        }

        public IActionResult mediaProcessors()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["mediaProcessors"] = Processor.GetMediaProcessors(authToken, false, true);
            return View();
        }

        public IActionResult notificationEndpoints()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["notificationEndpoints"] = Account.GetEntities(authToken, MediaEntity.NotificationEndpoint);
            return View();
        }

        public IActionResult assets()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["assets"] = Account.GetEntities(authToken, MediaEntity.Asset);
            return View();
        }

        public IActionResult jobs()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["jobs"] = Account.GetEntities(authToken, MediaEntity.Job);
            return View();
        }

        public IActionResult deliveryPolicies()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["deliveryPolicies"] = Account.GetEntities(authToken, MediaEntity.DeliveryPolicy);
            return View();
        }

        public IActionResult accessPolicies()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["accessPolicies"] = Account.GetEntities(authToken, MediaEntity.AccessPolicy);
            return View();
        }

        public IActionResult streamingLocators()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["streamingLocators"] = Account.GetEntities(authToken, MediaEntity.StreamingLocator);
            return View();
        }

        public IActionResult streamingFilters()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["streamingFilters"] = Account.GetEntities(authToken, MediaEntity.StreamingFilter);
            return View();
        }

        public IActionResult signOut()
        {
            this.SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        public IActionResult signIn()
        {
            ViewData["cssHost"] = string.Concat(this.Request.Scheme, "://", this.Request.Host.Value);
            return View();
        }

        public IActionResult profile()
        {
            ViewData["cssHost"] = string.Concat(this.Request.Scheme, "://", this.Request.Host.Value);
            return View();
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            ViewData["entityCounts"] = Account.GetEntityCounts(mediaClient);
            return View();
        }
    }
}