using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetOutputAssetNameSuffix(Preset transformPreset)
        {
            string outputAssetNameSuffix = null;
            if (transformPreset is VideoAnalyzerPreset)
            {
                outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.VideoAnalyzer;
            }
            else if (transformPreset is AudioAnalyzerPreset)
            {
                outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.AudioAnalyzer;
            }
            else if (transformPreset is FaceDetectorPreset)
            {
                outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.FaceDetector;
            }
            else if (transformPreset is StandardEncoderPreset)
            {
                outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.StandardEncoder;
            }
            else if (transformPreset is BuiltInStandardEncoderPreset builtInStandardEncoderPreset)
            {
                outputAssetNameSuffix = builtInStandardEncoderPreset.PresetName;
            }
            return Constant.TextFormatter.FormatValue(outputAssetNameSuffix);
        }

        private string GetOutputAssetName(MediaJob mediaJob, TransformOutput transformOutput)
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
                outputAssetName = inputAsset.Name;
            }
            string outputAssetNameSuffix = GetOutputAssetNameSuffix(transformOutput.Preset);
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

        private IList<JobOutput> GetJobOutputs(string transformName, MediaJob mediaJob)
        {
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            foreach (TransformOutput transformOutput in transform.Outputs)
            {
                string outputAssetName = GetOutputAssetName(mediaJob, transformOutput);
                string outputAssetDescription = null;
                if (!string.IsNullOrEmpty(mediaJob.InputAssetName))
                {
                    Asset inputAsset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
                    outputAssetDescription = inputAsset.Description;
                }
                CreateAsset(mediaJob.OutputAssetStorage, outputAssetName, outputAssetDescription, string.Empty);
                JobOutputAsset outputAsset = new JobOutputAsset(outputAssetName);
                outputAssets.Add(outputAsset);
            }
            return outputAssets.ToArray();
        }

        private IDictionary<string, string> GetCorrelationData(MediaJobOutputPublish jobOutputPublish)
        {
            Dictionary<string, string> correlationData = new Dictionary<string, string>();
            string userAccount = JsonConvert.SerializeObject(this.UserAccount);
            string mediaAccount = JsonConvert.SerializeObject(this.MediaAccount);
            correlationData.Add(Constant.Media.Job.CorrelationData.UserAccount, userAccount);
            correlationData.Add(Constant.Media.Job.CorrelationData.MediaAccount, mediaAccount);
            correlationData.Add(Constant.Media.Job.CorrelationData.OutputPublish, JsonConvert.SerializeObject(jobOutputPublish));
            return correlationData;
        }

        private Job CreateJob(string transformName, MediaJob mediaJob)
        {
            if (string.IsNullOrEmpty(mediaJob.Name))
            {
                mediaJob.Name = Guid.NewGuid().ToString();
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                Input = GetJobInput(mediaJob),
                Outputs = GetJobOutputs(transformName, mediaJob),
                CorrelationData = GetCorrelationData(mediaJob.OutputAssetPublish),
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public Job CreateJob(string transformName, string jobName, string jobDescription, Priority jobPriority, string inputFileUrl,
                             string inputAssetName, string outputAssetStorage, MediaJobOutputPublish outputAssetPublish)
        {
            EventGridClient.SetMediaSubscription(this.MediaAccount);
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputFileUrl = inputFileUrl,
                InputAssetName = inputAssetName,
                OutputAssetStorage = outputAssetStorage,
                OutputAssetPublish = outputAssetPublish
            };
            Job job = CreateJob(transformName, mediaJob);
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