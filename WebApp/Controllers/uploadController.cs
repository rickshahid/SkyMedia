using System.IO;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class uploadController : Controller
    {
        public JsonResult block(string name, int chunk, int chunks, string storageAccount, string contentType)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            string containerName = Constant.Storage.Blob.Container.FileUpload;
            Stream blockStream = this.Request.Form.Files[0].OpenReadStream();
            Storage.UploadBlock(authToken, storageAccount, containerName, blockStream, name, chunk, chunks, contentType);
            return Json(chunk);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            Dictionary<string, string> storageAccounts = Storage.GetAccounts(authToken);
            ViewData["storageAccount"] = homeController.GetListItems(storageAccounts);
            return View();
        }
    }
}