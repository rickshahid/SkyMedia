using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private static string[] GetInputAssetIds(MediaAssetInput[] inputAssets)
        {
            List<string> assetIds = new List<string>();
            foreach (MediaAssetInput inputAsset in inputAssets)
            {
                assetIds.Add(inputAsset.AssetId);
            }
            return assetIds.ToArray();
        }

        private static AssetCreationOptions GetOutputAssetEncryption(ContentProtection contentProtection)
        {
            AssetCreationOptions assetEncryption = AssetCreationOptions.None;
            if (contentProtection.AES || contentProtection.DRMPlayReady || contentProtection.DRMWidevine || contentProtection.DRMFairPlay)
            {
                assetEncryption = AssetCreationOptions.StorageEncrypted;
            }
            return assetEncryption;
        }

        private static MediaJobTask GetJobTask(MediaClient mediaClient, string taskName, MediaProcessor mediaProcessor, string processorConfig,
                                               MediaAssetInput[] inputAssets, string outputAssetName, ContentProtection contentProtection,
                                               TaskOptions taskOptions)
        {
            MediaJobTask jobTask = new MediaJobTask();
            jobTask.Name = taskName;
            jobTask.MediaProcessor = mediaProcessor;
            jobTask.ProcessorConfig = (processorConfig == null) ? string.Empty : processorConfig;
            jobTask.InputAssetIds = GetInputAssetIds(inputAssets);
            jobTask.OutputAssetName = outputAssetName;
            if (string.IsNullOrEmpty(jobTask.OutputAssetName))
            {
                string assetId = jobTask.InputAssetIds[0];
                IAsset asset = mediaClient.GetEntityById(EntityType.Asset, assetId) as IAsset;
                jobTask.OutputAssetName = string.Concat(asset.Name, " (", jobTask.Name, ")");
            }
            if (mediaProcessor == MediaProcessor.EncoderStandard || mediaProcessor == MediaProcessor.EncoderPremium)
            {
                jobTask.OutputAssetEncryption = GetOutputAssetEncryption(contentProtection);
            }
            jobTask.Options = taskOptions;
            return jobTask;
        }

        private static MediaJobTask[] GetJobTasks(MediaClient mediaClient, string taskName, MediaProcessor mediaProcessor, string processorConfig,
                                                  MediaAssetInput[] inputAssets, string[] outputAssetNames, ContentProtection contentProtection,
                                                  TaskOptions taskOptions, bool taskPerInputAsset)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (taskPerInputAsset)
            {
                for (int i = 0; i < inputAssets.Length; i++)
                {
                    MediaJobTask jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig, new MediaAssetInput[] { inputAssets[i] }, outputAssetNames[i], contentProtection, taskOptions);
                    jobTasks.Add(jobTask);
                }
            }
            else
            {
                string outputAssetName = (outputAssetNames == null || outputAssetNames.Length == 0) ? string.Empty : outputAssetNames[0];
                MediaJobTask jobTask = GetJobTask(mediaClient, taskName, mediaProcessor, processorConfig, inputAssets, outputAssetName, contentProtection, taskOptions);
                jobTasks.Add(jobTask);
            }
            return jobTasks.ToArray();
        }
    }
}
