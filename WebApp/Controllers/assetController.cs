using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        private async Task<Asset[]> CreateInputAssets(MediaClient mediaClient, string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames)
        {
            List<Asset> inputAssets = new List<Asset>();
            foreach (string fileName in fileNames)
            {
                Asset inputAsset = await mediaClient.CreateAsset(storageAccount, assetName, assetDescription, assetAlternateId, fileName);
                inputAssets.Add(inputAsset);
            }
            return inputAssets.ToArray();
        }

        public async Task<JsonResult> Workflow(string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames,
                                               bool adaptiveStreaming, bool contentAwareEncoding, bool thumbnailImages, bool thumbnailSprite,
                                               bool videoAnalyzer, bool audioAnalyzer, bool faceDetector, bool videoIndexer, bool audioIndexer)
        {
            try
            {
                List<MediaWorkflowEntity> newEntities = new List<MediaWorkflowEntity>();
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    Transform transform = mediaClient.GetTransform(adaptiveStreaming, contentAwareEncoding, thumbnailImages, thumbnailSprite, videoAnalyzer, audioAnalyzer, faceDetector, videoIndexer, audioIndexer);
                    Asset[] inputAssets = await CreateInputAssets(mediaClient, storageAccount, assetName, assetDescription, assetAlternateId, fileNames);
                    StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, storageAccount);
                    foreach (Asset inputAsset in inputAssets)
                    {
                        Job job = null;
                        string insightId = null;
                        Priority jobPriority = Priority.Normal;
                        MediaJobOutputPublish outputAssetPublish = new MediaJobOutputPublish()
                        {
                            StreamingPolicyName = PredefinedStreamingPolicy.DownloadAndClearStreaming
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
                            job = mediaClient.CreateJob(transform.Name, null, null, jobPriority, null, inputAsset, null, outputAssetPublish, outputInsight, true);
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
                        mediaClient.DeleteStreamingLocators(entityName);
                        message = string.Format(Constant.Message.AssetUnpublished, entityName);
                    }
                    else
                    {
                        Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, entityName);
                        string streamingPolicyName = PredefinedStreamingPolicy.DownloadAndClearStreaming;
                        StreamingLocator streamingLocator = mediaClient.GetStreamingLocator(asset.Name, streamingPolicyName, null);
                        message = mediaClient.GetLocatorUrl(streamingLocator, null, true);
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

        public JsonResult Create(int assetCount, string assetType, bool assetPublish)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    StorageBlobClient sourceBlobClient = new StorageBlobClient();
                    StorageBlobClient assetBlobClient = new StorageBlobClient(mediaClient.MediaAccount, mediaClient.StorageAccount);
                    for (int i = 1; i <= assetCount; i++)
                    {
                        List<Task> uploadTasks = new List<Task>();
                        string containerName = Constant.Storage.Blob.WorkflowContainerName;
                        string directoryPath = assetType;
                        string assetName = MediaClient.GetAssetName(sourceBlobClient, containerName, directoryPath, out MediaFile[] sourceFiles);
                        assetName = string.Concat(i.ToString(), Constant.TextDelimiter.AssetName, assetName);
                        Asset asset = mediaClient.CreateAsset(mediaClient.StorageAccount, assetName);
                        foreach (MediaFile sourceFile in sourceFiles)
                        {
                            CloudBlockBlob sourceBlob = sourceBlobClient.GetBlockBlob(containerName, directoryPath, sourceFile.Name);
                            CloudBlockBlob assetBlob = assetBlobClient.GetBlockBlob(asset.Container, null, sourceFile.Name);
                            Stream sourceStream = sourceBlob.OpenReadAsync().Result;
                            Task uploadTask = assetBlob.UploadFromStreamAsync(sourceStream);
                            uploadTasks.Add(uploadTask);
                        }
                        if (assetPublish)
                        {
                            Task.WaitAll(uploadTasks.ToArray());
                            string streamingPolicyName = assetType == Constant.Media.Asset.SingleBitrate ? PredefinedStreamingPolicy.DownloadOnly : PredefinedStreamingPolicy.DownloadAndClearStreaming;
                            mediaClient.GetStreamingLocator(asset.Name, streamingPolicyName, null);
                        }
                    }
                }
                return Json(assetCount);
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
                ViewData["mediaStreams"] = Media.GetAccountStreams(authToken, mediaClient);
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