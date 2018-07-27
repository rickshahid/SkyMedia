using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.WindowsAzure.Storage.Blob;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private void SetStyleHost()
        {
            ViewData["cssHost"] = string.Concat(Request.Scheme, "://", Request.Host.Value);
        }

        public void SignUpIn()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void ProfileEdit()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public void PasswordReset()
        {
            HttpContext.ChallengeAsync().Wait();
        }

        public JsonResult CreateAssets(int assetCount, string assetType, bool assetPublish)
        {
            string exception = string.Empty;
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    string storageAccount = Path.GetFileName(mediaClient.PrimaryStorage.Id);
                    BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, storageAccount);
                    string assetPath = Path.Combine(Directory.GetCurrentDirectory(), Constant.WebModels, Constant.Media.Asset.ModelDirectory, assetType);
                    string[] assetFilePaths = Directory.GetFiles(assetPath);
                    string streamingPolicyName = assetType == Constant.Media.Asset.SingleBitrate ? PredefinedStreamingPolicy.DownloadOnly : PredefinedStreamingPolicy.ClearStreamingOnly;
                    for (int i = 1; i <= assetCount; i++)
                    {
                        string assetId = string.Concat(assetType, Constant.Media.Asset.NameDefault, i.ToString());
                        Asset asset = mediaClient.CreateAsset(storageAccount, assetId, assetId, assetId);
                        foreach (string assetFilePath in assetFilePaths)
                        {
                            string fileName = Path.GetFileName(assetFilePath);
                            CloudBlockBlob assetBlob = blobClient.GetBlockBlob(asset.Container, fileName);
                            assetBlob.UploadFromFileAsync(assetFilePath).Wait();
                        }
                        if (assetPublish)
                        {
                            mediaClient.CreateLocator(asset.Name, asset.Name, streamingPolicyName, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            return Json(exception);
        }

        public void DeleteEntities(bool liveOnly)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Account.DeleteEntities(mediaClient, liveOnly);
            }
        }

        public void DeleteEntity(string gridId, string entityName, string parentEntityName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                switch (gridId)
                {
                    case "assets":
                        mediaClient.DeleteEntity(MediaEntity.Asset, entityName);
                        break;
                    case "transforms":
                        mediaClient.DeleteEntity(MediaEntity.Transform, entityName);
                        break;
                    case "transformJobs":
                        mediaClient.DeleteEntity(MediaEntity.TransformJob, entityName, parentEntityName);
                        break;
                    case "contentKeyPolicies":
                        mediaClient.DeleteEntity(MediaEntity.ContentKeyPolicy, entityName);
                        break;
                    case "streamingPolicies":
                        mediaClient.DeleteEntity(MediaEntity.StreamingPolicy, entityName);
                        break;
                    case "streamingEndpoints":
                        mediaClient.DeleteEntity(MediaEntity.StreamingEndpoint, entityName);
                        break;
                    case "streamingLocators":
                        mediaClient.DeleteEntity(MediaEntity.StreamingLocator, entityName);
                        break;
                    case "liveEvents":
                        mediaClient.DeleteEntity(MediaEntity.LiveEvent, entityName);
                        break;
                    case "liveEventOutputs":
                        mediaClient.DeleteEntity(MediaEntity.LiveEventOutput, entityName, parentEntityName);
                        break;
                    case "indexerInsights":
                        mediaClient.IndexerDeleteVideo(entityName, true);
                        break;
                }
            }
        }

        public IActionResult SignOut()
        {
            SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        public IActionResult SignIn()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult Password()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult Profile()
        {
            SetStyleHost();
            return View();
        }

        public IActionResult StorageAccounts()
        {
            List<MediaStorage> storageAccounts = new List<MediaStorage>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                foreach (StorageAccount storageAccount in mediaClient.StorageAccounts)
                {
                    MediaStorage mediaStorage = new MediaStorage(authToken, storageAccount);
                    storageAccounts.Add(mediaStorage);
                }
                ViewData["storageAccounts"] = storageAccounts.ToArray();
            }
            return View();
        }

        public IActionResult Assets()
        {
            List<MediaAsset> mediaAssets = new List<MediaAsset>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                IPage<Asset> assets = mediaClient.GetEntities<Asset>(MediaEntity.Asset);
                foreach (Asset asset in assets)
                {
                    MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, asset);
                    mediaAssets.Add(mediaAsset);
                }
            }
            ViewData["assets"] = mediaAssets.ToArray();
            return View();
        }

        public IActionResult Transforms()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            }
            return View();
        }

        public IActionResult TransformJobs()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            }
            return View();
        }

        public IActionResult ContentKeyPolicies()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["contentKeyPolicies"] = mediaClient.GetAllEntities<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy);
            }
            return View();
        }

        public IActionResult StreamingPolicies()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingPolicies"] = mediaClient.GetAllEntities<StreamingPolicy>(MediaEntity.StreamingPolicy);
            }
            return View();
        }

        public IActionResult StreamingEndpoints()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingEndpoints"] = mediaClient.GetAllEntities<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            }
            return View();
        }

        public IActionResult StreamingLocators()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["streamingLocators"] = mediaClient.GetAllEntities<StreamingLocator>(MediaEntity.StreamingLocator);
            }
            return View();
        }

        public IActionResult LiveEvents()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["liveEvents"] = mediaClient.GetAllEntities<LiveEvent>(MediaEntity.LiveEvent);
            }
            return View();
        }

        public IActionResult LiveEventOutputs()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["liveEventOutputs"] = mediaClient.GetAllEntities<LiveOutput>(MediaEntity.LiveEventOutput);
            }
            return View();
        }

        public IActionResult IndexerInsights()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["indexerInsights"] = mediaClient.IndexerGetInsights();
            }
            return View();
        }

        public IActionResult Operations()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["operations"] = mediaClient.GetOperations();
            }
            return View();
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["entityCounts"] = Account.GetEntityCounts(mediaClient);
            }
            return View();
        }
    }
}