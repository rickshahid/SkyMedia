using System;
using System.Linq;
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
            JobInputAsset inputAsset = new JobInputAsset(mediaJob.InputAssetName);
            string[] outputAssetNames = mediaJob.OutputAssetNames;
            if (outputAssetNames == null)
            {
                List<string> outputAssetNameList = new List<string>();
                Transform transform = mediaClient.GetEntity<Transform>(MediaEntity.Transform, transformName);
                foreach (TransformOutput transformOutput in transform.Outputs)
                {
                    string outputName = transformOutput.Preset.ToString().Split('.').Last();
                    string outputAssetName = string.Concat(mediaJob.InputAssetName, Constant.Media.Asset.NameDelimiter, outputName);
                    outputAssetNameList.Add(outputAssetName);
                }
                outputAssetNames = outputAssetNameList.ToArray();
            }
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            foreach (string outputAssetName in outputAssetNames)
            {
                mediaClient.CreateAsset(null, outputAssetName);
                JobOutputAsset outputAsset = new JobOutputAsset(outputAssetName);
                outputAssets.Add(outputAsset);
            }
            Job job = new Job()
            {
                Description = mediaJob.Description,
                Priority = mediaJob.Priority,
                Input = inputAsset,
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