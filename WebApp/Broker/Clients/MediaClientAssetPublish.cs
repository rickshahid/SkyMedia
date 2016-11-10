using System;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private static IAsset[] GetTaskOutputs(IJob job, string[] processorIds)
        {
            List<IAsset> taskOutputs = new List<IAsset>();
            foreach (string processorId in processorIds)
            {
                foreach (ITask jobTask in job.Tasks)
                {
                    if (string.Equals(jobTask.MediaProcessorId, processorId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        taskOutputs.AddRange(jobTask.OutputAssets);
                    }
                }
            }
            return taskOutputs.ToArray();
        }

        private static IAsset GetTaskOutput(IJob job, string[] processorIds)
        {
            IAsset[] taskOutputs = GetTaskOutputs(job, processorIds);
            return (taskOutputs.Length == 0) ? null : taskOutputs[0];
        }

        private static void PublishIndex(IJob job, IAsset encoderAsset, ContentPublish contentPublish)
        {
            string settingKey = Constants.AppSettings.MediaProcessorIndexerV1Id;
            string processorIdV1 = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorIndexerV2Id;
            string processorIdV2 = AppSetting.GetValue(settingKey);
            string[] processorIds = new string[] { processorIdV1, processorIdV2 };
            IAsset[] indexAssets = GetTaskOutputs(job, processorIds);
            if (indexAssets.Length > 0)
            {
                BlobClient blobClient = new BlobClient(contentPublish);
                foreach (IAsset indexAsset in indexAssets)
                {
                    string fileExtension = Constants.Media.AssetMetadata.WebVttExtension;
                    string[] fileNames = GetFileNames(indexAsset, fileExtension);
                }
            }
        }

        public static string[] GetFileNames(IAsset asset, string fileExtension)
        {
            List<string> fileNames = new List<string>();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(fileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    fileNames.Add(assetFile.Name);
                }
            }
            return fileNames.ToArray();
        }

        public static void PublishJob(MediaClient mediaClient, IJob job, ContentPublish contentPublish, ContentProtection contentProtection)
        {
            string settingKey = Constants.AppSettings.MediaProcessorEncoderStandardId;
            string processorIdStandard = AppSetting.GetValue(settingKey);
            settingKey = Constants.AppSettings.MediaProcessorEncoderPremiumId;
            string processorIdPremium = AppSetting.GetValue(settingKey);
            string[] processorIds = new string[] { processorIdStandard, processorIdPremium };
            IAsset asset = GetTaskOutput(job, processorIds);
            if (asset != null)
            {
                PublishAsset(mediaClient, asset, contentProtection);
                if (asset.IsStreamable)
                {
                    PublishIndex(job, asset, contentPublish);
                }
            }
        }

        public static void PublishAsset(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
        {
            if (asset.IsStreamable || asset.AssetType == AssetType.MP4)
            {
                if (asset.Options == AssetCreationOptions.StorageEncrypted && asset.DeliveryPolicies.Count == 0)
                {
                    mediaClient.AddDeliveryPolicies(asset, contentProtection);
                }
                if (asset.Locators.Count == 0)
                {
                    LocatorType locatorType = LocatorType.OnDemandOrigin;
                    mediaClient.CreateLocator(null, locatorType, asset, null);
                }
            }
        }
    }
}
