using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

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
                    Text = Constant.TextFormatter.FormatValue(mediaTransform.Name),
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
                    Text = Constant.TextFormatter.FormatValue(streamingPolicy.Name),
                    Value = streamingPolicy.Name
                };
                policies.Add(policy);
            }
            return policies.ToArray();
        }

        public JsonResult Create(string transformName, string jobName, string jobDescription, Priority jobPriority, string jobData,
                                 string inputFileUrl, string inputAssetName, MediaJobOutputMode outputAssetMode, string streamingPolicyName)
        {
            try
            {
                Job job = null;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    bool videoAnalyzerPreset = false;
                    bool audioAnalyzerPreset = false;
                    Transform transform = mediaClient.GetEntity<Transform>(MediaEntity.Transform, transformName);
                    foreach (TransformOutput transformOutput in transform.Outputs)
                    {
                        if (transformOutput.Preset is VideoAnalyzerPreset)
                        {
                            videoAnalyzerPreset = true;
                        }
                        else if (transformOutput.Preset is AudioAnalyzerPreset)
                        {
                            audioAnalyzerPreset = true;
                        }
                    }
                    string insightId = null;
                    Asset inputAsset = null;
                    string assetDescription = null;
                    if (!string.IsNullOrEmpty(inputAssetName))
                    {
                        inputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, inputAssetName);
                        assetDescription = inputAsset.Description;
                    }
                    if (mediaClient.IndexerEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
                    {
                        bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                        insightId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, inputAsset, null, jobPriority, true, audioOnly);
                    }
                    if (!string.IsNullOrEmpty(transformName))
                    {
                        if (string.IsNullOrEmpty(inputFileUrl) && inputAsset != null)
                        {
                            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, inputAsset.StorageAccountName);
                            MediaAsset mediaAsset = new MediaAsset(mediaClient, inputAsset);
                            string fileName = mediaAsset.Files[0].Name;
                            inputFileUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                        }
                        string[] assetDescriptions = new string[] { assetDescription };
                        string[] assetAlternateIds = new string[] { insightId };
                        job = mediaClient.CreateJob(authToken, transformName, jobName, jobDescription, jobPriority, JObject.Parse(jobData), inputFileUrl, inputAssetName, outputAssetMode, assetDescriptions, assetAlternateIds, streamingPolicyName);
                    }
                }
                return Json(job);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Update(string transformName, string jobName, string jobDescription, Priority jobPriority)
        {
            try
            {
                Job job;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    job = mediaClient.UpdateJob(transformName, jobName, jobDescription, jobPriority);
                }
                return Json(job);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Publish(string entityName, string parentName, string insightId, bool unpublish)
        {
            try
            {
                MediaJobPublish jobPublish;
                if (unpublish)
                {
                    string authToken = HomeController.GetAuthToken(Request, Response);
                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, entityName, parentName);
                        foreach (JobOutputAsset outputAsset in job.Outputs)
                        {
                            mediaClient.DeleteLocators(outputAsset.AssetName);
                        }
                    }
                    jobPublish = new MediaJobPublish()
                    {
                        UserNotification = new UserNotification()
                        {
                            JobOutputMessage = string.Format(Constant.Message.JobOutputUnpublished, entityName)
                        }
                    };
                }
                else
                {
                    jobPublish = MediaClient.PublishJobOutput(entityName, insightId);
                }
                return Json(jobPublish);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Refresh(string[] transformNames, string[] jobNames)
        {
            try
            {
                List<Job> jobs = new List<Job>();
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    for (int i = 0; i < jobNames.Length; i++)
                    {
                        string transformName = transformNames[i];
                        string jobName = jobNames[i];
                        Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
                        jobs.Add(job);
                    }
                }
                return Json(jobs.ToArray());
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Cancel(string transformName, string jobName)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.CancelJob(transformName, jobName);
                }
                return Json(jobName);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
                ViewData["transforms"] = GetTransforms(mediaClient);
                ViewData["streamingPolicies"] = GetStreamingPolicies(mediaClient);
            }
            return View();
        }

        public IActionResult Item(string transformName, string jobName)
        {
            List<Job> jobs = new List<Job>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
                if (job != null)
                {
                    jobs.Add(job);
                }
            }
            ViewData["transformJobs"] = jobs.ToArray();
            return View();
        }
    }
}