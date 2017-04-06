using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class workflowController : Controller
    {
        public JsonResult upload(string[] fileNames, string storageAccount, bool storageEncryption, string inputAssetName,
                                 bool multipleFileAsset, bool publishInputAsset, MediaAssetInput[] inputAssets, MediaJob mediaJob)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            inputAssets = Workflow.CreateInputAssets(authToken, mediaClient, storageAccount, storageEncryption, inputAssetName, multipleFileAsset, publishInputAsset, fileNames);
            object result = Workflow.SubmitJob(authToken, mediaClient, storageAccount, inputAssets, mediaJob);
            return Json(result);
        }

        public JsonResult save(MediaAssetInput[] inputAssets, MediaJob mediaJob)
        {
            mediaJob.SaveAsTemplate = true;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            object result = Workflow.SubmitJob(authToken, mediaClient, null, inputAssets, mediaJob);
            return Json(result);
        }

        public JsonResult start(MediaAssetInput[] inputAssets, MediaJob mediaJob)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            inputAssets = Workflow.GetInputAssets(mediaClient, inputAssets);
            using (DatabaseClient databaseClient = new DatabaseClient(false))
            {
                foreach (MediaJobTask jobTask in mediaJob.Tasks)
                {
                    if (!string.IsNullOrEmpty(jobTask.ProcessorDocumentId))
                    {
                        JObject processorConfig = databaseClient.GetDocument(jobTask.ProcessorDocumentId);
                        jobTask.ProcessorConfig = processorConfig.ToString();
                    }
                }
            }
            object result = Workflow.SubmitJob(authToken, mediaClient, null, inputAssets, mediaJob);
            return Json(result);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            uploadController.SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}
