using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
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
                if (!entity.Name.StartsWith(Constant.Media.PredefinedPrefix, StringComparison.OrdinalIgnoreCase))
                {
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
            DeleteEntities<AccountFilter>(mediaClient, MediaEntity.StreamingFilterAccount);
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
            int streamingFiltersAccount = mediaClient.GetEntityCount<AccountFilter>(MediaEntity.StreamingFilterAccount);
            int streamingFiltersAsset = mediaClient.GetEntityCount<AssetFilter, Asset>(MediaEntity.StreamingFilterAsset, MediaEntity.Asset);
            int liveEvents = mediaClient.GetEntityCount<LiveEvent>(MediaEntity.LiveEvent);
            int liveEventOutputs = mediaClient.GetEntityCount<LiveOutput, LiveEvent>(MediaEntity.LiveEventOutput, MediaEntity.LiveEvent);
            int indexerInsights = !mediaClient.IndexerEnabled() ? 0 : mediaClient.IndexerGetInsights().Count;
            int indexerProjects = !mediaClient.IndexerEnabled() ? 0 : mediaClient.IndexerGetProjects().Count;

            List<string[]> entityCounts = new List<string[]>
            {
                new string[] { "Storage Accounts", mediaClient.StorageAccounts.Count.ToString(Constant.TextFormatter.NumericLong), "/account/storageAccounts" },
                new string[] { "Media Assets", assets.ToString(Constant.TextFormatter.NumericLong), "/asset" },
                new string[] { "Media Transforms", transforms.ToString(Constant.TextFormatter.NumericLong), "/transform" },
                new string[] { "Media Transform Jobs", transformJobs.ToString(Constant.TextFormatter.NumericLong), "/job" },
                new string[] { "Content Key Policies", contentKeyPolicies.ToString(Constant.TextFormatter.NumericLong), "/account/contentKeyPolicies" },
                new string[] { "Streaming Policies", streamingPolicies.ToString(Constant.TextFormatter.NumericLong), "/account/streamingPolicies" },
                new string[] { "Streaming Endpoints", streamingEndpoints.ToString(Constant.TextFormatter.NumericLong), "/account/streamingEndpoints" },
                new string[] { "Streaming Locators", streamingLocators.ToString(Constant.TextFormatter.NumericLong), "/account/streamingLocators" },
                new string[] { "Streaming Filters (Account)", streamingFiltersAccount.ToString(Constant.TextFormatter.NumericLong), "/account/streamingFiltersAccount" },
                new string[] { "Streaming Filters (Asset)", streamingFiltersAsset.ToString(Constant.TextFormatter.NumericLong), "/account/streamingFiltersAsset" },
                new string[] { "Live Events", liveEvents.ToString(Constant.TextFormatter.NumericLong), "/account/liveEvents" },
                new string[] { "Live Event Outputs", liveEventOutputs.ToString(Constant.TextFormatter.NumericLong), "/account/liveEventOutputs" },
                new string[] { "Video Indexer Insights", indexerInsights.ToString(Constant.TextFormatter.NumericLong), "/insight" },
                new string[] { "Video Indexer Projects", indexerProjects.ToString(Constant.TextFormatter.NumericLong), "/insight/projects" }
            };

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