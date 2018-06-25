using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public Job CreateJob(string transformName, string jobName, JobInput jobInput, string[] outputAssetNames)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                jobName = Guid.NewGuid().ToString();
            }
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            foreach (string outputAssetName in outputAssetNames)
            {
                JobOutputAsset outputAsset = new JobOutputAsset(outputAssetName);
                outputAssets.Add(outputAsset);
            }
            Job job = new Job()
            {
                Input = jobInput,
                Outputs = outputAssets.ToArray()
            };
            Task<AzureOperationResponse<Job>> task = _media.Jobs.CreateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName, job);
            AzureOperationResponse<Job> response = task.Result;
            return response.Body;
        }

        public string CancelJob(string transformName, string jobName)
        {
            Task<AzureOperationResponse> task = _media.Jobs.CancelJobWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName);
            AzureOperationResponse response = task.Result;
            return response.RequestId;
        }
    }
}