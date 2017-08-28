using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class accountController : Controller
    {
        public void signin(string subdomain)
        {
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.ChallengeAsync(authProperties).Wait();
        }

        public void profileedit(string subdomain)
        {
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.ChallengeAsync(authProperties).Wait();
        }

        public void passwordreset()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public JsonResult clear(bool allEntities)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            Account.DeleteEntities(authToken, allEntities);
            return Json(true);
        }

        public IActionResult signout()
        {
            this.SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        public IActionResult processors()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["mediaProcessors"] = Processor.GetMediaProcessors(authToken, false);
            return View();
        }

        public IActionResult endpoints()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["notificationEndpoints"] = Account.GetNotificationEndpoints(authToken);
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