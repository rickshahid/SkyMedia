﻿using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        private Asset[] CreateInputAssets(MediaClient mediaClient, string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames)
        {
            List<Asset> inputAssets = new List<Asset>();
            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, storageAccount);
            foreach (string fileName in fileNames)
            {
                string sourceContainer = Constant.Storage.BlobContainer.MediaServices;
                Asset inputAsset = mediaClient.CreateAsset(blobClient, blobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, fileName);
                inputAssets.Add(inputAsset);
            }
            return inputAssets.ToArray();
        }

        public JsonResult Workflow(string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames,
                                   bool adaptiveStreaming, bool thumbnailImages, bool videoAnalyzer, bool audioAnalyzer, bool videoIndexer, bool audioIndexer)
        {
            try
            {
                Asset[] inputAssets;
                List<Job> jobs = new List<Job>();
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    Transform transform = mediaClient.CreateTransform(adaptiveStreaming, thumbnailImages, videoAnalyzer, audioAnalyzer, videoIndexer, audioIndexer);
                    inputAssets = CreateInputAssets(mediaClient, storageAccount, assetName, assetDescription, assetAlternateId, fileNames);
                    foreach (Asset inputAsset in inputAssets)
                    {
                        Job job = null;
                        string indexId = null;
                        if (mediaClient.IndexerEnabled() && (videoIndexer || audioIndexer))
                        {
                            bool audioOnly = !videoIndexer && audioIndexer;
                            indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, inputAsset, null, Priority.Normal, true, audioOnly);
                        }
                        if (transform != null)
                        {
                            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, inputAsset.StorageAccountName);
                            MediaAsset mediaAsset = new MediaAsset(mediaClient, inputAsset);
                            string fileName = mediaAsset.Files[0].Name;
                            string inputFileUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                            MediaJobOutputMode outputAssetMode = MediaJobOutputMode.InputAsset;
                            string[] outputAssetDescriptions = new string[] { assetDescription };
                            string[] outputAssetAlternateIds = new string[] { indexId };
                            PredefinedStreamingPolicy streamingPolicy = PredefinedStreamingPolicy.ClearStreamingOnly;
                            job = mediaClient.CreateJob(authToken, transform.Name, null, null, Priority.Normal, null, inputFileUrl, inputAsset.Name, outputAssetMode, outputAssetDescriptions, outputAssetAlternateIds, streamingPolicy);
                        }
                        if (job != null)
                        {
                            if (!string.IsNullOrEmpty(indexId))
                            {
                                job.CorrelationData.Add("indexId", indexId);
                            }
                            jobs.Add(job);
                        }
                        else
                        {
                            inputAsset.AlternateId = indexId;
                        }
                    }
                }
                return jobs.Count > 0 ? Json(jobs.ToArray()) : Json(inputAssets);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Publish(string assetName)
        {
            try
            {
                string playerUrl;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                    string streamingPolicyName = PredefinedStreamingPolicy.DownloadAndClearStreaming;
                    StreamingLocator locator = mediaClient.CreateLocator(asset.Name, asset.Name, streamingPolicyName, null);
                    playerUrl = mediaClient.GetPlayerUrl(locator);
                }
                return Json(playerUrl);
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
                    StorageBlobClient assetBlobClient = new StorageBlobClient(mediaClient.MediaAccount, mediaClient.PrimaryStorageAccount);
                    for (int i = 1; i <= assetCount; i++)
                    {
                        int assetId = i % 2 == 0 ? 2 : i % 2;
                        List<Task> uploadTasks = new List<Task>();
                        string containerName = Constant.Storage.BlobContainer.MediaServices;
                        string directoryPath = string.Concat(assetType, "/", assetId.ToString());
                        string assetName = MediaAsset.GetAssetName(sourceBlobClient, containerName, directoryPath);
                        assetName = string.Concat(i.ToString(), Constant.Media.Asset.NameDelimiter, assetName);
                        Asset asset = mediaClient.CreateAsset(mediaClient.PrimaryStorageAccount, assetName);
                        MediaFile[] sourceFiles = MediaAsset.GetAssetFiles(sourceBlobClient, containerName, directoryPath);
                        foreach (MediaFile sourceFile in sourceFiles)
                        {
                            CloudBlockBlob sourceBlob = sourceBlobClient.GetBlockBlob(containerName, sourceFile.Name);
                            CloudBlockBlob assetBlob = assetBlobClient.GetBlockBlob(asset.Container, sourceFile.Name);
                            Stream sourceStream = sourceBlob.OpenReadAsync().Result;
                            Task uploadTask = assetBlob.UploadFromStreamAsync(sourceStream);
                            uploadTasks.Add(uploadTask);
                        }
                        if (assetPublish)
                        {
                            Task.WaitAll(uploadTasks.ToArray());
                            string streamingPolicyName = assetType == Constant.Media.Asset.SingleBitrate ? PredefinedStreamingPolicy.DownloadOnly : PredefinedStreamingPolicy.ClearStreamingOnly;
                            mediaClient.CreateLocator(asset.Name, asset.Name, streamingPolicyName, null);
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

        public JsonResult Insight(string assetName, string fileName, string indexId)
        {
            try
            {
                JObject insight;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    if (!string.IsNullOrEmpty(indexId))
                    {
                        insight = mediaClient.IndexerGetInsight(indexId);
                    }
                    else
                    {
                        Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                        StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
                        CloudBlockBlob fileBlob = blobClient.GetBlockBlob(asset.Container, fileName);
                        using (Stream fileStream = fileBlob.OpenReadAsync().Result)
                        {
                            StreamReader fileReader = new StreamReader(fileStream);
                            string fileData = fileReader.ReadToEnd();
                            insight = JObject.Parse(fileData);
                        }
                    }
                }
                return Json(insight);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Reindex(string indexId)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.IndexerReindexVideo(indexId, Priority.Normal);
                }
                return Json(indexId);
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

        public IActionResult Index()
        {
            List<MediaAsset> mediaAssets = new List<MediaAsset>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                IPage<Asset> assets = mediaClient.GetEntities<Asset>(MediaEntity.Asset);
                foreach (Asset asset in assets)
                {
                    MediaAsset mediaAsset = new MediaAsset(mediaClient, asset);
                    mediaAssets.Add(mediaAsset);
                }
            }
            ViewData["assets"] = mediaAssets.ToArray();
            return View();
        }

        public IActionResult Item(string assetName)
        {
            List<MediaAsset> mediaAssets = new List<MediaAsset>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                if (asset != null)
                {
                    MediaAsset mediaAsset = new MediaAsset(mediaClient, asset);
                    mediaAssets.Add(mediaAsset);
                }
            }
            ViewData["assets"] = mediaAssets.ToArray();
            return View();
        }
    }
}