using System.IO;
//using System.Net;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class uploadController : Controller
    {
        //public JsonResult storage(TransferService transferService, string storageAccount, string[] filePaths)
        //{
        //    object storage = null;
        //    string authToken = homeController.GetAuthToken(this.Request, this.Response);
        //    string containerName = Constant.Storage.Blob.Container.FileUpload;
        //    Storage.CreateContainer(authToken, storageAccount, containerName);
        //    string storageAccountKey = Storage.GetAccountKey(authToken, storageAccount);
        //    switch (transferService)
        //    {
        //        case TransferService.SigniantFlight:
        //            string storageContainer = string.Format(Constant.Storage.Partner.SigniantContainer, storageAccount, storageAccountKey, containerName);
        //            storage = string.Concat("{", storageContainer, "}");
        //            break;
        //        case TransferService.AsperaFasp:
        //            AsperaClient asperaClient = new AsperaClient(authToken);
        //            storageContainer = string.Format(Constant.Storage.Partner.AsperaContainer, storageAccount, WebUtility.UrlEncode(storageAccountKey));
        //            storage = asperaClient.GetTransferSpecs(storageContainer, containerName, filePaths, false);
        //            break;
        //    } 
        //    return Json(storage);
        //}

        public JsonResult file(string name, int chunk, int chunks, string storageAccount)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            string containerName = Constant.Storage.Blob.Container.FileUpload;
            Stream inputStream = this.Request.Form.Files[0].OpenReadStream();
            Storage.UploadFile(authToken, storageAccount, containerName, inputStream, name, chunk, chunks);
            return Json(chunk);
        }

        public IActionResult signiant()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            User authUser = new User(authToken);
            ViewData["signiantServiceGateway"] = authUser.SigniantServiceGateway;

            string settingKey = Constant.AppSettingKey.SigniantTransferApi;
            ViewData["signiantTransferApi"] = AppSetting.GetValue(settingKey);

            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }

        public IActionResult aspera()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            User authUser = new User(authToken);
            ViewData["asperaServiceGateway"] = authUser.AsperaServiceGateway;
            ViewData["asperaAccountId"] = authUser.AsperaAccountId;

            string settingKey = Constant.AppSettingKey.AsperaTransferApi;
            ViewData["asperaTransferApi"] = AppSetting.GetValue(settingKey);

            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}