using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class workflowController : Controller
    {
        public JsonResult ingest(string[] fileNames, string storageAccount, bool storageEncryption, string inputAssetName, bool multipleFileAsset, MediaJob mediaJob)
        {
            string directoryId = Startup.GetDirectoryId(this.Request);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            MediaJobInput[] jobInputs = Workflow.CreateJobInputs(authToken, mediaClient, storageAccount, storageEncryption, inputAssetName, multipleFileAsset, fileNames);
            object output = Workflow.SubmitJob(directoryId, authToken, mediaClient, jobInputs, mediaJob);
            return Json(output);
        }

        public JsonResult start(MediaJobInput[] jobInputs, MediaJob mediaJob)
        {
            string directoryId = Startup.GetDirectoryId(this.Request);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            jobInputs = Workflow.GetJobInputs(mediaClient, jobInputs);
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
            object output = Workflow.SubmitJob(directoryId, authToken, mediaClient, jobInputs, mediaJob);
            return Json(output);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            homeController.SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}