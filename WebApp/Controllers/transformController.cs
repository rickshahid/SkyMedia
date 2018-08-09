using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class TransformController : Controller
    {
        internal static Transform Create(MediaClient mediaClient, bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
            Transform transform;
            if (mediaClient.IndexerIsEnabled())
            {
                transform = mediaClient.CreateTransform(standardEncoderPreset, false, false);
            }
            else
            {
                transform = mediaClient.CreateTransform(standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
            }
            return transform;
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
                ViewData["transforms"] = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            }
            return View();
        }
    }
}