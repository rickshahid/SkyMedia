using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Workflow
    {
        private static MediaJobInput GetJobInput(IAsset asset)
        {
            MediaJobInput jobInput = new MediaJobInput()
            {
                AssetId = asset.Id,
                AssetName = asset.Name,
                PrimaryFile = MediaClient.GetPrimaryFile(asset)
            };
            return jobInput;
        }

        private static MediaJobInput GetJobInput(MediaClient mediaClient, string assetId)
        {
            IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
            return GetJobInput(asset);
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

            MediaContentPublish contentPublish = new MediaContentPublish()
            {
                PartitionKey = accountName,
                RowKey = job.Id,
                MediaAccountKey = accountKey,
                StorageAccountName = storageAccount,
                StorageAccountKey = Storage.GetAccountKey(authToken, storageAccount),
                UserId = userId,
                MobileNumber = mobileNumber
            };

            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.ContentPublish;
            entityClient.InsertEntity(tableName, contentPublish);

            if (contentProtection != null)
            {
                tableName = Constant.Storage.TableName.ContentProtection;
                contentProtection.PartitionKey = contentPublish.PartitionKey;
                contentProtection.RowKey = contentPublish.RowKey;
                entityClient.InsertEntity(tableName, contentProtection);
            }
        }

        internal static object GetJobOutput(MediaClient mediaClient, IJob job, IJobTemplate jobTemplate, MediaJobInput[] jobInputs)
        {
            object output = job;
            if (job == null || string.IsNullOrEmpty(job.Id))
            {
                if (jobTemplate != null)
                {
                    output = jobTemplate;
                }
                else if (jobInputs != null)
                {
                    foreach (MediaJobInput jobInput in jobInputs)
                    {
                        IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, jobInput.AssetId) as IAsset;
                        jobInput.AssetName = asset.Name;
                    }
                    output = jobInputs;
                }
            }
            return output;
        }

        public static MediaJobInput[] GetJobInputs(MediaClient mediaClient, MediaJobInput[] jobInputs)
        {
            List<MediaJobInput> assets = new List<MediaJobInput>();
            foreach (MediaJobInput jobInput in jobInputs)
            {
                MediaJobInput asset = GetJobInput(mediaClient, jobInput.AssetId);
                asset.MarkInSeconds = jobInput.MarkInSeconds;
                asset.MarkOutSeconds = jobInput.MarkOutSeconds;
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

        public static MediaJobInput[] CreateJobInputs(string authToken, MediaClient mediaClient, string storageAccount, bool storageEncryption,
                                                      string inputAssetName, bool multipleFileAsset, string[] fileNames)
        {
            List<MediaJobInput> jobInputs = new List<MediaJobInput>();
            if (multipleFileAsset)
            {
                IAsset asset = mediaClient.CreateAsset(authToken, inputAssetName, storageAccount, storageEncryption, fileNames);
                MediaJobInput jobInput = GetJobInput(asset);
                jobInputs.Add(jobInput);
            }
            else
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    string fileName = fileNames[i];
                    string assetName = Path.GetFileNameWithoutExtension(fileName);
                    IAsset asset = mediaClient.CreateAsset(authToken, assetName, storageAccount, storageEncryption, new string[] { fileName });
                    MediaJobInput jobInput = GetJobInput(asset);
                    jobInputs.Add(jobInput);
                }
            }
            return jobInputs.ToArray();
        }

        public static object SubmitJob(string authToken, MediaClient mediaClient, string storageAccount, MediaJobInput[] jobInputs, MediaJob mediaJob)
        {
            IJob job = null;
            IJobTemplate jobTemplate = null;
            ContentProtection contentProtection = null;
            if (mediaJob.Tasks != null)
            {
                mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs);
                contentProtection = GetContentProtection(mediaJob.Tasks);
                job = mediaClient.CreateJob(mediaJob, jobInputs, out jobTemplate);
            }
            else if (!string.IsNullOrEmpty(mediaJob.TemplateId) || mediaJob.SaveWorkflow)
            {
                job = mediaClient.CreateJob(mediaJob, jobInputs, out jobTemplate);
            }
            if (job != null && !string.IsNullOrEmpty(job.Id))
            {
                TrackJob(authToken, job, storageAccount, contentProtection);
            }
            return GetJobOutput(mediaClient, job, jobTemplate, jobInputs);
        }
    }
}