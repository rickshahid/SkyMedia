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

        private static MediaJobTask MapJobTask(MediaClient mediaClient, MediaJobTask jobTask, string assetName)
        {
            jobTask.Name = Processor.GetProcessorName(jobTask.ProcessorType);
            if (jobTask.ProcessorConfig.Contains(Constant.Media.ProcessorConfig.EncoderStandardThumbnailsFormat))
            {
                jobTask.Name = string.Concat(jobTask.Name, " ", Constant.Media.ProcessorConfig.EncoderStandardThumbnailsPreset);
            }
            if (string.IsNullOrEmpty(jobTask.OutputAssetName))
            {
                jobTask.OutputAssetName = string.Concat(assetName, " - ", jobTask.Name);
            }
            jobTask.OutputAssetEncryption = jobTask.ContentProtection != null ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            return jobTask;
        }

        private static MediaJobTask[] MapJobTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets, bool multipleInputTask)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (multipleInputTask)
            {
                jobTask = MapJobTask(mediaClient, jobTask, inputAssets[0].AssetName);
                jobTask.InputAssetIds = GetAssetIds(inputAssets);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (MediaAssetInput inputAsset in inputAssets)
                {
                    MediaJobTask newJobTask = jobTask.CreateCopy();
                    newJobTask = MapJobTask(mediaClient, newJobTask, inputAsset.AssetName);
                    newJobTask.InputAssetIds = new string[] { inputAsset.AssetId };
                    jobTasks.Add(newJobTask);
                }
            }
            return jobTasks.ToArray();
        }
    }
}
