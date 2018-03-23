using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class accountController : Controller
    {
        public void signupin()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void profileedit()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void passwordreset()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void clear(bool allEntities, bool liveOnly)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            Account.DeleteEntities(authToken, allEntities, liveOnly);
        }

        public IActionResult contentKeys()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["contentKeys"] = Account.GetContentKeys(authToken);
            return View();
        }

        public IActionResult contentKeyAuthZPolicies()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["contentKeyAuthZPolicies"] = Account.GetContentKeyAuthZPolicies(authToken);
            return View();
        }

        public IActionResult contentKeyAuthZPolicyOptions()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["contentKeyAuthZPolicyOptions"] = Account.GetContentKeyAuthZPolicyOptions(authToken);
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
            ViewData["notificationEndpoints"] = Account.GetNotificationEndpoints(authToken);
            return View();
        }

        public IActionResult signout()
        {
            this.SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        public IActionResult signup()
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