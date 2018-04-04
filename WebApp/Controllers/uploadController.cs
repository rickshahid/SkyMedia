using System.IO;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class uploadController : Controller
    {
        public JsonResult file(string name, int chunk, int chunks, string storageAccount)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            string containerName = Constant.Storage.Blob.Container.FileUpload;
            Stream fileStream = this.Request.Form.Files[0].OpenReadStream();
            Storage.UploadFile(authToken, storageAccount, containerName, fileStream, name, chunk, chunks);
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