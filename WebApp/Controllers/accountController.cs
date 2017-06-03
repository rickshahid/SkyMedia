using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class accountController : Controller
    {
        private string _authScheme = OpenIdConnectDefaults.AuthenticationScheme;

        public void signin(string subdomain)
        {
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.Authentication.ChallengeAsync(_authScheme, authProperties).Wait();
        }

        public void profileedit(string subdomain)
        {
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.Authentication.ChallengeAsync(_authScheme, authProperties).Wait();
        }

        public void passwordreset()
        {
            AuthenticationProperties authProperties = new AuthenticationProperties();
            HttpContext.Authentication.ChallengeAsync(_authScheme, authProperties).Wait();
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
            ViewData["mediaProcessors"] = Account.GetMediaProcessors(authToken, true);
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

        public JsonResult clear(bool allEntities)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            string attributeName = Constant.UserAttribute.VideoIndexerKey;
            string indexerKey = AuthToken.GetClaimValue(authToken, attributeName);

            MediaClient mediaClient = new MediaClient(authToken);
            Account.ClearEntities(indexerKey, mediaClient, allEntities);
            string[][] entityCounts = Account.GetEntityCounts(mediaClient);

            return Json(entityCounts);
        }
    }
}
