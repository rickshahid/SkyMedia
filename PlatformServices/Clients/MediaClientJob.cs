using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

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
            string outputAssetStorage = null;
            string outputAssetName = null;
            if (!string.IsNullOrEmpty(mediaJob.InputFileUrl))
            {
                outputAssetStorage = this.PrimaryStorageAccount;
                outputAssetName = Path.GetFileNameWithoutExtension(mediaJob.InputFileUrl);
            }
            else if (!string.IsNullOrEmpty(mediaJob.InputAssetName))
            {
                Asset asset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
                MediaAsset mediaAsset = new MediaAsset(MediaAccount, asset);
                string fileName = mediaAsset.Files[0].Name;
                StorageBlobClient blobClient = new StorageBlobClient(MediaAccount, asset.StorageAccountName);
                mediaJob.InputFileUrl = blobClient.GetDownloadUrl(asset.Container, fileName, false);
                outputAssetStorage = asset.StorageAccountName;
                outputAssetName = mediaJob.InputAssetName;
            }
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            for (int i = 0; i < transform.Outputs.Count; i++)
            {
                TransformOutput transformOutput = transform.Outputs[i];
                string assetName = outputAssetName;
                if (mediaJob.OutputAssetSeparation)
                {
                    assetName = string.Concat(assetName, " (", i, ")");
                }
                string assetDescription = i > mediaJob.OutputAssetDescriptions.Length - 1 ? null : mediaJob.OutputAssetDescriptions[i];
                string assetAlternateId = i > mediaJob.OutputAssetAlternateIds.Length - 1 ? null : mediaJob.OutputAssetAlternateIds[i];
                CreateAsset(outputAssetStorage, assetName, assetDescription, assetAlternateId);
                JobOutputAsset outputAsset = new JobOutputAsset(outputAssetName);
                outputAssets.Add(outputAsset);

            }
            Dictionary<string, string> jobData = null;
            if (mediaJob.Data != null)
            {
                jobData = mediaJob.Data.ToObject<Dictionary<string, string>>();
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                CorrelationData = jobData,
                Input = new JobInputHttp()
                {
                    Files = new string[] { mediaJob.InputFileUrl }
                },
                Outputs = outputAssets.ToArray()
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public Job CreateJob(string authToken, string transformName, string jobName, string jobDescription, Priority jobPriority,
                             JObject jobData, string inputAssetName, string inputFileUrl, bool outputAssetSeparation,
                             string[] outputAssetDescriptions, string[] outputAssetAlternateIds, string streamingPolicyName)
        {
            string mobilePhoneNumber = null;
            if (!string.IsNullOrEmpty(authToken))
            {
                User userProfile = new User(authToken);
                mobilePhoneNumber = userProfile.MobilePhoneNumber;
            }
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                Data = jobData,
                InputAssetName = inputAssetName,
                InputFileUrl = inputFileUrl,
                OutputAssetSeparation = outputAssetSeparation,
                OutputAssetDescriptions = outputAssetDescriptions,
                OutputAssetAlternateIds = outputAssetAlternateIds
            };
            Job job = CreateJob(transformName, mediaJob);
            MediaPublish mediaPublish = new MediaPublish()
            {
                Id = job.Name,
                TransformName = transformName,
                StreamingPolicyName = streamingPolicyName,
                MediaAccount = MediaAccount,
                UserContact = new UserContact()
                {
                    MobilePhoneNumber = mobilePhoneNumber
                }
            };
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.MediaPublish;
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