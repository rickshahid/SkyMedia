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

        private string[] CreateOutputAssets(MediaClient mediaClient, string storageAccount, string assetName,
                                            bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
            List<string> outputAssetNames = new List<string>();
            if (standardEncoderPreset)
            {
                string outputAssetName = string.Concat(assetName, Constant.Media.Job.OutputAssetSuffixEncoderStandard);
                Asset outputAsset = mediaClient.CreateAsset(storageAccount, outputAssetName);
                outputAssetNames.Add(outputAssetName);
            }
            if (videoAnalyzerPreset)
            {
                string outputAssetName = string.Concat(assetName, Constant.Media.Job.OutputAssetSuffixAnalyzerVideo);
                Asset outputAsset = mediaClient.CreateAsset(storageAccount, outputAssetName);
                outputAssetNames.Add(outputAssetName);
            }
            if (audioAnalyzerPreset)
            {
                string outputAssetName = string.Concat(assetName, Constant.Media.Job.OutputAssetSuffixAnalyzerAudio);
                Asset outputAsset = mediaClient.CreateAsset(storageAccount, outputAssetName);
                outputAssetNames.Add(outputAssetName);
            }
            return outputAssetNames.ToArray();
        }

        public JsonResult Create(string storageAccount, string assetName, string description, string alternateId, string[] fileNames,
                                 bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
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
                    if (transform != null)
                    {
                        string[] outputAssetNames = CreateOutputAssets(mediaClient, storageAccount, inputAsset.Name, standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
                        Job job = JobController.Create(mediaClient, transform.Name, inputAsset.Name, outputAssetNames);
                        jobs.Add(job);
                    }
                    if (mediaClient.IndexerIsEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
                    {
                        bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                        string indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, storageAccount, inputAsset, string.Empty, audioOnly);
                        inputAsset.AlternateId = indexId;
                    }
                }
            }
            return jobs.Count > 0 ? Json(jobs.ToArray()) : Json(inputAssets);
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