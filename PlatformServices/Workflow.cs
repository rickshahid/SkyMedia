using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Workflow
    {
        private static string GetAssetName(string assetName, string fileName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = Path.GetFileNameWithoutExtension(fileName);
            }
            return assetName;
        }

        public static Asset[] CreateAssets(string authToken, MediaClient mediaClient, string storageAccount, string assetName,
                                           string description, string alternateId, bool multipleFileAsset, string[] fileNames)
        {
            List<Asset> assets = new List<Asset>();
            string blobContainer = Constant.Storage.Blob.Container.FileUpload;
            if (multipleFileAsset)
            {
                assetName = GetAssetName(assetName, fileNames[0]);
                Asset asset = mediaClient.CreateAsset(storageAccount, assetName, description, alternateId, blobContainer, fileNames);
                assets.Add(asset);
            }
            else
            {
                foreach (string fileName in fileNames)
                {
                    assetName = GetAssetName(assetName, fileName);
                    Asset asset = mediaClient.CreateAsset(storageAccount, assetName, description, alternateId, blobContainer, new string[] { fileName });
                    assets.Add(asset);
                }
            }
            return assets.ToArray();
        }

        //public static MediaJobInput[] GetJobInputs(MediaClient mediaClient, string[] assetIds)
        //{
        //    List<MediaJobInput> jobInputs = new List<MediaJobInput>();
        //    foreach (string assetId in assetIds)
        //    {
        //        IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
        //        if (asset != null)
        //        {
        //            MediaJobInput jobInput = MediaClient.GetJobInput(asset);
        //            jobInputs.Add(jobInput);
        //        }
        //    }
        //    return jobInputs.ToArray();
        //}

        //public static object SubmitJob(string directoryId, string authToken, MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        //{
        //    mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs);
        //    IJob job = mediaClient.CreateJob(mediaJob, jobInputs);
        //    TrackJob(directoryId, authToken, mediaClient.MediaAccount, job, mediaJob.Tasks);
        //    return job;
        //}

        //public static string SubmitJob(MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        //{
        //    string directoryId = Constant.DirectoryService.B2C;
        //    IJob job = SubmitJob(directoryId, null, mediaClient, mediaJob, jobInputs) as IJob;
        //    return job.Id;
        //}

        //private static MediaInsightConfig GetInsightConfig(MediaJobTask[] jobTasks)
        //{
        //    MediaInsightConfig insightConfig = null;
        //    foreach (MediaJobTask jobTask in jobTasks)
        //    {
        //        if (jobTask.InsightConfig != null)
        //        {
        //            insightConfig = jobTask.InsightConfig;
        //        }
        //    }
        //    return insightConfig;
        //}

        //private static void TrackJob(string directoryId, string authToken, MediaAccount mediaAccount, IJob job, MediaJobTask[] jobTasks)
        //{
        //    List<string> taskIds = new List<string>();
        //    foreach (ITask task in job.Tasks)
        //    {
        //        taskIds.Add(task.Id);
        //    }

        //    string mobileNumber = string.Empty;
        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        User authUser = new User(authToken);
        //        mobileNumber = authUser.MobileNumber;
        //    }

        //    MediaInsightConfig insightConfig = GetInsightConfig(jobTasks);
        //    MediaPublish contentPublish = new MediaPublish()
        //    {
        //        Id = job.Id,
        //        TaskIds = taskIds.ToArray(),
        //        InsightConfig = insightConfig,
        //        MediaAccount = mediaAccount,
        //        MobileNumber = mobileNumber
        //    };

        //    DatabaseClient databaseClient = new DatabaseClient();
        //    string collectionId = Constant.Database.Collection.OutputPublish;
        //    databaseClient.UpsertDocument(collectionId, contentPublish);

        //    ContentProtection[] jobProtection = MediaClient.GetJobProtection(directoryId, job, jobTasks);
        //    foreach (ContentProtection contentProtection in jobProtection)
        //    {
        //        databaseClient.UpsertDocument(collectionId, contentProtection);
        //    }
        //}
    }
}