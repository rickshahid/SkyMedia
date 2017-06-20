using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class workflowController : Controller
    {
        public JsonResult ingest(string[] fileNames, string storageAccount, bool storageEncryption, string inputAssetName, bool multipleFileAsset,
                                 bool indexInputAsset, string indexerLanguage, bool indexerPrivacyPublic, MediaJob mediaJob)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            MediaAssetInput[] inputAssets = Workflow.CreateInputAssets(authToken, mediaClient, storageAccount, storageEncryption, inputAssetName, multipleFileAsset, indexInputAsset, indexerLanguage, indexerPrivacyPublic, fileNames);
            object result = Workflow.SubmitJob(authToken, mediaClient, storageAccount, inputAssets, mediaJob);
            return Json(result);
        }

        public JsonResult start(MediaAssetInput[] inputAssets, MediaJob mediaJob)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            inputAssets = Workflow.GetInputAssets(mediaClient, inputAssets);
            if (mediaJob.Tasks != null)
            {
                using (CosmosClient cosmosClient = new CosmosClient(false))
                {
                    foreach (MediaJobTask jobTask in mediaJob.Tasks)
                    {
                        if (!string.IsNullOrEmpty(jobTask.ProcessorDocumentId))
                        {
                            JObject processorConfig = cosmosClient.GetDocument(jobTask.ProcessorDocumentId);
                            jobTask.ProcessorConfig = processorConfig.ToString();
                        }
                    }
                }
            }
            object result = Workflow.SubmitJob(authToken, mediaClient, null, inputAssets, mediaJob);
            return Json(result);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}
