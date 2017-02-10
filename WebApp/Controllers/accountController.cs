using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class accountController : Controller
    {
        public void signin(string subdomain)
        {
            string authScheme = OpenIdConnectDefaults.AuthenticationScheme;
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.Authentication.ChallengeAsync(authScheme, authProperties).Wait();
        }

        public void profileedit(string subdomain)
        {
            string authScheme = OpenIdConnectDefaults.AuthenticationScheme;
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.Authentication.ChallengeAsync(authScheme, authProperties).Wait();
        }

        public void passwordreset()
        {
            string authScheme = OpenIdConnectDefaults.AuthenticationScheme;
            AuthenticationProperties authProperties = new AuthenticationProperties();
            HttpContext.Authentication.ChallengeAsync(authScheme, authProperties).Wait();
        }

        public IActionResult signout()
        {
            HttpContext.Authentication.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme).Wait();
            HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("index", "home");
        }

        public IActionResult processors()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["mediaProcessors"] = Entities.GetMediaProcessors(authToken);
            return View();
        }

        public IActionResult endpoints()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["notificationEndpoints"] = Entities.GetNotificationEndpoints(authToken);
            return View();
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            ViewData["entityCounts"] = Entities.GetEntityCounts(mediaClient);
            ViewData["id"] = this.Request.Query["id"];
            ViewData["name"] = this.Request.Query["name"];
            return View();
        }
    }
}
