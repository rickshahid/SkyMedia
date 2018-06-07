using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class jobController : Controller
    {
        internal static Job CreateJob(string authToken, string transformName, string jobName, JobInput jobInput, string outputAssetName, MediaPublish mediaPublish)
        {
            Job job;
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                job = mediaClient.CreateJob(transformName, jobName, jobInput, outputAssetName);
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

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            }
            return View();
        }
    }
}