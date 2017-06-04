using System.IO;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class uploadController : Controller
    {
        public JsonResult storage(TransferService transferService, string storageAccount, string[] filePaths)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            string accountKey = Storage.GetUserAccountKey(authToken, storageAccount);
            string containerName = Constant.Storage.Blob.Container.Upload;
            Storage.CreateContainer(authToken, storageAccount, containerName);
            object result = null;
            switch (transferService)
            {
                case TransferService.SigniantFlight:
                    string storageContainer = string.Format(Constant.Storage.Partner.SigniantContainer, storageAccount, accountKey, containerName);
                    result = string.Concat("{", storageContainer, "}");
                    break;
                case TransferService.AsperaFasp:
                    AsperaClient asperaClient = new AsperaClient(authToken);
                    storageContainer = string.Format(Constant.Storage.Partner.AsperaContainer, storageAccount, WebUtility.UrlEncode(accountKey));
                    result = asperaClient.GetTransferSpecs(storageContainer, containerName, filePaths, false);
                    break;
            } 
            return Json(result);
        }

        public JsonResult file(string name, int chunk, int chunks, string storageAccount)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            string containerName = Constant.Storage.Blob.Container.Upload;
            Stream inputStream = this.Request.Form.Files[0].OpenReadStream();
            Storage.UploadFile(authToken, storageAccount, containerName, inputStream, name, chunk, chunks);
            return Json(chunk);
        }

        public IActionResult signiant()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);

            string settingKey = Constant.AppSettingKey.SigniantTransferApi;
            ViewData["signiantTransferApi"] = AppSetting.GetValue(settingKey);

            string attributeName = Constant.UserAttribute.SigniantServiceGateway;
            ViewData["signiantServiceGateway"] = AuthToken.GetClaimValue(authToken, attributeName);

            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }

        public IActionResult aspera()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);

            string settingKey = Constant.AppSettingKey.AsperaTransferApi;
            ViewData["asperaTransferApi"] = AppSetting.GetValue(settingKey);

            string attributeName = Constant.UserAttribute.AsperaServiceGateway;
            ViewData["asperaServiceGateway"] = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.AsperaAccountId;
            ViewData["asperaAccountId"] = AuthToken.GetClaimValue(authToken, attributeName);

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
