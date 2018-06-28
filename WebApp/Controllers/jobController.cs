using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class JobController : Controller
    {
        internal static Job Create(string authToken, string transformName, string jobName, JobInput jobInput, string[] outputAssetNames, MediaPublish mediaPublish)
        {
            Job job;
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                job = mediaClient.CreateJob(transformName, jobName, jobInput, outputAssetNames);
                mediaPublish.Id = job.Name;
                mediaPublish.TransformName = transformName;
                mediaPublish.MediaAccount = mediaClient.MediaAccount;
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    string collectionId = Constant.Database.Collection.OutputPublish;
                    databaseClient.UpsertDocument(collectionId, mediaPublish);
                }
            }
            return job;
        }

        public JsonResult Cancel(string jobName, string transformName)
        {
            string requestId = string.Empty;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                requestId = mediaClient.CancelJob(transformName, jobName);
            }
            return Json(requestId);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            }
            return View();
        }
    }
}