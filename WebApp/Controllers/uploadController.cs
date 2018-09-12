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
            User userProfile = new User(authToken);
            StorageBlobClient blobClient = new StorageBlobClient(userProfile.MediaAccount, storageAccount);
            Stream blockStream = Request.Form.Files[0].OpenReadStream();
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            blobClient.UploadBlock(blockStream, containerName, name, chunk, chunks, contentType);
            return Json(chunk);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            Dictionary<string, string> storageAccounts = Account.GetStorageAccounts(authToken);
            ViewData["storageAccount"] = HomeController.GetListItems(storageAccounts);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["indexerEnabled"] = mediaClient.IndexerEnabled() ? 1 : 0;
            }
            return View();
        }
    }
}