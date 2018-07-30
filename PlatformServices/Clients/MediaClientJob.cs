using System;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public Job CreateJob(MediaClient mediaClient, string transformName, MediaJob mediaJob)
        {
            string jobName = mediaJob.Name;
            if (string.IsNullOrEmpty(jobName))
            {
                jobName = Guid.NewGuid().ToString();
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
                jobInput = new JobInputAsset(mediaJob.InputAssetName);
            }
            Asset inputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, mediaJob.InputAssetName);
            string outputAssetName = string.Concat(mediaJob.InputAssetName, Constant.Media.Asset.NameDelimiter, Constant.TextFormatter.GetValue(transformName));
            mediaClient.CreateAsset(inputAsset.StorageAccountName, outputAssetName);
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            Transform transform = mediaClient.GetEntity<Transform>(MediaEntity.Transform, transformName);
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
                Outputs = outputAssets.ToArray(),
                CorrelationData = mediaJob.CorrelationData
            };
            return _media.Jobs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName, job);
        }

        public void CancelJob(string transformName, string jobName)
        {
            _media.Jobs.CancelJob(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName);
        }
    }
}