using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetOutputAssetNameSuffix(MediaJobOutputMode jobOutputMode, IList<TransformOutput> transformOutputs, int i)
        {
            string outputAssetNameSuffix = null;
            if (jobOutputMode == MediaJobOutputMode.SingleAsset && transformOutputs.Count > 1)
            {
                outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.MediaServices;
            }
            else
            {
                Preset transformPreset = transformOutputs[i].Preset;
                if (transformPreset is VideoAnalyzerPreset)
                {
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.VideoAnalyzer;
                }
                else if (transformPreset is AudioAnalyzerPreset)
                {
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.AudioAnalyzer;
                }
                else if (transformPreset is StandardEncoderPreset)
                {
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.StandardEncoder;
                }
                else if (transformPreset is BuiltInStandardEncoderPreset builtInStandardEncoderPreset)
                {
                    outputAssetNameSuffix = builtInStandardEncoderPreset.PresetName;
                }
            }
            return Constant.TextFormatter.FormatValue(outputAssetNameSuffix);
        }

        private string GetOutputAssetName(MediaJob mediaJob, IList<TransformOutput> transformOutputs, int i)
        {
            string outputAssetName = mediaJob.InputAssetName;
            if (!string.IsNullOrEmpty(mediaJob.InputFileUrl) && string.IsNullOrEmpty(outputAssetName))
            {
                Uri inputFileUri = new Uri(mediaJob.InputFileUrl);
                outputAssetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            }
            else if (!string.IsNullOrEmpty(mediaJob.InputAssetName))
            {
                Asset inputAsset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
            }
            string outputAssetNameSuffix = GetOutputAssetNameSuffix(mediaJob.OutputAssetMode, transformOutputs, i);
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
                string outputAssetName = GetOutputAssetName(mediaJob, transform.Outputs, i);
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
                CreateAsset(mediaJob.OutputAssetStorage, outputAssetName, outputAssetDescription, outputAssetAlternateId);
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
                CorrelationData = GetCorrelationData(mediaJob.Data, true),
                Input = GetJobInput(mediaJob),
                Outputs = GetJobOutputs(transformName, insightId, mediaJob)
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public async Task<Job> CreateJob(string transformName, string jobName, string jobDescription, Priority jobPriority, string jobData,
                                         string inputFileUrl, string inputAssetName, MediaJobOutputMode outputAssetMode, string outputAssetStorage,
                                         string streamingPolicyName)
        {
            string insightId = null;
            await EventGridClient.SetMediaSubscription(this.MediaAccount);
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputFileUrl = inputFileUrl,
                InputAssetName = inputAssetName,
                OutputAssetMode = outputAssetMode,
                OutputAssetStorage = outputAssetStorage
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
            Job job = CreateJob(transformName, insightId, mediaJob);
            if (!string.IsNullOrEmpty(insightId))
            {
                job.CorrelationData.Add("insightId", insightId);
            }
            return job;
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