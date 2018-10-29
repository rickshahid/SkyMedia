using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class TransformController : Controller
    {
        internal static Transform[] GetTransforms(MediaClient mediaClient)
        {
            Transform[] transforms = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            if (transforms.Length == 0)
            {
                transforms = mediaClient.CreateTransforms();
            }
            return transforms;
        }

        public JsonResult Create(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs)
        {
            Transform transform;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                transform = mediaClient.CreateTransform(transformName, transformDescription, transformOutputs);
            }
            return Json(transform);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = GetTransforms(mediaClient);
            }
            return View();
        }
    }
}