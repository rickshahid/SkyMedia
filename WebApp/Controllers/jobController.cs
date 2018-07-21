using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class JobController : Controller
    {
        private static SelectListItem[] GetTransforms(MediaClient mediaClient)
        {
            List<SelectListItem> transforms = new List<SelectListItem>
            {
                new SelectListItem()
            };
            Transform[] mediaTransforms = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            foreach (Transform mediaTransform in mediaTransforms)
            {
                SelectListItem transform = new SelectListItem()
                {
                    Text = mediaTransform.Name,
                    Value = mediaTransform.Name
                };
                transforms.Add(transform);
            }
            return transforms.ToArray();
        }

        private static SelectListItem[] GetStreamingPolicies(MediaClient mediaClient)
        {
            List<SelectListItem> policies = new List<SelectListItem>
            {
                new SelectListItem()
            };
            StreamingPolicy[] streamingPolicies = mediaClient.GetAllEntities<StreamingPolicy>(MediaEntity.StreamingPolicy);
            foreach (StreamingPolicy streamingPolicy in streamingPolicies)
            {
                SelectListItem policy = new SelectListItem()
                {
                    Text = streamingPolicy.Name,
                    Value = streamingPolicy.Name
                };
                policies.Add(policy);
            }
            return policies.ToArray();
        }

        internal static Job Create(MediaClient mediaClient, string transformName, string jobName, string jobDescription, Priority jobPriority, string inputAssetName, string[] outputAssetNames, string streamingPolicyName)
        {
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputAssetName = inputAssetName,
                OutputAssetNames = outputAssetNames
            };
            Job job = mediaClient.CreateJob(mediaClient, transformName, mediaJob);
            MediaPublish mediaPublish = new MediaPublish()
            {
                Id = job.Name,
                TransformName = transformName,
                StreamingPolicyName = streamingPolicyName, 
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

        public JsonResult Create(string transformName, string jobName, string jobDescription, string jobPriority, string inputAssetName, string streamingPolicyName)
        {
            Job job;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                job = Create(mediaClient, transformName, jobName, jobDescription, jobPriority, inputAssetName, null, streamingPolicyName);
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
                ViewData["transforms"] = GetTransforms(mediaClient);
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
                ViewData["streamingPolicies"] = GetStreamingPolicies(mediaClient);
            }
            return View();
        }
    }
}