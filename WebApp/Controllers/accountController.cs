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
            AuthenticationProperties authProperties = new AuthenticationProperties();
            HttpContext.ChallengeAsync(authProperties).Wait();
        }

        public void profileedit()
        {
            AuthenticationProperties authProperties = new AuthenticationProperties();
            HttpContext.ChallengeAsync(authProperties).Wait();
        }

        public void passwordreset()
        {
            AuthenticationProperties authProperties = new AuthenticationProperties();
            HttpContext.ChallengeAsync(authProperties).Wait();
        }

        public JsonResult clear(bool allEntities, bool liveOnly)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            Account.DeleteEntities(authToken, allEntities, liveOnly);
            return Json(allEntities);
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
            return View();
        }

        public IActionResult profile()
        {
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