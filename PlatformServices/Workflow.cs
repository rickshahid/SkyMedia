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

        private static object GetJobOutput(IJob job, IJobTemplate jobTemplate, MediaJobInput[] jobInputs)
        {
            object jobOutput = job;
            if (jobTemplate != null)
            {
                jobOutput = jobTemplate;
            }
            else if (jobOutput == null)
            {
                jobOutput = jobInputs;
            }
            return jobOutput;
        }

        private static void TrackJob(string directoryId, string authToken, IJob job, MediaJobTask[] jobTasks)
        {
            User authUser = new User(authToken);

            string storageAccountName = job.InputMediaAssets[0].StorageAccountName;
            string storageAccountKey = Storage.GetAccountKey(authToken, storageAccountName);

            MediaPublish contentPublish = new MediaPublish()
            {
                PartitionKey = authUser.MediaAccount.Id,
                RowKey = job.Id,
                MediaAccount = authUser.MediaAccount,
                StorageAccountName = storageAccountName,
                StorageAccountKey = storageAccountKey,
                UserId = authUser.Id,
                MobileNumber = authUser.MobileNumber
            };

            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.Table.ContentPublish;
            tableClient.InsertEntity(tableName, contentPublish);

            ContentProtection[] contentProtections = MediaClient.GetContentProtections(directoryId, job, jobTasks);
            foreach (ContentProtection contentProtection in contentProtections)
            {
                tableName = Constant.Storage.Table.ContentProtection;
                contentProtection.PartitionKey = contentPublish.RowKey;
                tableClient.InsertEntity(tableName, contentProtection);
            }
        }

        public static MediaJobInput[] GetJobInputs(string authToken, MediaClient mediaClient, string storageAccount, bool storageEncryption,
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
                foreach (string fileName in fileNames)
                {
                    string assetName = fileName;
                    if (fileNames.Length == 1 && !string.IsNullOrEmpty(inputAssetName))
                    {
                        assetName = inputAssetName;
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
            IJob job = null;
            IJobTemplate jobTemplate = null;
            if (mediaJob.Tasks != null)
            {
                mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs);
            }
            if (mediaJob.Tasks != null || !string.IsNullOrEmpty(mediaJob.TemplateId))
            {
                job = mediaClient.CreateJob(mediaJob, jobInputs, out jobTemplate);
            }
            if (job != null && !string.IsNullOrEmpty(job.Id))
            {
                TrackJob(directoryId, authToken, job, mediaJob.Tasks);
            }
            return GetJobOutput(job, jobTemplate, jobInputs);
        }
    }
}