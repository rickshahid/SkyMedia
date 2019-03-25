using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Account
    {
        private static void DeleteEntities<T>(MediaClient mediaClient, MediaEntity entityType) where T : Resource
        {
            T[] entities = mediaClient.GetAllEntities<T>(entityType);
            foreach (T entity in entities)
            {
                if (!entity.Name.StartsWith(Constant.Media.PredefinedPrefix))
                {
                    if (entityType == MediaEntity.Transform)
                    {
                        Job[] jobs = mediaClient.GetAllEntities<Job>(MediaEntity.TransformJob, null, entity.Name);
                        foreach (Job job in jobs)
                        {
                            mediaClient.DeleteEntity(MediaEntity.TransformJob, job.Name, entity.Name);
                        }
                    }
                    mediaClient.DeleteEntity(entityType, entity.Name);
                }
            }
        }

        public static void DeleteEntities(MediaClient mediaClient, bool skipLive)
        {
            DeleteEntities<Asset>(mediaClient, MediaEntity.Asset);
            DeleteEntities<Transform>(mediaClient, MediaEntity.Transform);
            DeleteEntities<ContentKeyPolicy>(mediaClient, MediaEntity.ContentKeyPolicy);
            DeleteEntities<StreamingPolicy>(mediaClient, MediaEntity.StreamingPolicy);
            DeleteEntities<StreamingLocator>(mediaClient, MediaEntity.StreamingLocator);
            DeleteEntities<AccountFilter>(mediaClient, MediaEntity.FilterAccount);
            if (!skipLive)
            {
                DeleteEntities<LiveEvent>(mediaClient, MediaEntity.LiveEvent);
            }
            if (mediaClient.IndexerEnabled())
            {
                JArray insights = mediaClient.IndexerGetInsights();
                foreach (JToken insight in insights)
                {
                    string insightId = insight["id"].ToString();
                    mediaClient.IndexerDeleteVideo(insightId);
                }
            }
        }

        public static string[][] GetEntityCounts(MediaClient mediaClient)
        {
            int assets = mediaClient.GetEntityCount<Asset>(MediaEntity.Asset);
            int transforms = mediaClient.GetEntityCount<Transform>(MediaEntity.Transform);
            int transformJobs = mediaClient.GetEntityCount<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            int contentKeyPolicies = mediaClient.GetEntityCount<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy);
            int streamingPolicies = mediaClient.GetEntityCount<StreamingPolicy>(MediaEntity.StreamingPolicy);
            int streamingEndpoints = mediaClient.GetEntityCount<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            int streamingLocators = mediaClient.GetEntityCount<StreamingLocator>(MediaEntity.StreamingLocator);
            int filtersAccount = mediaClient.GetEntityCount<AccountFilter>(MediaEntity.FilterAccount);
            int filtersAsset = mediaClient.GetEntityCount<AssetFilter, Asset>(MediaEntity.FilterAsset, MediaEntity.Asset);
            int liveEvents = mediaClient.GetEntityCount<LiveEvent>(MediaEntity.LiveEvent);
            int liveEventOutputs = mediaClient.GetEntityCount<LiveOutput, LiveEvent>(MediaEntity.LiveEventOutput, MediaEntity.LiveEvent);
            int indexerInsights = !mediaClient.IndexerEnabled() ? 0 : mediaClient.IndexerGetInsights().Count;

            List<string[]> entityCounts = new List<string[]>();
            entityCounts.Add(new string[] { "Storage Accounts", mediaClient.StorageAccounts.Count.ToString(Constant.TextFormatter.NumericLong), "/account/storageAccounts" });
            entityCounts.Add(new string[] { "Media Assets", assets.ToString(Constant.TextFormatter.NumericLong), "/asset" });
            entityCounts.Add(new string[] { "Media Transforms", transforms.ToString(Constant.TextFormatter.NumericLong), "/transform" });
            entityCounts.Add(new string[] { "Media Transform Jobs", transformJobs.ToString(Constant.TextFormatter.NumericLong), "/job" });
            entityCounts.Add(new string[] { "Content Key Policies", contentKeyPolicies.ToString(Constant.TextFormatter.NumericLong), "/account/contentKeyPolicies" });
            entityCounts.Add(new string[] { "Streaming Policies", streamingPolicies.ToString(Constant.TextFormatter.NumericLong), "/account/streamingPolicies" });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpoints.ToString(Constant.TextFormatter.NumericLong), "/account/streamingEndpoints" });
            entityCounts.Add(new string[] { "Streaming Locators", streamingLocators.ToString(Constant.TextFormatter.NumericLong), "/account/streamingLocators" });
            entityCounts.Add(new string[] { "Filters (Account)", filtersAccount.ToString(Constant.TextFormatter.NumericLong), "/account/filtersAccount" });
            entityCounts.Add(new string[] { "Filters (Asset)", filtersAsset.ToString(Constant.TextFormatter.NumericLong), "/account/filtersAsset" });
            entityCounts.Add(new string[] { "Live Events", liveEvents.ToString(Constant.TextFormatter.NumericLong), "/account/liveEvents" });
            entityCounts.Add(new string[] { "Live Event Outputs", liveEventOutputs.ToString(Constant.TextFormatter.NumericLong), "/account/liveEventOutputs" });
            entityCounts.Add(new string[] { "Video Indexer Insights", indexerInsights.ToString(Constant.TextFormatter.NumericLong), "/insight" });

            return entityCounts.ToArray();
        }

        public static Dictionary<string, string> GetStorageAccounts(string authToken)
        {
            Dictionary<string, string> storageAccounts = new Dictionary<string, string>();
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                IList<StorageAccount> mediaStorageAccounts = mediaClient.StorageAccounts;
                foreach (StorageAccount mediaStorageAccount in mediaStorageAccounts)
                {
                    MediaStorage mediaStorage = new MediaStorage(mediaClient.MediaAccount, mediaStorageAccount);
                    string accountName = Path.GetFileName(mediaStorageAccount.Id);
                    string accountInfo = string.Concat(accountName, mediaStorage.ToString());
                    storageAccounts.Add(accountName, accountInfo);
                }
            }
            return storageAccounts;
        }

        public static CloudStorageAccount GetStorageAccount(MediaAccount mediaAccount, string accountName)
        {
            CloudStorageAccount storageAccount;
            if (mediaAccount == null)
            {
                string settingKey = Constant.AppSettingKey.AzureStorage;
                string systemStorage = AppSetting.GetValue(settingKey);
                storageAccount = CloudStorageAccount.Parse(systemStorage);
            }
            else
            {
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = mediaAccount.StorageAccounts.First().Key;
                }
                string accountKey = mediaAccount.StorageAccounts[accountName];
                StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);
                storageAccount = new CloudStorageAccount(storageCredentials, true);
            }
            return storageAccount;
        }

        public static CloudStorageAccount GetStorageAccount()
        {
            return GetStorageAccount(null, null);
        }
    }
}