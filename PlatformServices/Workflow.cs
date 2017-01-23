using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Workflow
    {
        private static MediaAssetInput MapInputAsset(IAsset asset)
        {
            MediaAssetInput inputAsset = new MediaAssetInput();
            inputAsset.AssetId = asset.Id;
            inputAsset.PrimaryFile = MediaClient.GetPrimaryFile(asset);
            return inputAsset;
        }

        private static ContentProtection[] GetContentProtections(MediaJobTask[] jobTasks)
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

        private static object GetWorkflowResult(MediaClient mediaClient, IJob job, MediaAssetInput[] inputAssets)
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
            return result;
        }

        public static void SetInputClips(MediaClient mediaClient, MediaAssetInput[] inputAssets)
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

        public static MediaAssetInput[] CreateInputAssets(string authToken, MediaClient mediaClient, string storageAccount, bool storageEncryption,
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

        public static MediaAssetInput[] MapInputAssets(MediaClient mediaClient, MediaAssetInput[] assets)
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

        public static object SubmitJob(string authToken, MediaClient mediaClient, string storageAccount, MediaAssetInput[] inputAssets, MediaJob mediaJob)
        {
            IJob job = null;
            if (mediaJob.Tasks != null && mediaJob.Tasks.Length > 0)
            {
                mediaJob = MediaClient.SetJob(mediaClient, mediaJob, inputAssets);
                job = mediaClient.CreateJob(mediaJob);
                if (string.IsNullOrEmpty(storageAccount))
                {
                    storageAccount = job.InputMediaAssets[0].StorageAccountName;
                }
                ContentProtection[] contentProtections = GetContentProtections(mediaJob.Tasks);
                MediaClient.TrackJob(authToken, job, storageAccount, contentProtections);
            }
            return GetWorkflowResult(mediaClient, job, inputAssets);
        }
    }
}
