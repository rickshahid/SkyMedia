using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Workflow
    {
        private static MediaAssetInput GetInputAsset(IAsset asset)
        {
            MediaAssetInput inputAsset = new MediaAssetInput();
            inputAsset.AssetId = asset.Id;
            inputAsset.AssetName = asset.Name;
            inputAsset.PrimaryFile = MediaClient.GetPrimaryFile(asset);
            return inputAsset;
        }

        private static MediaAssetInput GetInputAsset(MediaClient mediaClient, string assetId)
        {
            IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
            return GetInputAsset(asset);
        }

        private static ContentProtection GetContentProtection(MediaJobTask[] jobTasks)
        {
            ContentProtection contentProtection = null;
            foreach (MediaJobTask jobTask in jobTasks)
            {
                if (jobTask.ContentProtection != null)
                {
                    contentProtection = jobTask.ContentProtection;
                }
            }
            return contentProtection;
        }

        internal static void TrackJob(string authToken, IJob job, string storageAccount, ContentProtection contentProtection)
        {
            string attributeName = Constant.UserAttribute.MediaAccountName;
            string accountName = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.MediaAccountKey;
            string accountKey = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.UserId;
            string userId = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.MobileNumber;
            string mobileNumber = AuthToken.GetClaimValue(authToken, attributeName);

            if (string.IsNullOrEmpty(storageAccount))
            {
                storageAccount = job.InputMediaAssets[0].StorageAccountName;
            }

            JobPublish jobPublish = new JobPublish();
            jobPublish.PartitionKey = accountName;
            jobPublish.RowKey = job.Id;
            jobPublish.MediaAccountKey = accountKey;
            jobPublish.StorageAccountName = storageAccount;
            jobPublish.StorageAccountKey = Storage.GetUserAccountKey(authToken, storageAccount);
            jobPublish.UserId = userId;
            jobPublish.MobileNumber = mobileNumber;

            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.JobPublish;
            entityClient.InsertEntity(tableName, jobPublish);

            if (contentProtection != null)
            {
                tableName = Constant.Storage.TableName.ContentProtection;
                contentProtection.PartitionKey = jobPublish.PartitionKey;
                contentProtection.RowKey = jobPublish.RowKey;
                entityClient.InsertEntity(tableName, contentProtection);
            }
        }

        internal static object GetJobResult(MediaClient mediaClient, IJob job, IJobTemplate jobTemplate, MediaAssetInput[] inputAssets)
        {
            object result = job;
            if (job == null || string.IsNullOrEmpty(job.Id))
            {
                if (jobTemplate != null)
                {
                    result = jobTemplate;
                }
                else if (inputAssets != null)
                {
                    foreach (MediaAssetInput inputAsset in inputAssets)
                    {
                        IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, inputAsset.AssetId) as IAsset;
                        inputAsset.AssetName = asset.Name;
                    }
                    result = inputAssets;
                }
            }
            return result;
        }

        public static MediaAssetInput[] GetInputAssets(MediaClient mediaClient, MediaAssetInput[] inputAssets)
        {
            List<MediaAssetInput> assets = new List<MediaAssetInput>();
            foreach (MediaAssetInput inputAsset in inputAssets)
            {
                MediaAssetInput asset = GetInputAsset(mediaClient, inputAsset.AssetId);
                asset.MarkInSeconds = inputAsset.MarkInSeconds;
                asset.MarkOutSeconds = inputAsset.MarkOutSeconds;
                if (asset.MarkInSeconds > 0)
                {
                    TimeSpan markIn = new TimeSpan(0, 0, asset.MarkInSeconds);
                    TimeSpan markOut = new TimeSpan(0, 0, asset.MarkOutSeconds - asset.MarkInSeconds);
                    asset.MarkInTime = markIn.ToString(Constant.TextFormatter.ClockTime);
                    asset.MarkOutTime = markOut.ToString(Constant.TextFormatter.ClockTime);
                }
                assets.Add(asset);
            }
            return assets.ToArray();
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
                    MediaClient.PublishContent(mediaClient, asset, null);
                }
                MediaAssetInput inputAsset = GetInputAsset(asset);
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
                        MediaClient.PublishContent(mediaClient, asset, null);
                    }
                    MediaAssetInput inputAsset = GetInputAsset(asset);
                    inputAssets.Add(inputAsset);
                }
            }
            return inputAssets.ToArray();
        }

        public static object SubmitJob(string authToken, MediaClient mediaClient, string storageAccount, MediaAssetInput[] inputAssets, MediaJob mediaJob)
        {
            IJob job = null;
            IJobTemplate jobTemplate = null;
            ContentProtection contentProtection = null;
            if (mediaJob.Tasks != null)
            {
                mediaJob = MediaClient.GetJob(mediaClient, mediaJob, inputAssets);
                contentProtection = GetContentProtection(mediaJob.Tasks);
                job = mediaClient.CreateJob(mediaJob, inputAssets, out jobTemplate);
            }
            else if (!string.IsNullOrEmpty(mediaJob.TemplateId) || mediaJob.SaveWorkflow)
            {
                job = mediaClient.CreateJob(mediaJob, inputAssets, out jobTemplate);
            }
            if (job != null && !string.IsNullOrEmpty(job.Id))
            {
                TrackJob(authToken, job, storageAccount, contentProtection);
            }
            return GetJobResult(mediaClient, job, jobTemplate, inputAssets);
        }
    }
}
