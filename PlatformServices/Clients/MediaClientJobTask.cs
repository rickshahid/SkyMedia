using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static string[] GetAssetIds(MediaAssetInput[] inputAssets)
        {
            List<string> assetIds = new List<string>();
            foreach (MediaAssetInput inputAsset in inputAssets)
            {
                assetIds.Add(inputAsset.AssetId);
            }
            return assetIds.ToArray();
        }

        private static JObject GetProcessorConfig(string documentId)
        {
            JObject processorConfig;
            using (CosmosClient cosmosClient = new CosmosClient(false))
            {
                processorConfig = cosmosClient.GetDocument(documentId);
            }
            return processorConfig;
        }

        private static MediaJobTask SetJobTask(MediaClient mediaClient, MediaJobTask jobTask, string assetName)
        {
            jobTask.Name = Processor.GetProcessorName(jobTask.MediaProcessor);
            if (string.IsNullOrEmpty(jobTask.OutputAssetName))
            {
                jobTask.OutputAssetName = string.Concat(assetName, " - ", jobTask.Name);
            }
            jobTask.OutputAssetEncryption = AssetCreationOptions.None;
            if (jobTask.ContentProtection != null)
            {
                jobTask.OutputAssetEncryption = AssetCreationOptions.StorageEncrypted;
            }
            return jobTask;
        }

        private static MediaJobTask[] SetJobTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, bool multipleInputTask)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (multipleInputTask)
            {
                jobTask = SetJobTask(mediaClient, jobTask, inputAssets[0].AssetName);
                jobTask.InputAssetIds = GetAssetIds(inputAssets);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (MediaAssetInput inputAsset in inputAssets)
                {
                    MediaJobTask newTask = jobTask.CreateCopy();
                    newTask = SetJobTask(mediaClient, newTask, inputAsset.AssetName);
                    newTask.InputAssetIds = new string[] { inputAsset.AssetId };
                    jobTasks.Add(newTask);
                }
            }
            return jobTasks.ToArray();
        }
    }
}
