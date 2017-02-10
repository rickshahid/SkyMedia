using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
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

        private static JObject GetProcessorConfig(string documentId)
        {
            JObject processorConfig;
            using (DatabaseClient databaseClient = new DatabaseClient(false))
            {
                processorConfig = databaseClient.GetDocument(documentId);
            }
            return processorConfig;
        }

        private static bool HasProtectionEnabled(ContentProtection contentProtection)
        {
            return contentProtection != null && (contentProtection.AES || contentProtection.DRMPlayReady || contentProtection.DRMWidevine);
        }

        private static MediaJobTask SetJobTask(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            jobTask.Name = Regex.Replace(jobTask.MediaProcessor.ToString(), Constants.CapitalSpacingExpression, Constants.CapitalSpacingReplacement);
            jobTask.InputAssetIds = GetInputAssetIds(inputAssets);
            if (string.IsNullOrEmpty(jobTask.OutputAssetName))
            {
                string assetId = jobTask.InputAssetIds[0];
                IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                jobTask.OutputAssetName = string.Concat(asset.Name, " (", jobTask.Name, ")");
            }
            jobTask.OutputAssetEncryption = HasProtectionEnabled(jobTask.ContentProtection) ? AssetCreationOptions.StorageEncrypted : AssetCreationOptions.None;
            return jobTask;
        }
    }
}
