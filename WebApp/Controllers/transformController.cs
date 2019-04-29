using System.Net;

using Microsoft.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class TransformController : Controller
    {
        public JsonResult Save(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs, Image thumbnailCodec)
        {
            try
            {
                Transform transform;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    transform = mediaClient.GetTransform(transformName, transformDescription, transformOutputs, thumbnailCodec, true);
                }
                return Json(transform);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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