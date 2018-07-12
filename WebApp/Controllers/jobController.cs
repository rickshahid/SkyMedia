using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class JobController : Controller
    {
        internal static Job Create(MediaClient mediaClient, string transformName, string jobName, string jobDescription, Priority jobPriority, string jobInputAssetName, string[] jobOutputAssetNames)
        {
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputAssetName = jobInputAssetName,
                OutputAssetNames = jobOutputAssetNames
            };
            Job job = mediaClient.CreateJob(transformName, mediaJob);
            MediaPublish mediaPublish = new MediaPublish()
            {
                Id = job.Name,
                TransformName = transformName,
                MediaAccount = mediaClient.MediaAccount,
                UserAccount = mediaClient.UserAccount
            };
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.OutputPublish;
                databaseClient.UpsertDocument(collectionId, mediaPublish);
            }
            return job;
        }

        internal static Job Create(MediaClient mediaClient, string transformName, string jobInputAssetName, string[] jobOutputAssetNames)
        {
            return Create(mediaClient, transformName, string.Empty, string.Empty, Priority.Normal, jobInputAssetName, jobOutputAssetNames);
        }

        public JsonResult Create(string transformName, string jobName, string jobDescription, Priority jobPriority, string jobInputAssetName, string[] jobOutputAssetNames)
        {
            Job job;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                job = Create(mediaClient, transformName, jobName, jobDescription, jobPriority, jobInputAssetName, jobOutputAssetNames);
            }
            return Json(job);
        }

        public void Cancel(string jobName, string transformName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                mediaClient.CancelJob(transformName, jobName);
            }
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            }
            return View();
        }
    }
}