using System.IO;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        private static string GetAssetName(string assetName, string fileName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = Path.GetFileNameWithoutExtension(fileName);
            }
            return assetName;
        }

        private static Asset[] CreateInputAssets(string authToken, MediaClient mediaClient, string storageAccount, string assetName,
                                                 string description, string alternateId, string[] fileNames)
        {
            List<Asset> assets = new List<Asset>();
            string blobContainer = Constant.Storage.Blob.Container.FileUpload;
            //if (multipleFileAsset)
            //{
            //    assetName = GetAssetName(assetName, fileNames[0]);
            //    Asset asset = mediaClient.CreateAsset(storageAccount, assetName, description, alternateId, blobContainer, fileNames);
            //    assets.Add(asset);
            //}
            //else
            //{
            foreach (string fileName in fileNames)
            {
                assetName = GetAssetName(assetName, fileName);
                Asset asset = mediaClient.CreateAsset(storageAccount, assetName, description, alternateId, blobContainer, new string[] { fileName });
                assets.Add(asset);
            }
            //}
            return assets.ToArray();
        }

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
                                 bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
            Asset[] assets;
            List<Job> jobs = new List<Job>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                assets = CreateInputAssets(authToken, mediaClient, storageAccount, assetName, description, alternateId, fileNames);
                Transform transform = TransformController.CreateTransform(mediaClient, standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
                if (transform != null)
                {
                    MediaPublish mediaPublish = new MediaPublish();
                    foreach (Asset asset in assets)
                    {
                        if (mediaClient.IndexerIsEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
                        {
                            bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                            mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, asset, string.Empty, audioOnly);
                        }
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
            string authToken = HomeController.GetAuthToken(Request, Response);
            MediaStream[] streams = Media.GetClipperStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
            return Json(streams);
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