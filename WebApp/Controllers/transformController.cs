using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class TransformController : Controller
    {
        public JsonResult Create(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs)
        {
            try
            {
                Transform transform;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    transform = mediaClient.CreateTransform(transformName, transformDescription, transformOutputs);
                }
                return Json(transform);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            }
            return View();
        }
    }
}