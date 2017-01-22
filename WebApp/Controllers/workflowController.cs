using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.Services;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class workflowController : Controller
    {
        private MediaAssetInput MapInputAsset(IAsset asset)
        {
            MediaAssetInput inputAsset = new MediaAssetInput();
            inputAsset.AssetId = asset.Id;
            inputAsset.PrimaryFile = MediaClient.GetPrimaryFile(asset);
            return inputAsset;
        }

        private MediaAssetInput[] MapInputAssets(MediaClient mediaClient, MediaAssetInput[] assets)
        {
            List<MediaAssetInput> inputAssets = new List<MediaAssetInput>();
            foreach (MediaAssetInput asset in assets)
            {
                IAsset mediaAsset = mediaClient.GetEntityById(MediaEntity.Asset, asset.AssetId) as IAsset;
                MediaAssetInput inputAsset = MapInputAsset(mediaAsset);
                inputAsset.MarkIn = asset.MarkIn;
                inputAsset.MarkOut = asset.MarkOut;
                inputAssets.Add(inputAsset);
            }
            return inputAssets.ToArray();
        }

        private MediaAssetInput[] CreateInputAssets(string authToken, MediaClient mediaClient, string storageAccount, bool storageEncryption,
                                                    string inputAssetName, bool multipleFileAsset, bool publishInputAsset, string[] fileNames)
        {
            List<MediaAssetInput> inputAssets = new List<MediaAssetInput>();
            if (multipleFileAsset)
            {
                IAsset asset = mediaClient.CreateAsset(authToken, inputAssetName, storageAccount, storageEncryption, fileNames);
                if (publishInputAsset)
                {
                    MediaClient.PublishContent(mediaClient, asset);
                }
                MediaAssetInput inputAsset = MapInputAsset(asset);
                inputAssets.Add(inputAsset);
            }
            else
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    string fileName = fileNames[i];
                    string assetName = Path.GetFileNameWithoutExtension(fileName);
                    IAsset asset = mediaClient.CreateAsset(authToken, assetName, storageAccount, storageEncryption, new string[] { fileName });
                    if (publishInputAsset)
                    {
                        MediaClient.PublishContent(mediaClient, asset);
                    }
                    MediaAssetInput inputAsset = MapInputAsset(asset);
                    inputAssets.Add(inputAsset);
                }
            }
            return inputAssets.ToArray();
        }

        private void SetInputClips(MediaClient mediaClient, MediaAssetInput[] inputAssets)
        {
            for (int i = 0; i < inputAssets.Length; i++)
            {
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, inputAssets[i].AssetId) as IAsset;
                inputAssets[i].PrimaryFile = MediaClient.GetPrimaryFile(asset);
                if (!string.IsNullOrEmpty(inputAssets[i].MarkIn) && !string.IsNullOrEmpty(inputAssets[i].MarkOut))
                {
                    int markIn = Convert.ToInt32(inputAssets[i].MarkIn);
                    int markOut = Convert.ToInt32(inputAssets[i].MarkOut);
                    int clipDuration = markOut - markIn;
                    TimeSpan markInTime = new TimeSpan(0, 0, markIn);
                    TimeSpan clipDurationTime = new TimeSpan(0, 0, clipDuration);
                    inputAssets[i].MarkIn = markInTime.ToString(Constants.FormatTime);
                    inputAssets[i].ClipDuration = clipDurationTime.ToString(Constants.FormatTime);
                }
            }
        }

        private ContentProtection[] GetContentProtections(MediaJobTask[] jobTasks)
        {
            List<ContentProtection> contentProtections = new List<ContentProtection>();
            foreach (MediaJobTask jobTask in jobTasks)
            {
                if (jobTask.ContentProtection != null)
                {
                    contentProtections.Add(jobTask.ContentProtection);
                }
            }
            return contentProtections.ToArray();
        }

        private IJob SubmitJob(string authToken, MediaClient mediaClient, string storageAccount, MediaAssetInput[] inputAssets,
                               string jobName, int jobPriority, MediaJobTask[] jobTasks)
        {
            IJob job = null;
            if (jobTasks != null && jobTasks.Length > 0)
            {
                MediaJob mediaJob = MediaClient.CreateJob(mediaClient, jobName, jobPriority, jobTasks, inputAssets);
                job = mediaClient.CreateJob(mediaJob);
                if (string.IsNullOrEmpty(storageAccount))
                {
                    storageAccount = job.InputMediaAssets[0].StorageAccountName;
                }
                ContentProtection[] contentProtections = GetContentProtections(jobTasks);
                MediaClient.TrackJob(authToken, job, storageAccount, contentProtections);
            }
            return job;
        }

        private JsonResult GetResult(MediaClient mediaClient, IJob job, MediaAssetInput[] inputAssets)
        {
            object result = job;
            if (job == null)
            {
                foreach (MediaAssetInput inputAsset in inputAssets)
                {
                    IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, inputAsset.AssetId) as IAsset;
                    inputAsset.AssetName = asset.Name;
                }
                result = inputAssets;
            }
            return Json(result);
        }

        public JsonResult upload(string[] fileNames, string storageAccount, bool storageEncryption, string inputAssetName,
                                 bool multipleFileAsset, bool publishInputAsset, MediaAssetInput[] inputAssets, string jobName,
                                 int jobPriority, MediaJobTask[] jobTasks)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            inputAssets = CreateInputAssets(authToken, mediaClient, storageAccount, storageEncryption, inputAssetName, multipleFileAsset, publishInputAsset, fileNames);
            IJob job = SubmitJob(authToken, mediaClient, storageAccount, inputAssets, jobName, jobPriority, jobTasks);
            return GetResult(mediaClient, job, inputAssets);
        }

        public JsonResult start(MediaAssetInput[] inputAssets, string jobName, int jobPriority, MediaJobTask[] jobTasks)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            inputAssets = MapInputAssets(mediaClient, inputAssets);
            SetInputClips(mediaClient, inputAssets);
            DatabaseClient databaseClient = new DatabaseClient();
            foreach (MediaJobTask jobTask in jobTasks)
            {
                if (!string.IsNullOrEmpty(jobTask.ProcessorDocumentId))
                {
                    JObject processorConfig = databaseClient.GetDocument(jobTask.ProcessorDocumentId);
                    jobTask.ProcessorConfig = processorConfig.ToString();
                }
            }
            IJob job = SubmitJob(authToken, mediaClient, null, inputAssets, jobName, jobPriority, jobTasks);
            return GetResult(mediaClient, job, inputAssets);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            uploadController.SetViewData(authToken, this.ViewData);
            return View();
        }
    }
}
