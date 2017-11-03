using System.IO;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class uploadController : Controller
    {
        public JsonResult file(string name, int chunk, int chunks, string storageAccount, string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                containerName = Constant.Storage.Blob.Container.FileUpload;
            }
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            Stream readStream = this.Request.Form.Files[0].OpenReadStream();
            Storage.UploadFile(authToken, storageAccount, containerName, readStream, name, chunk, chunks);
            return Json(chunk);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}