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
        private string GetAssetNameSuffix(TransformOutput transformOutput)
        {
            string assetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.Default;
            if (transformOutput.Preset is BuiltInStandardEncoderPreset)
            {
                assetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.AdaptiveStreaming;
            }
            else if (transformOutput.Preset is VideoAnalyzerPreset)
            {
                assetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.VideoAnalyzer;
            }
            else if (transformOutput.Preset is AudioAnalyzerPreset)
            {
                assetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.AudioAnalyzer;
            }
            return string.Concat(" (", assetNameSuffix, ")");
        }

        private JobInput GetJobInput(MediaJob mediaJob)
        {
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
            return jobInput;
        }

        private IList<JobOutput> GetJobOutputs(string transformName, MediaJob mediaJob, string outputAssetStorage, string outputAssetName)
        {
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            for (int i = 0; i < transform.Outputs.Count; i++)
            {
                TransformOutput transformOutput = transform.Outputs[i];
                string assetName = outputAssetName;
                if (!mediaJob.OutputAssetFilesMerge)
                {
                    string assetNameSuffix = GetAssetNameSuffix(transformOutput);
                    assetName = string.Concat(assetName, assetNameSuffix);
                }
                string assetDescription = i > mediaJob.OutputAssetDescriptions.Length - 1 ? null : mediaJob.OutputAssetDescriptions[i];
                string assetAlternateId = i > mediaJob.OutputAssetAlternateIds.Length - 1 ? null : mediaJob.OutputAssetAlternateIds[i];
                CreateAsset(outputAssetStorage, assetName, assetDescription, assetAlternateId);
                JobOutputAsset outputAsset = new JobOutputAsset(assetName);
                outputAssets.Add(outputAsset);
            }
            return outputAssets.ToArray();
        }

        private Job CreateJob(string transformName, MediaJob mediaJob)
        {
            if (string.IsNullOrEmpty(mediaJob.Name))
            {
                mediaJob.Name = Guid.NewGuid().ToString();
            }
            string outputAssetStorage = this.PrimaryStorageAccount;
            string outputAssetName = mediaJob.InputAssetName;
            if (!string.IsNullOrEmpty(mediaJob.InputFileUrl) && string.IsNullOrEmpty(outputAssetName))
            {
                Uri inputFileUri = new Uri(mediaJob.InputFileUrl);
                outputAssetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            }
            else if (!string.IsNullOrEmpty(mediaJob.InputAssetName))
            {
                Asset inputAsset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
                outputAssetStorage = inputAsset.StorageAccountName;
            }
            Dictionary<string, string> jobData = null;
            if (mediaJob.Data != null)
            {
                jobData = new Dictionary<string, string>();
                foreach (KeyValuePair<string, JToken> property in mediaJob.Data)
                {
                    string propertyData = property.Value.ToString();
                    jobData.Add(property.Key, propertyData);
                }
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                CorrelationData = jobData,
                Input = GetJobInput(mediaJob),
                Outputs = GetJobOutputs(transformName, mediaJob, outputAssetStorage, outputAssetName)
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public Job CreateJob(string authToken, string transformName, string jobName, string jobDescription, Priority jobPriority,
                             JObject jobData, string inputFileUrl, string inputAssetName, bool outputAssetFilesMerge,
                             string[] outputAssetDescriptions, string[] outputAssetAlternateIds, string streamingPolicyName)
        {
            SetJobPublish(authToken, jobData, streamingPolicyName);
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                Data = jobData,
                InputFileUrl = inputFileUrl,
                InputAssetName = inputAssetName,
                OutputAssetFilesMerge = outputAssetFilesMerge,
                OutputAssetDescriptions = outputAssetDescriptions,
                OutputAssetAlternateIds = outputAssetAlternateIds
            };
            Job job = CreateJob(transformName, mediaJob);
            MediaJobPublish jobPublish = new MediaJobPublish()
            {
                JobName = job.Name,
                TransformName = transformName,
                MediaAccount = MediaAccount,
            };
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.MediaJob;
                databaseClient.UpsertDocument(collectionId, jobPublish);
            }
            return job;
        }

        public void CancelJob(string transformName, string jobName)
        {
            _media.Jobs.CancelJob(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName);
        }
    }
}