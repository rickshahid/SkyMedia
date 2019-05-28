using System.Net;
using System.Collections.Generic;

using Microsoft.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.Storage.Blob;

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

        internal static SelectListItem[] GetStreamingPolicies(MediaClient mediaClient)
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

        public JsonResult Create(string transformName, string jobName, string jobDescription, string jobPriority, string inputFileUrl,
                                 string inputAssetName, StandardBlobTier inputAssetStorageTier, string streamingPolicyName, ContentProtection contentProtection)
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
                    MediaJobOutputPublish outputAssetPublish = new MediaJobOutputPublish()
                    {
                        InputAssetStorageTier = inputAssetStorageTier,
                        StreamingPolicyName = streamingPolicyName,
                        ContentProtection = contentProtection,
                    };
                    string insightId = null;
                    Asset inputAsset = string.IsNullOrEmpty(inputAssetName) ? null : mediaClient.GetEntity<Asset>(MediaEntity.Asset, inputAssetName);
                    bool indexerEnabled = mediaClient.IndexerEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset);
                    if (indexerEnabled)
                    {
                        insightId = mediaClient.IndexerUploadVideo(inputFileUrl, inputAsset, jobPriority, videoAnalyzerPreset, audioAnalyzerPreset);
                    }
                    if (!string.IsNullOrEmpty(transformName))
                    {
                        MediaJobOutputInsight outputInsight = new MediaJobOutputInsight()
                        {
                            Id = insightId,
                            VideoIndexer = videoAnalyzerPreset,
                            AudioIndexer = audioAnalyzerPreset
                        };
                        job = mediaClient.CreateJob(transformName, jobName, jobDescription, jobPriority, inputFileUrl, inputAsset, null, outputAssetPublish, outputInsight, true);
                    }
                }
                return Json(job);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Publish(string entityName, string parentName, bool unpublish)
        {
            try
            {
                MediaPublishNotification publishNotification;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    if (unpublish)
                    {
                        publishNotification = mediaClient.UnpublishJobOutput(parentName, entityName);
                    }
                    else
                    {
                        string eventType = Constant.Media.Job.EventType.Finished;
                        publishNotification = mediaClient.PublishJobOutput(parentName, entityName, eventType);
                    }
                }
                return Json(publishNotification);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Index(string transformName, string jobName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Job[] jobs = new Job[] { };
                if (!string.IsNullOrEmpty(jobName))
                {
                    Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
                    if (job != null)
                    {
                        jobs = new Job[] { job };
                    }
                }
                else
                {
                    jobs = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
                }
                ViewData["transformJobs"] = jobs;
                ViewData["transforms"] = GetTransforms(mediaClient);
                ViewData["streamingPolicies"] = GetStreamingPolicies(mediaClient);
            }
            return View();
        }
    }
}