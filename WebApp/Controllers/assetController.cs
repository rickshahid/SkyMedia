using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        private Asset[] CreateInputAssets(MediaClient mediaClient, string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames)
        {
            List<Asset> inputAssets = new List<Asset>();
            BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, storageAccount);
            foreach (string fileName in fileNames)
            {
                string sourceContainer = Constant.Storage.BlobContainer.FileUpload;
                Asset inputAsset = mediaClient.CreateAsset(blobClient, blobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, fileName);
                inputAssets.Add(inputAsset);
            }
            return inputAssets.ToArray();
        }

        public JsonResult Create(string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames,
                                 bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset, string streamingPolicyName)
        {
            Asset[] inputAssets;
            List<Job> jobs = new List<Job>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Transform transform = TransformController.Create(mediaClient, standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
                inputAssets = CreateInputAssets(mediaClient, storageAccount, assetName, assetDescription, assetAlternateId, fileNames);
                foreach (Asset inputAsset in inputAssets)
                {
                    string indexId = null;
                    if (mediaClient.IndexerIsEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
                    {
                        BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, storageAccount);
                        MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, inputAsset);
                        string fileName = mediaAsset.Files[0].Name;
                        string videoUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                        bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                        indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, videoUrl, inputAsset.Name, inputAsset.Description, string.Empty, audioOnly);
                    }
                    Job job = null;
                    if (transform != null)
                    {
                        job = mediaClient.CreateJob(transform.Name, null, null, Priority.Normal, inputAsset.Name, null, assetDescription, indexId, streamingPolicyName);
                        jobs.Add(job);
                    }
                    if (job != null)
                    {
                        job.CorrelationData.Add("indexId", indexId);
                    }
                    else
                    {
                        inputAsset.AlternateId = indexId;
                    }
                }
            }
            return jobs.Count > 0 ? Json(jobs.ToArray()) : Json(inputAssets);
        }

        public JsonResult Find(string assetName)
        {
            bool entityFound = true;
            if (!string.IsNullOrEmpty(assetName))
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                    entityFound = asset != null;
                }
            }
            return Json(entityFound);
        }

        public IActionResult Sprite()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}