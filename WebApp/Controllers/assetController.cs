using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        public async Task<JsonResult> Workflow(string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames,
                                               bool adaptiveStreaming, bool contentAwareEncoding, bool contentProtection, bool thumbnailImages, bool thumbnailSprite,
                                               bool videoAnalyzer, bool audioAnalyzer, bool faceDetector, bool videoIndexer, bool audioIndexer)
        {
            try
            {
                List<MediaWorkflowEntity> newEntities = new List<MediaWorkflowEntity>();
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    Transform transform = mediaClient.GetTransform(adaptiveStreaming, contentAwareEncoding, thumbnailImages, thumbnailSprite, videoAnalyzer, audioAnalyzer, faceDetector, videoIndexer, audioIndexer);
                    Asset[] inputAssets = await mediaClient.CreateAssets(storageAccount, assetName, assetDescription, assetAlternateId, fileNames);
                    foreach (Asset inputAsset in inputAssets)
                    {
                        Job job = null;
                        string insightId = null;
                        Priority jobPriority = Priority.Normal;
                        MediaJobOutputPublish outputPublish = new MediaJobOutputPublish()
                        {
                            StreamingPolicyName = contentProtection ? PredefinedStreamingPolicy.ClearKey : PredefinedStreamingPolicy.DownloadAndClearStreaming
                        };
                        if (mediaClient.IndexerEnabled() && (videoIndexer || audioIndexer))
                        {
                            insightId = mediaClient.IndexerUploadVideo(null, inputAsset, jobPriority, videoIndexer, audioIndexer);
                        }
                        if (transform != null)
                        {
                            MediaJobOutputInsight outputInsight = new MediaJobOutputInsight()
                            {
                                Id = insightId,
                                VideoIndexer = videoIndexer,
                                AudioIndexer = audioIndexer
                            };
                            job = mediaClient.CreateJob(transform.Name, null, null, jobPriority, null, inputAsset, null, outputPublish, outputInsight, true);
                        }
                        MediaWorkflowEntity newEntity = new MediaWorkflowEntity();
                        if (job != null)
                        {
                            newEntity.Type = MediaEntity.TransformJob;
                            newEntity.Id = job.Id;
                            newEntity.Name = job.Name;
                        }
                        else
                        {
                            newEntity.Type = MediaEntity.Asset;
                            newEntity.Id = inputAsset.Id;
                            newEntity.Name = inputAsset.Name;
                        }
                        newEntity.InsightId = insightId;
                        newEntities.Add(newEntity);
                    }
                }
                return Json(newEntities.ToArray());
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Publish(string entityName, bool unpublish)
        {
            try
            {
                string message;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    if (unpublish)
                    {
                        mediaClient.DeleteLocators(entityName);
                        message = string.Format(Constant.Message.AssetUnpublished, entityName);
                    }
                    else
                    {
                        Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, entityName);
                        string streamingPolicyName = PredefinedStreamingPolicy.DownloadAndClearStreaming;
                        StreamingLocator streamingLocator = mediaClient.GetStreamingLocator(asset.Name, asset.Name, streamingPolicyName, null);
                        message = mediaClient.GetStreamingUrl(streamingLocator, null);
                    }
                }
                return Json(message);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Find(string assetName)
        {
            try
            {
                Asset asset = null;
                if (!string.IsNullOrEmpty(assetName))
                {
                    string authToken = HomeController.GetAuthToken(Request, Response);
                    using (MediaClient mediaClient = new MediaClient(authToken))
                    {
                        asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                    }
                }
                return Json(asset);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Sprite()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["mediaStreams"] = Media.GetAccountStreams(authToken, mediaClient, 1, out int streamSkipCount, out int streamTunnerPageSize, out bool streamTunerLastPage);
            }
            return View();
        }

        public IActionResult Index(string assetName)
        {
            List<MediaAsset> mediaAssets = new List<MediaAsset>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                if (!string.IsNullOrEmpty(assetName))
                {
                    Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                    if (asset != null)
                    {
                        MediaAsset mediaAsset = new MediaAsset(mediaClient, asset);
                        mediaAssets.Add(mediaAsset);
                    }
                }
                else
                {
                    IPage<Asset> assets = mediaClient.GetEntities<Asset>(MediaEntity.Asset);
                    foreach (Asset asset in assets)
                    {
                        MediaAsset mediaAsset = new MediaAsset(mediaClient, asset);
                        mediaAssets.Add(mediaAsset);
                    }
                }
            }
            ViewData["assets"] = mediaAssets.ToArray();
            return View();
        }
    }
}