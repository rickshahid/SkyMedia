using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Workflow
    {
        private static void TrackJob(string directoryId, string authToken, MediaAccount mediaAccount, IJob job, MediaJobTask[] jobTasks)
        {
            List<string> taskIds = new List<string>();
            foreach (ITask task in job.Tasks)
            {
                taskIds.Add(task.Id);
            }

            string mobileNumber = string.Empty;
            if (!string.IsNullOrEmpty(authToken))
            {
                User authUser = new User(authToken);
                mobileNumber = authUser.MobileNumber;
            }

            MediaPublish contentPublish = new MediaPublish()
            {
                Id = job.Id,
                TaskIds = taskIds.ToArray(),
                MediaAccount = mediaAccount,
                MobileNumber = mobileNumber
            };

            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constant.Database.Collection.MediaPublish;
            databaseClient.UpsertDocument(collectionId, contentPublish);

            ContentProtection[] jobProtection = MediaClient.GetJobProtection(directoryId, job, jobTasks);
            foreach (ContentProtection contentProtection in jobProtection)
            {
                databaseClient.UpsertDocument(collectionId, contentProtection);
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
                MediaJobInput jobInput = MediaClient.GetJobInput(asset);
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
                    MediaJobInput jobInput = MediaClient.GetJobInput(asset);
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
                    MediaJobInput jobInput = MediaClient.GetJobInput(asset);
                    jobInputs.Add(jobInput);
                }
            }
            return jobInputs.ToArray();
        }

        public static object SubmitJob(string directoryId, string authToken, MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        {
            mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs);
            IJob job = mediaClient.CreateJob(mediaJob, jobInputs);
            TrackJob(directoryId, authToken, mediaClient.MediaAccount, job, mediaJob.Tasks);
            return job;
        }

        public static string SubmitJob(MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        {
            string directoryId = Constant.DirectoryService.B2C;
            IJob job = SubmitJob(directoryId, null, mediaClient, mediaJob, jobInputs) as IJob;
            return job.Id;
        }
    }
}