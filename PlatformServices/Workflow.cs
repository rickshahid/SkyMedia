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
                AssetType = asset.AssetType.ToString()
            };
            return jobInput;
        }

        private static void TrackJob(string directoryId, string authToken, IJob job, MediaJobTask[] jobTasks)
        {
            string storageAccountName = job.InputMediaAssets[0].StorageAccountName;
            string storageAccountKey = Storage.GetAccountKey(authToken, storageAccountName);

            User authUser = new User(authToken);
            MediaPublish contentPublish = new MediaPublish()
            {
                UserId = authUser.Id,
                PartitionKey = authUser.MediaAccount.Id,
                RowKey = job.Id,
                MediaAccount = authUser.MediaAccount,
                StorageAccountName = storageAccountName,
                StorageAccountKey = storageAccountKey,
                MobileNumber = authUser.MobileNumber
            };

            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.Table.ContentPublish;
            tableClient.InsertEntity(tableName, contentPublish);

            ContentProtection[] contentProtections = MediaClient.GetContentProtections(directoryId, job, jobTasks);
            foreach (ContentProtection contentProtection in contentProtections)
            {
                tableName = Constant.Storage.Table.ContentProtection;
                contentProtection.PartitionKey = contentPublish.PartitionKey;
                tableClient.InsertEntity(tableName, contentProtection);
            }
        }

        public static MediaJobInput[] GetJobInputs(string authToken, MediaClient mediaClient, string storageAccount, bool storageEncryption,
                                                   string inputAssetName, bool multipleFileAsset, string[] fileNames)
        {
            List<MediaJobInput> jobInputs = new List<MediaJobInput>();
            if (multipleFileAsset)
            {
                string assetName = inputAssetName;
                if (string.IsNullOrEmpty(assetName))
                {
                    assetName = Path.GetFileNameWithoutExtension(fileNames[0]);
                }
                IAsset asset = mediaClient.CreateAsset(authToken, assetName, storageAccount, storageEncryption, fileNames);
                MediaJobInput jobInput = GetJobInput(asset);
                jobInputs.Add(jobInput);
            }
            else
            {
                foreach (string fileName in fileNames)
                {
                    string assetName = inputAssetName;
                    if (string.IsNullOrEmpty(assetName))
                    {
                        assetName = fileName;
                    }
                    IAsset asset = mediaClient.CreateAsset(authToken, assetName, storageAccount, storageEncryption, new string[] { fileName });
                    MediaJobInput jobInput = GetJobInput(asset);
                    jobInputs.Add(jobInput);
                }
            }
            return jobInputs.ToArray();
        }

        public static MediaJobInput[] GetJobInputs(MediaClient mediaClient, string[] assetIds)
        {
            List<MediaJobInput> jobInputs = new List<MediaJobInput>();
            foreach (string assetId in assetIds)
            {
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                if (asset != null)
                {
                    MediaJobInput jobInput = GetJobInput(asset);
                    jobInputs.Add(jobInput);
                }
            }
            return jobInputs.ToArray();
        }

        public static object SubmitJob(string directoryId, string authToken, MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        {
            mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs);
            IJob job = mediaClient.CreateJob(mediaJob, jobInputs);
            TrackJob(directoryId, authToken, job, mediaJob.Tasks);
            return job;
        }

        public static string SubmitJob(MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        {
            string directoryId = Constant.DirectoryService.B2C;
            IJob job = SubmitJob(directoryId, null, mediaClient, mediaJob, jobInputs) as IJob;
            return job == null ? string.Empty : job.Id;
        }
    }
}