using System;
using System.IO;
using System.Web;
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
                string inputFileUrl = HttpUtility.UrlDecode(mediaJob.InputFileUrl);
                outputAssetName = Path.GetFileNameWithoutExtension(inputFileUrl);
            }
            else
            {
                Asset asset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
                outputAssetStorage = asset.StorageAccountName;
                outputAssetName = mediaJob.InputAssetName;
            }
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            for (int i = 0; i < transform.Outputs.Count; i++)
            {
                TransformOutput transformOutput = transform.Outputs[i];
                string assetName = outputAssetName;
                if (!mediaJob.OutputAssetFilesMerge)
                {
                    assetName = string.Concat(assetName, " (", i, ")");
                }
                string assetDescription = i > mediaJob.OutputAssetDescriptions.Length - 1 ? null : mediaJob.OutputAssetDescriptions[i];
                string assetAlternateId = i > mediaJob.OutputAssetAlternateIds.Length - 1 ? null : mediaJob.OutputAssetAlternateIds[i];
                CreateAsset(outputAssetStorage, assetName, assetDescription, assetAlternateId);
                JobOutputAsset outputAsset = new JobOutputAsset(assetName);
                outputAssets.Add(outputAsset);
            }
            Dictionary<string, string> jobData = null;
            if (mediaJob.Data != null)
            {
                jobData = mediaJob.Data.ToObject<Dictionary<string, string>>();
            }
            JobInput jobInput;
            if (!string.IsNullOrEmpty(mediaJob.InputFileUrl))
            {
                jobInput = new JobInputHttp()
                {
                    Files = new string[] { mediaJob.InputFileUrl }
                };
            }
            else
            {
                jobInput = new JobInputAsset()
                {
                    AssetName = mediaJob.InputAssetName
                };
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                CorrelationData = jobData,
                Input = jobInput,
                Outputs = outputAssets.ToArray()
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public Job CreateJob(string authToken, string transformName, string jobName, string jobDescription, Priority jobPriority,
                             JObject jobData, string inputAssetName, string inputFileUrl, bool outputAssetFilesMerge,
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
                OutputAssetFilesMerge = outputAssetFilesMerge,
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