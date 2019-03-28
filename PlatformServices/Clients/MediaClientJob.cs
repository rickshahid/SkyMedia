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
        private string GetOutputAssetNameSuffix(Preset transformPreset, MediaJobOutputMode jobOutputMode)
        {
            string outputAssetNameSuffix = null;
            if (transformPreset is BuiltInStandardEncoderPreset standardEncoderPreset &&
               (standardEncoderPreset.PresetName.ToString().Contains("Single") ||
                standardEncoderPreset.PresetName.ToString().Contains("Audio")))
            {
                outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.SingleBitrate;
            }
            switch (jobOutputMode)
            {
                case MediaJobOutputMode.InputAsset:
                    outputAssetNameSuffix = null;
                    break;
                case MediaJobOutputMode.OutputAsset:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.MultipleBitrate;
                    break;
                case MediaJobOutputMode.OutputAssets:
                    if (transformPreset is VideoAnalyzerPreset)
                    {
                        outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.VideoAnalyzer;
                    }
                    else if (transformPreset is AudioAnalyzerPreset)
                    {
                        outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.AudioAnalyzer;
                    }
                    break;
            }
            return outputAssetNameSuffix;
        }

        private string GetOutputAssetName(TransformOutput transformOutput, MediaJob mediaJob, out string outputAssetStorage)
        {
            outputAssetStorage = this.PrimaryStorageAccount;
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
            string outputAssetNameSuffix = GetOutputAssetNameSuffix(transformOutput.Preset, mediaJob.OutputAssetMode);
            if (!string.IsNullOrEmpty(outputAssetNameSuffix))
            {
                outputAssetName = string.Concat(outputAssetName, " (", outputAssetNameSuffix, ")");
            }
            return outputAssetName;
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

        private IList<JobOutput> GetJobOutputs(string transformName, string insightId, MediaJob mediaJob)
        {
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            for (int i = 0; i < transform.Outputs.Count; i++)
            {
                TransformOutput transformOutput = transform.Outputs[i];
                string outputAssetName = GetOutputAssetName(transformOutput, mediaJob, out string outputAssetStorage);
                string outputAssetDescription = null;
                if (!string.IsNullOrEmpty(mediaJob.InputAssetName))
                {
                    Asset inputAsset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
                    outputAssetDescription = inputAsset.Description;
                }
                string outputAssetAlternateId = null;
                if (transformOutput.Preset is BuiltInStandardEncoderPreset || transformOutput.Preset is StandardEncoderPreset)
                {
                    outputAssetAlternateId = insightId;
                }
                CreateAsset(outputAssetStorage, outputAssetName, outputAssetDescription, outputAssetAlternateId);
                JobOutputAsset outputAsset = new JobOutputAsset(outputAssetName);
                outputAssets.Add(outputAsset);
            }
            return outputAssets.ToArray();
        }

        private Job CreateJob(string transformName, string insightId, MediaJob mediaJob)
        {
            if (string.IsNullOrEmpty(mediaJob.Name))
            {
                mediaJob.Name = Guid.NewGuid().ToString();
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                CorrelationData = GetCorrelationData(mediaJob.Data),
                Input = GetJobInput(mediaJob),
                Outputs = GetJobOutputs(transformName, insightId, mediaJob)
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public Job CreateJob(string transformName, string jobName, string jobDescription, Priority jobPriority, string jobData,
                             string inputFileUrl, string inputAssetName, MediaJobOutputMode outputAssetMode, string streamingPolicyName)
        {
            string insightId = null;
            EventGridClient.SetMediaSubscription(this.MediaAccount, null);
            if (transformName.Contains("Indexer"))
            {
                insightId = IndexerUploadVideo(inputFileUrl, inputAssetName, jobPriority, false, false, false);
            }
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputFileUrl = inputFileUrl,
                InputAssetName = inputAssetName,
                OutputAssetMode = outputAssetMode
            };
            mediaJob.Data = new JObject
            {
                { "streamingPolicyName", streamingPolicyName },
                { "contentProtection", JObject.FromObject(new ContentProtection()) }
            };
            if (!string.IsNullOrEmpty(jobData))
            {
                mediaJob.Data.Add(jobData);
            }
            return CreateJob(transformName, insightId, mediaJob);
        }

        public Job UpdateJob(string transformName, string jobName, string jobDescription, Priority jobPriority)
        {
            Job job = new Job()
            {
                Description = jobDescription,
                Priority = jobPriority
            };
            return _media.Jobs.Update(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName, job);
        }

        public void CancelJob(string transformName, string jobName)
        {
            _media.Jobs.CancelJob(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName);
        }
    }
}