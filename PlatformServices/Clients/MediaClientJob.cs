using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.ApplicationInsights;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
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

        private void SetOutputAssetLink(MediaTransformPreset[] transformPresets, JobOutputAsset[] jobOutputs, MediaJobOutputInsight outputInsight)
        {
            if (transformPresets.Length > 1 || !string.IsNullOrEmpty(outputInsight.Id))
            {
                string encodingOutputAssetName = null;
                Dictionary<MediaTransformPreset, string> jobOutputAssets = new Dictionary<MediaTransformPreset, string>();
                for (int i = 0; i < transformPresets.Length; i++)
                {
                    MediaTransformPreset transformPreset = transformPresets[i];
                    if (transformPreset == MediaTransformPreset.AdaptiveStreaming || transformPreset == MediaTransformPreset.ContentAwareEncoding)
                    {
                        encodingOutputAssetName = jobOutputs[i].AssetName;
                    }
                    else
                    {
                        jobOutputAssets.Add(transformPreset, jobOutputs[i].AssetName);
                    }
                }
                if (!string.IsNullOrEmpty(outputInsight.Id))
                {
                    if (outputInsight.VideoIndexer)
                    {
                        jobOutputAssets.Add(MediaTransformPreset.VideoIndexer, outputInsight.Id);
                    }
                    if (outputInsight.AudioIndexer)
                    {
                        jobOutputAssets.Add(MediaTransformPreset.AudioIndexer, outputInsight.Id);
                    }
                }
                if (!string.IsNullOrEmpty(encodingOutputAssetName))
                {
                    MediaAssetLink assetLink = new MediaAssetLink()
                    {
                        AssetName = encodingOutputAssetName,
                        JobOutputs = jobOutputAssets,
                        MediaAccount = this.MediaAccount,
                        UserAccount = this.UserAccount
                    };
                    using (DatabaseClient databaseClient = new DatabaseClient(true))
                    {
                        string collectionId = Constant.Database.Collection.MediaAssets;
                        databaseClient.UpsertDocument(collectionId, assetLink);
                    }
                }
            }
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

        private JobOutputAsset[] GetJobOutputs(MediaTransformPreset[] transformPresets, MediaJob mediaJob)
        {
            List<JobOutputAsset> jobOutputs = new List<JobOutputAsset>();
            foreach (MediaTransformPreset transformPreset in transformPresets)
            {
                string outputAssetName = GetOutputAssetName(transformPreset, mediaJob);
                CreateAsset(mediaJob.OutputAssetStorage, outputAssetName);
                JobOutputAsset jobOutput = new JobOutputAsset(outputAssetName);
                jobOutputs.Add(jobOutput);
            }
            return jobOutputs.ToArray();
        }

        private IDictionary<string, string> GetJobPublish(MediaJobOutputPublish jobOutputPublish)
        {
            Dictionary<string, string> jobPublish = new Dictionary<string, string>();
            string outputPublish = JsonConvert.SerializeObject(jobOutputPublish);
            string mediaAccount = JsonConvert.SerializeObject(this.MediaAccount);
            string userAccount = JsonConvert.SerializeObject(this.UserAccount);
            jobPublish.Add(Constant.Media.Job.CorrelationData.OutputPublish, outputPublish);
            jobPublish.Add(Constant.Media.Job.CorrelationData.MediaAccount, mediaAccount);
            jobPublish.Add(Constant.Media.Job.CorrelationData.UserAccount, userAccount);
            return jobPublish;
        }

        private Job CreateJob(string transformName, MediaJob mediaJob)
        {
            Job job = null;
            try
            {
                MediaTransformPreset[] transformPresets = GetTransformPresets(transformName);
                JobInput jobInput = GetJobInput(mediaJob);
                JobOutputAsset[] jobOutputs = GetJobOutputs(transformPresets, mediaJob);
                IDictionary<string, string> jobPublish = GetJobPublish(mediaJob.OutputPublish);
                job = new Job()
                {
                    Description = mediaJob.Description,
                    Priority = mediaJob.Priority,
                    Input = jobInput,
                    Outputs = jobOutputs,
                    CorrelationData = jobPublish
                };
                job = _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
                SetOutputAssetLink(transformPresets, jobOutputs, mediaJob.OutputInsight);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> exProperties = new Dictionary<string, string>
                {
                    { "Media Account", JsonConvert.SerializeObject(MediaAccount) },
                    { "Transform Name", transformName },
                    { "Media Job", JsonConvert.SerializeObject(mediaJob) }
                };
                if (job != null)
                {
                    exProperties.Add("Job", JsonConvert.SerializeObject(job));
                }
                TelemetryClient telemetryClient = new TelemetryClient();
                telemetryClient.TrackException(ex, exProperties);
                throw;
            }
            return job;
        }

        public Job CreateJob(string transformName, string jobName, string jobDescription, Priority jobPriority, string inputFileUrl, string inputAssetName,
                             string outputAssetStorage, MediaJobOutputPublish outputPublish, MediaJobOutputInsight outputInsight)
        {
            EventGridClient.SetMediaSubscription(this.MediaAccount);
            if (string.IsNullOrEmpty(jobName))
            {
                jobName = Guid.NewGuid().ToString();
            }
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputFileUrl = inputFileUrl,
                InputAssetName = inputAssetName,
                OutputAssetStorage = outputAssetStorage,
                OutputPublish = outputPublish,
                OutputInsight = outputInsight
            };
            return CreateJob(transformName, mediaJob);
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