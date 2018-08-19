using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private Job CreateJob(string transformName, MediaJob mediaJob)
        {
            if (string.IsNullOrEmpty(mediaJob.Name))
            {
                mediaJob.Name = Guid.NewGuid().ToString();
            }
            string outputAssetName = null;
            if (!string.IsNullOrEmpty(mediaJob.InputFileUrl))
            {
                outputAssetName = Path.GetFileNameWithoutExtension(mediaJob.InputFileUrl);
            }
            else if (!string.IsNullOrEmpty(mediaJob.InputAssetName))
            {
                outputAssetName = mediaJob.InputAssetName;
                Asset asset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
                MediaAsset mediaAsset = new MediaAsset(MediaAccount, asset);
                string fileName = mediaAsset.Files[0].Name;
                BlobClient blobClient = new BlobClient(MediaAccount, asset.StorageAccountName);
                mediaJob.InputFileUrl = blobClient.GetDownloadUrl(asset.Container, fileName, false);
            }
            CreateAsset(mediaJob.OutputAssetStorage, outputAssetName, mediaJob.OutputAssetDescription, mediaJob.OutputAssetAlternateId);
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            foreach (TransformOutput transformOutput in transform.Outputs)
            {
                JobOutputAsset outputAsset = new JobOutputAsset(outputAssetName);
                outputAssets.Add(outputAsset);
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                Input = new JobInputHttp()
                {
                    Files = new string[] { mediaJob.InputFileUrl }
                },
                Outputs = outputAssets.ToArray()
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public Job CreateJob(string transformName, string jobName, string jobDescription, Priority jobPriority, string inputAssetName, string inputFileUrl, string outputAssetStorage, string outputAssetDescription, string outputAssetAlternateId, string streamingPolicyName)
        {
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputAssetName = inputAssetName,
                InputFileUrl = inputFileUrl,
                OutputAssetStorage = outputAssetStorage,
                OutputAssetDescription = outputAssetDescription,
                OutputAssetAlternateId = outputAssetAlternateId
            };
            Job job = CreateJob(transformName, mediaJob);
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.ContentPublish;
                MediaPublish mediaPublish = new MediaPublish()
                {
                    Id = job.Name,
                    TransformName = transformName,
                    StreamingPolicyName = streamingPolicyName,
                    MediaAccount = MediaAccount,
                    UserAccount = UserAccount
                };
                databaseClient.UpsertDocument(collectionId, mediaPublish);
            }
            return job;
        }

        public void CancelJob(string transformName, string jobName)
        {
            _media.Jobs.CancelJob(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName);
        }
    }
}