using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class workflowController : Controller
    {
        public JsonResult ingest(string storageAccount, bool storageEncryption, string inputAssetName, bool multipleFileAsset, string[] fileNames, MediaJob mediaJob)
        {
            string directoryId = homeController.GetDirectoryId(this.Request);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            MediaJobInput[] jobInputs = Workflow.GetJobInputs(authToken, mediaClient, storageAccount, storageEncryption, inputAssetName, multipleFileAsset, fileNames);
            object jobOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs);
            return Json(jobOutput);
        }

        public JsonResult start(string[] assetIds, MediaJob mediaJob)
        {
            string directoryId = homeController.GetDirectoryId(this.Request);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            if (mediaJob.Tasks != null)
            {
                using (DocumentClient documentClient = new DocumentClient())
                {
                    foreach (MediaJobTask jobTask in mediaJob.Tasks)
                    {
                        if (!string.IsNullOrEmpty(jobTask.ProcessorDocumentId))
                        {
                            JObject processorConfig = documentClient.GetDocument(jobTask.ProcessorDocumentId);
                            if (processorConfig != null)
                            {
                                jobTask.ProcessorConfig = processorConfig.ToString();
                            }
                        }
                    }
                }
            }
            MediaJobInput[] jobInputs = Workflow.GetJobInputs(mediaClient, assetIds);
            object jobOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs);
            return Json(jobOutput);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}