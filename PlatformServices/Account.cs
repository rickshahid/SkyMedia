using System.Collections.Generic;

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
                    mediaClient.DeleteEntity(entityType, entity.Name);
                }
            }
        }

        public static void DeleteEntities(MediaClient mediaClient, bool liveOnly)
        {
            if (!liveOnly)
            {
                DeleteEntities<Asset>(mediaClient, MediaEntity.Asset);
                DeleteEntities<Transform>(mediaClient, MediaEntity.Transform);
                DeleteEntities<ContentKeyPolicy>(mediaClient, MediaEntity.ContentKeyPolicy);
                DeleteEntities<StreamingPolicy>(mediaClient, MediaEntity.StreamingPolicy);
                DeleteEntities<StreamingLocator>(mediaClient, MediaEntity.StreamingLocator);
                JArray insights = mediaClient.IndexerGetInsights();
                foreach (JToken insight in insights)
                {
                    string indexId = insight["id"].ToString();
                    mediaClient.IndexerDeleteVideo(indexId, true);
                }
            }
            DeleteEntities<LiveEvent>(mediaClient, MediaEntity.LiveEvent);
        }

        public static string[][] GetEntityCounts(MediaClient mediaClient)
        {
            int assetCount = mediaClient.GetEntityCount<Asset>(MediaEntity.Asset);
            int transformCount = mediaClient.GetEntityCount<Transform>(MediaEntity.Transform);
            int transformJobCount = mediaClient.GetEntityCount<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            int contentKeyPolicyCount = mediaClient.GetEntityCount<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy);
            int streamingPolicyCount = mediaClient.GetEntityCount<StreamingPolicy>(MediaEntity.StreamingPolicy);
            int streamingEndpointCount = mediaClient.GetEntityCount<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            int streamingLocatorCount = mediaClient.GetEntityCount<StreamingLocator>(MediaEntity.StreamingLocator);
            int liveEventCount = mediaClient.GetEntityCount<LiveEvent>(MediaEntity.LiveEvent);
            int liveEventOutputCount = mediaClient.GetEntityCount<LiveOutput, LiveEvent>(MediaEntity.LiveEventOutput, MediaEntity.LiveEvent);
            int videoIndexerInsights = mediaClient.IndexerGetInsights().Count;

            List<string[]> entityCounts = new List<string[]>();
            entityCounts.Add(new string[] { "Storage Accounts", mediaClient.StorageAccounts.Count.ToString(Constant.TextFormatter.NumericLong), "/account/storageAccounts" });
            entityCounts.Add(new string[] { "Assets", assetCount.ToString(Constant.TextFormatter.NumericLong), "/account/assets" });
            entityCounts.Add(new string[] { "Transforms", transformCount.ToString(Constant.TextFormatter.NumericLong), "/account/transforms" });
            entityCounts.Add(new string[] { "Transform Jobs", transformJobCount.ToString(Constant.TextFormatter.NumericLong), "/account/transformJobs" });
            entityCounts.Add(new string[] { "Content Key Policies", contentKeyPolicyCount.ToString(Constant.TextFormatter.NumericLong), "/account/contentKeyPolicies" });
            entityCounts.Add(new string[] { "Streaming Policies", streamingPolicyCount.ToString(Constant.TextFormatter.NumericLong), "/account/streamingPolicies" });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpointCount.ToString(Constant.TextFormatter.NumericLong), "/account/streamingEndpoints" });
            entityCounts.Add(new string[] { "Streaming Locators", streamingLocatorCount.ToString(Constant.TextFormatter.NumericLong), "/account/streamingLocators" });
            entityCounts.Add(new string[] { "Live Events", liveEventCount.ToString(Constant.TextFormatter.NumericLong), "/account/liveEvents" });
            entityCounts.Add(new string[] { "Live Event Outputs", liveEventOutputCount.ToString(Constant.TextFormatter.NumericLong), "/account/liveEventOutputs" });
            entityCounts.Add(new string[] { "Video Indexer Insights", videoIndexerInsights.ToString(Constant.TextFormatter.NumericLong), "/account/videoIndexerInsights" });

            return entityCounts.ToArray();
        }
    }
}