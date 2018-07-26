using System.IO;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        private static Asset[] CreateInputAssets(MediaClient mediaClient, string storageAccount, string assetName, string description, string alternateId, string[] fileNames)
        {
            List<Asset> inputAssets = new List<Asset>();
            foreach (string fileName in fileNames)
            {
                string inputAssetFile = Path.GetFileNameWithoutExtension(fileName);
                string inputAssetName = string.IsNullOrEmpty(assetName) ? inputAssetFile : string.Concat(assetName, " (", inputAssetFile, ")");
                string blobContainer = Constant.Storage.Blob.Container.FileUpload;
                Asset inputAsset = mediaClient.CreateAsset(storageAccount, inputAssetName, description, alternateId, blobContainer, fileName);
                inputAssets.Add(inputAsset);
            }
            return inputAssets.ToArray();
        }

        public JsonResult Create(string storageAccount, string assetName, string description, string alternateId, string[] fileNames,
                                 bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset, string streamingPolicyName)
        {
            Asset[] inputAssets;
            List<Job> jobs = new List<Job>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                inputAssets = CreateInputAssets(mediaClient, storageAccount, assetName, description, alternateId, fileNames);
                Transform transform = TransformController.CreateTransform(mediaClient, standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
                foreach (Asset inputAsset in inputAssets)
                {
                    Job job = null;
                    if (transform != null)
                    {
                        job = JobController.Create(mediaClient, transform.Name, null, null, Priority.Normal, inputAsset.Name, null, streamingPolicyName);
                        jobs.Add(job);
                    }
                    if ((videoAnalyzerPreset || videoAnalyzerPreset) && mediaClient.IndexerIsEnabled())
                    {
                        bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                        string indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, storageAccount, inputAsset, string.Empty, audioOnly);
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