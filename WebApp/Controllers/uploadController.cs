using System.IO;
using System.Net;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

using SkyMedia.ServiceBroker;

namespace SkyMedia.WebApp.Controllers
{
    public class uploadController : Controller
    {
        public JsonResult manifest(string manifestId, string inputAssetName, string storageAccount, bool storageEncryption,
                                   bool multipleFileAsset, bool uploadFileWatcher, string[] fileNames)
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            if (string.IsNullOrEmpty(inputAssetName) && fileNames.Length > 0)
            {
                inputAssetName = fileNames[0];
            }
            IIngestManifest manifest = mediaClient.SetManifest(manifestId, inputAssetName, storageAccount, storageEncryption, multipleFileAsset, uploadFileWatcher, fileNames);
            return Json(manifest);
        }

        public JsonResult storage(TransferService transferService, string[] filePaths, string storageAccount, string containerName)
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);
            string accountKey = Storage.GetAccountKey(authToken, storageAccount);
            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            blobClient.GetContainer(containerName); // CreateIfNotExists
            object result = null;
            switch (transferService)
            {
                case TransferService.SigniantFlight:
                    string storageContainer = string.Format(Constants.Storage.SigniantContainer, storageAccount, accountKey, containerName);
                    result = string.Concat("{", storageContainer, "}");
                    break;
                case TransferService.AsperaFasp:
                    AsperaClient asperaClient = new AsperaClient(authToken);
                    storageContainer = string.Format(Constants.Storage.AsperaContainer, storageAccount, WebUtility.UrlEncode(accountKey));
                    result = asperaClient.GetTransferSpecs(storageContainer, containerName, filePaths, false);
                    break;
            } 
            return Json(result);
        }

        public JsonResult file(string name, int chunk, int chunks, string storageAccount, string storageContainer)
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);
            BlobClient blobClient = new BlobClient(authToken, storageAccount);
            Stream inputStream = this.Request.Form.Files[0].OpenReadStream();
            string containerName = Path.GetFileName(storageContainer);
            if (chunks == 0)
            {
                blobClient.UploadFile(inputStream, containerName, null, name);
            }
            else
            {
                bool lastBlock = (chunk == chunks - 1);
                string attributeName = Constants.UserAttribute.UserId;
                string userId = AuthToken.GetClaimValue(authToken, attributeName);
                blobClient.UploadBlock(userId, inputStream, containerName, null, name, chunk, lastBlock);
            }
            return Json(chunk);
        }

        internal static void SetViewData(string authToken, ViewDataDictionary viewData)
        {
            viewData["storageContainer"] = Constants.Storage.ContainerNames.Upload;

            string attributeName = Constants.UserAttribute.SigniantAccountKey;
            viewData["signiantAccountKey"] = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.AsperaAccountKey;
            viewData["asperaAccountKey"] = AuthToken.GetClaimValue(authToken, attributeName);

            string settingKey = Constants.AppSettings.StorageBlockChunkSize;
            viewData["blockChunkSize"] = AppSetting.GetValue(settingKey);

            settingKey = Constants.AppSettings.StorageMaxFileSize;
            viewData["maxFileSize"] = AppSetting.GetValue(settingKey);

            settingKey = Constants.AppSettings.StorageMaxRetryCount;
            viewData["maxRetryCount"] = AppSetting.GetValue(settingKey);

            viewData["storageAccount"] = Selections.GetStorageAccounts(authToken);
            viewData["mediaProcessor1"] = Selections.GetMediaProcessors(false);
            viewData["processorConfig1"] = new List<SelectListItem>();
            viewData["spokenLanguages1"] = Selections.GetSpokenLanguages();
        }

        public IActionResult index()
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);
            SetViewData(authToken, this.ViewData);
            return View();
        }

        public IActionResult signiant()
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);

            string settingKey = Constants.AppSettings.SigniantTransferApi;
            ViewData["signiantTransferApi"] = AppSetting.GetValue(settingKey);

            string attributeName = Constants.UserAttribute.SigniantServiceGateway;
            ViewData["signiantServiceGateway"] = AuthToken.GetClaimValue(authToken, attributeName);

            SetViewData(authToken, this.ViewData);
            return View();
        }

        public IActionResult aspera()
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);

            string settingKey = Constants.AppSettings.AsperaTransferApi;
            ViewData["asperaTransferApi"] = AppSetting.GetValue(settingKey);

            string attributeName = Constants.UserAttribute.AsperaServiceGateway;
            ViewData["asperaServiceGateway"] = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.AsperaAccountId;
            ViewData["asperaAccountId"] = AuthToken.GetClaimValue(authToken, attributeName);

            SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}
