using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        private string[] CreateOutputAssets(MediaClient mediaClient, string storageAccount, string assetName, bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
            List<string> outputAssetNames = new List<string>();
            if (standardEncoderPreset)
            {
                string outputAssetName = string.Concat(assetName, Constant.Media.Job.OutputAssetSuffixEncoderStandard);
                Asset outputAsset = mediaClient.CreateAsset(storageAccount, outputAssetName, string.Empty, string.Empty);
                outputAssetNames.Add(outputAssetName);
            }
            if (videoAnalyzerPreset)
            {
                string outputAssetName = string.Concat(assetName, Constant.Media.Job.OutputAssetSuffixAnalyzerVideo);
                Asset outputAsset = mediaClient.CreateAsset(storageAccount, outputAssetName, string.Empty, string.Empty);
                outputAssetNames.Add(outputAssetName);
            }
            if (audioAnalyzerPreset)
            {
                string outputAssetName = string.Concat(assetName, Constant.Media.Job.OutputAssetSuffixAnalyzerAudio);
                Asset outputAsset = mediaClient.CreateAsset(storageAccount, outputAssetName, string.Empty, string.Empty);
                outputAssetNames.Add(outputAssetName);
            }
            return outputAssetNames.ToArray();
        }

        public JsonResult Create(string storageAccount, string assetName, string description, string alternateId, string[] fileNames,
                                 bool multipleFileAsset, bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
            Asset[] assets;
            List<Job> jobs = new List<Job>();
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                assets = Workflow.CreateAssets(authToken, mediaClient, storageAccount, assetName, description, alternateId, multipleFileAsset, fileNames);
                Transform transform = TransformController.CreateTransform(mediaClient, standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
                if (transform != null)
                {
                    MediaPublish mediaPublish = new MediaPublish();
                    foreach (Asset asset in assets)
                    {
                        JobInputAsset inputAsset = new JobInputAsset(asset.Name);
                        string[] outputAssetNames = CreateOutputAssets(mediaClient, storageAccount, asset.Name, standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);

                        Job job = JobController.Create(authToken, transform.Name, null, inputAsset, outputAssetNames, mediaPublish);
                        jobs.Add(job);
                    }
                }
            }
            return jobs.Count > 0 ? Json(jobs.ToArray()) : Json(assets);
        }

        public JsonResult Streams(string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            string authToken = HomeController.GetAuthToken(this.Request, this.Response);
            MediaStream[] streams = Media.GetMediaStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
            return Json(streams);
        }

        public IActionResult Edit()
        {
            return View();
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