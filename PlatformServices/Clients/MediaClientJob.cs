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
        private MediaTransformPreset[] GetTransformPresets(string transformName)
        {
            List<MediaTransformPreset> transformPresets = new List<MediaTransformPreset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            foreach (TransformOutput transformOutput in transform.Outputs)
            {
                if (transformOutput.Preset is BuiltInStandardEncoderPreset builtInEncoderPreset)
                {
                    if (builtInEncoderPreset.PresetName == EncoderNamedPreset.ContentAwareEncodingExperimental)
                    {
                        transformPresets.Add(MediaTransformPreset.ContentAwareEncoding);
                    }
                    else
                    {
                        transformPresets.Add(MediaTransformPreset.AdaptiveStreaming);
                    }
                }
                else if (transformOutput.Preset is StandardEncoderPreset standardEncoderPreset)
                {
                    if (false)
                    {
                        transformPresets.Add(MediaTransformPreset.ThumbnailSprite);
                    }
                    else
                    {
                        transformPresets.Add(MediaTransformPreset.ThumbnailImages);
                    }
                }
                else if (transformOutput.Preset is VideoAnalyzerPreset)
                {
                    transformPresets.Add(MediaTransformPreset.VideoAnalyzer);
                }
                else if (transformOutput.Preset is AudioAnalyzerPreset)
                {
                    transformPresets.Add(MediaTransformPreset.AudioAnalyzer);
                }
                else if (transformOutput.Preset is FaceDetectorPreset)
                {
                    transformPresets.Add(MediaTransformPreset.FaceDetector);
                }
            }
            return transformPresets.ToArray();
        }

        private string GetOutputAssetNameSuffix(MediaTransformPreset transformPreset)
        {
            string outputAssetNameSuffix = null;
            switch (transformPreset)
            {
                case MediaTransformPreset.AdaptiveStreaming:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.AdaptiveStreaming;
                    break;
                case MediaTransformPreset.ContentAwareEncoding:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.ContentAwareEncoding;
                    break;
                case MediaTransformPreset.ThumbnailImages:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.ThumbnailImages;
                    break;
                case MediaTransformPreset.ThumbnailSprite:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.ThumbnailSprite;
                    break;
                case MediaTransformPreset.VideoAnalyzer:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.VideoAnalyzer;
                    break;
                case MediaTransformPreset.AudioAnalyzer:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.AudioAnalyzer;
                    break;
                case MediaTransformPreset.FaceDetector:
                    outputAssetNameSuffix = Constant.Media.Job.OutputAssetNameSuffix.FaceDetector;
                    break;
            }
            return outputAssetNameSuffix;
        }

        private string GetOutputAssetName(MediaTransformPreset transformPreset, MediaJob mediaJob)
        {
            string outputAssetName = mediaJob.InputAssetName;
            if (string.IsNullOrEmpty(outputAssetName) && !string.IsNullOrEmpty(mediaJob.InputFileUrl))
            {
                Uri inputFileUri = new Uri(mediaJob.InputFileUrl);
                outputAssetName = Path.GetFileNameWithoutExtension(inputFileUri.LocalPath);
            }
            string outputAssetNameSuffix = GetOutputAssetNameSuffix(transformPreset);
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

        private IList<JobOutput> GetJobOutputs(MediaTransformPreset[] transformPresets, MediaJob mediaJob)
        {
            List<JobOutputAsset> jobOutputAssets = new List<JobOutputAsset>();
            foreach (MediaTransformPreset transformPreset in transformPresets)
            {
                string outputAssetName = GetOutputAssetName(transformPreset, mediaJob);
                CreateAsset(mediaJob.OutputAssetStorage, outputAssetName);
                JobOutputAsset jobOutputAsset = new JobOutputAsset(outputAssetName)
                {
                    Label = transformPreset.ToString()
                };
                jobOutputAssets.Add(jobOutputAsset);
            }
            return jobOutputAssets.ToArray();
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
            MediaTransformPreset[] transformPresets = GetTransformPresets(transformName);
            if (string.IsNullOrEmpty(mediaJob.Name))
            {
                mediaJob.Name = Guid.NewGuid().ToString();
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                Input = GetJobInput(mediaJob),
                Outputs = GetJobOutputs(transformPresets, mediaJob),
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