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
            JobInput jobInput;
            string outputAssetName;
            string storageAccount = null;
            if (!string.IsNullOrEmpty(mediaJob.InputFileUrl))
            {
                jobInput = new JobInputHttp()
                {
                    Files = new string[] { mediaJob.InputFileUrl }
                };
                outputAssetName = Path.GetFileNameWithoutExtension(mediaJob.InputFileUrl);
            }
            else
            {
                Asset asset = GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
                storageAccount = asset.StorageAccountName;
                jobInput = new JobInputAsset(asset.Name);
                outputAssetName = mediaJob.InputAssetName;
            }
            CreateAsset(storageAccount, outputAssetName, mediaJob.OutputAssetDescription, mediaJob.OutputAssetAlternateId);
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
                Input = jobInput,
                Outputs = outputAssets.ToArray()
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, mediaJob.Name, job);
        }

        public Job CreateJob(string transformName, string jobName, string jobDescription, Priority jobPriority, string inputAssetName, string inputFileUrl, string outputAssetDescription, string indexId, string streamingPolicyName)
        {
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Description = jobDescription,
                Priority = jobPriority,
                InputAssetName = inputAssetName,
                InputFileUrl = inputFileUrl,
                OutputAssetDescription = outputAssetDescription,
                OutputAssetAlternateId = indexId
            };
            Job job = CreateJob(transformName, mediaJob);
            MediaPublish mediaPublish = new MediaPublish()
            {
                Id = job.Name,
                TransformName = transformName,
                StreamingPolicyName = streamingPolicyName,
                MediaAccount = MediaAccount,
                UserAccount = UserAccount
            };
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.ContentPublish;
                databaseClient.UpsertDocument(collectionId, mediaPublish);
            }
            return job;
        }

        public Job CreateJob(string transformName, MediaIngestManifest ingestManifest)
        {
            string indexId = null;
            using (MediaClient mediaClient = new MediaClient(null, ingestManifest.MediaAccount))
            {
                bool videoAnalyzerPreset = HasTransformPreset(ingestManifest.TransformPresets, MediaTransformPreset.VideoAnalyzer);
                bool audioAnalyzerPreset = HasTransformPreset(ingestManifest.TransformPresets, MediaTransformPreset.AudioAnalyzer);
                if (mediaClient.IndexerIsEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
                {
                    string videoUrl = ingestManifest.JobInputFileUrl;
                    if (string.IsNullOrEmpty(videoUrl))
                    {
                        BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, ingestManifest.StorageAccount);
                        Asset inputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, ingestManifest.AssetName);
                        string fileName = Path.GetFileNameWithoutExtension(ingestManifest.AssetFiles[0]);
                        videoUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                    }
                    bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                    indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, videoUrl, string.Empty, string.Empty, string.Empty, audioOnly);
                }
            }
            if (string.IsNullOrEmpty(ingestManifest.AssetName) && ingestManifest.AssetFiles.Length > 0)
            {
                ingestManifest.AssetName = Path.GetFileNameWithoutExtension(ingestManifest.AssetFiles[0]);
            }
            return CreateJob(transformName, ingestManifest.JobName, ingestManifest.JobDescription, ingestManifest.JobPriority, ingestManifest.AssetName, ingestManifest.JobInputFileUrl, ingestManifest.JobOutputAssetDescription, indexId, ingestManifest.StreamingPolicyName);
        }

        public void CancelJob(string transformName, string jobName)
        {
            _media.Jobs.CancelJob(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName);
        }
    }
}