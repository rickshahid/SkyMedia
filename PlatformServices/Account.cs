using System.Collections.Generic;

using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Account
    {
        //    }
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
            int streamingFiltersCount = 0;
            int liveEventCount = mediaClient.GetEntityCount<LiveEvent>(MediaEntity.LiveEvent);
            int liveEventOutputCount = mediaClient.GetEntityCount<LiveOutput, LiveEvent>(MediaEntity.LiveEventOutput, MediaEntity.LiveEvent);

            List<string[]> entityCounts = new List<string[]>();
            entityCounts.Add(new string[] { "Storage Accounts", mediaClient.StorageAccounts.Count.ToString() });
            entityCounts.Add(new string[] { "Assets", assetCount.ToString(), "/account/assets" });
            entityCounts.Add(new string[] { "Transforms", transformCount.ToString(), "/account/transforms" });
            entityCounts.Add(new string[] { "Transform Jobs", transformJobCount.ToString(), "/account/transformJobs" });
            entityCounts.Add(new string[] { "Content Key Policies", contentKeyPolicyCount.ToString(), "/account/contentKeyPolicies" });
            entityCounts.Add(new string[] { "Streaming Policies", streamingPolicyCount.ToString(), "/account/streamingPolicies" });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpointCount.ToString(), "/account/streamingEndpoints" });
            entityCounts.Add(new string[] { "Streaming Locators", streamingLocatorCount.ToString(), "/account/streamingLocators" });
            entityCounts.Add(new string[] { "Streaming Filters", streamingFiltersCount.ToString(), "/account/streamingFilters" });
            entityCounts.Add(new string[] { "Live Events", liveEventCount.ToString(), "/account/liveEvents" });
            entityCounts.Add(new string[] { "Live Event Outputs", liveEventOutputCount.ToString(), "/account/liveEventOutputs" });

            return entityCounts.ToArray();
        }
    }
}