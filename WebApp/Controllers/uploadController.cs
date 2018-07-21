using System.IO;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class UploadController : Controller
    {
        public JsonResult Block(string name, int chunk, int chunks, string storageAccount, string contentType)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            string containerName = Constant.Storage.Blob.Container.FileUpload;
            Stream blockStream = Request.Form.Files[0].OpenReadStream();
            Storage.UploadBlock(authToken, storageAccount, containerName, blockStream, name, chunk, chunks, contentType);
            return Json(chunk);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            User authUser = new User(authToken);
            Dictionary<string, string> storageAccounts = Storage.GetAccounts(authToken);
            ViewData["storageAccount"] = HomeController.GetListItems(storageAccounts);
            ViewData["videoIndexerKey"] = authUser.MediaAccount.VideoIndexerKey;
            return View();
        }
    }
}