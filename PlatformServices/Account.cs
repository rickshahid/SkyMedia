using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Account
    {
        //private static void DeleteAsset(MediaClient mediaClient, IAsset asset)
        //{
        //    string insightId = asset.AlternateId;
        //    if (!string.IsNullOrEmpty(insightId))
        //    {
        //        DatabaseClient databaseClient = new DatabaseClient();
        //        string collectionId = Constant.Database.Collection.OutputInsight;
        //        MediaInsight mediaInsight = databaseClient.GetDocument<MediaInsight>(collectionId, insightId);
        //        if (mediaInsight != null)
        //        {
        //            foreach (MediaInsightSource insightSource in mediaInsight.Sources)
        //            {
        //                string documentId = insightSource.OutputId;
        //                //if (insightSource.MediaProcessor == MediaProcessor.VideoAnalyzer)
        //                //{
        //                //    VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
        //                //    videoAnalyzer.DeleteVideo(documentId, true);
        //                //    databaseClient.DeleteDocument(Constant.Database.Collection.OutputPublish, documentId);
        //                //}
        //                databaseClient.DeleteDocument(collectionId, documentId);
        //            }
        //            databaseClient.DeleteDocument(collectionId, mediaInsight.Id);
        //        }
        //    }
        //    foreach (ILocator locator in asset.Locators)
        //    {
        //        locator.Delete();
        //    }
        //    for (int i = asset.DeliveryPolicies.Count - 1; i >= 0; i--)
        //    {
        //        asset.DeliveryPolicies.RemoveAt(i);
        //    }
        //    asset.Delete();
        //}

        //private static void DeleteLive(MediaClient mediaClient, bool deleteAssets)
        //{
        //    //IProgram[] programs = mediaClient.GetEntities(MediaEntity.Program) as IProgram[];
        //    //foreach (IProgram program in programs)
        //    //{
        //    //    if (program.State == ProgramState.Running)
        //    //    {
        //    //        program.Stop();
        //    //    }
        //    //    program.Delete();
        //    //    if (deleteAssets)
        //    //    {
        //    //        DeleteAsset(mediaClient, program.Asset);
        //    //    }
        //    //}
        //    //IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
        //    //foreach (IChannel channel in channels)
        //    //{
        //    //    if (channel.State == ChannelState.Running)
        //    //    {
        //    //        channel.Stop();
        //    //    }
        //    //    channel.Delete();
        //    //}
        //}

        //private static void DeleteJob(IJob job)
        //{
        //    DatabaseClient databaseClient = new DatabaseClient();
        //    string collectionId = Constant.Database.Collection.OutputPublish;
        //    foreach (ITask jobTask in job.Tasks)
        //    {
        //        databaseClient.DeleteDocument(collectionId, jobTask.Id);
        //    }
        //    databaseClient.DeleteDocument(collectionId, job.Id);
        //    job.Delete();
        //}

        //public static void DeleteEntity(string authToken, MediaEntity entityType, string entityId)
        //{
        //    MediaClient mediaClient = new MediaClient(authToken);
        //    switch (entityType)
        //    {
        //        //case MediaEntity.ContentKey:
        //        //    IContentKey contentKey = mediaClient.GetEntityById(entityType, entityId) as IContentKey;
        //        //    contentKey.Delete();
        //        //    break;

        //        //case MediaEntity.ContentKeyAuthPolicy:
        //        //    IContentKeyAuthorizationPolicy contentKeyAuthPolicy = mediaClient.GetEntityById(entityType, entityId) as IContentKeyAuthorizationPolicy;
        //        //    contentKeyAuthPolicy.Delete();
        //        //    break;

        //        //case MediaEntity.ContentKeyAuthPolicyOption:
        //        //    IContentKeyAuthorizationPolicyOption contentKeyAuthPolicyOption = mediaClient.GetEntityById(entityType, entityId) as IContentKeyAuthorizationPolicyOption;
        //        //    contentKeyAuthPolicyOption.Delete();
        //        //    break;

        //        //case MediaEntity.NotificationEndpoint:
        //        //    INotificationEndPoint notificationEndpoint = mediaClient.GetEntityById(entityType, entityId) as INotificationEndPoint;
        //        //    notificationEndpoint.Delete();
        //        //    break;

        //        //case MediaEntity.Asset:
        //        //    IAsset asset = mediaClient.GetEntityById(entityType, entityId) as IAsset;
        //        //    DeleteAsset(mediaClient, asset);
        //        //    break;

        //        //case MediaEntity.Job:
        //        //    IJob job = mediaClient.GetEntityById(entityType, entityId) as IJob;
        //        //    DeleteJob(job);
        //        //    break;

        //        //case MediaEntity.DeliveryPolicy:
        //        //    IAssetDeliveryPolicy deliveryPolicy = mediaClient.GetEntityById(entityType, entityId) as IAssetDeliveryPolicy;
        //        //    deliveryPolicy.Delete();
        //        //    break;

        //        //case MediaEntity.AccessPolicy:
        //        //    IAccessPolicy accessPolicy = mediaClient.GetEntityById(entityType, entityId) as IAccessPolicy;
        //        //    accessPolicy.Delete();
        //        //    break;

        //        //case MediaEntity.StreamingLocator:
        //        //    ILocator streamingLocator = mediaClient.GetEntityById(entityType, entityId) as ILocator;
        //        //    streamingLocator.Delete();
        //        //    break;

        //        //case MediaEntity.StreamingFilter:
        //        //    IStreamingFilter streamingFilter = mediaClient.GetEntityById(entityType, entityId) as IStreamingFilter;
        //        //    streamingFilter.Delete();
        //        //    break;
        //    }
        //}

        //public static void DeleteEntities(string authToken, bool allEntities, bool liveChannels)
        //{
        //    MediaClient mediaClient = new MediaClient(authToken);
        //    if (liveChannels)
        //    {
        //        DeleteLive(mediaClient, false);
        //    }
        //    else if (!allEntities)
        //    {
        //        //IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset) as IAsset[];
        //        //foreach (IAsset asset in assets)
        //        //{
        //        //    if (asset.ParentAssets.Count > 0)
        //        //    {
        //        //        DeleteAsset(mediaClient, asset);
        //        //    }
        //        //}
        //    }
        //    else
        //    {
        //        //DeleteLive(mediaClient, true);
        //        //IJobTemplate[] jobTemplates = mediaClient.GetEntities(MediaEntity.JobTemplate) as IJobTemplate[];
        //        //foreach (IJobTemplate jobTemplate in jobTemplates)
        //        //{
        //        //    jobTemplate.Delete();
        //        //}
        //        //IJob[] jobs = mediaClient.GetEntities(MediaEntity.Job) as IJob[];
        //        //foreach (IJob job in jobs)
        //        //{
        //        //    DeleteJob(job);
        //        //}
        //        //INotificationEndPoint[] notificationEndpoints = mediaClient.GetEntities(MediaEntity.NotificationEndpoint) as INotificationEndPoint[];
        //        //foreach (INotificationEndPoint notificationEndpoint in notificationEndpoints)
        //        //{
        //        //    if (notificationEndpoint.EndPointType == NotificationEndPointType.AzureTable)
        //        //    {
        //        //        IMonitoringConfiguration monitoringConfig = mediaClient.GetEntityById(MediaEntity.MonitoringConfiguration, notificationEndpoint.Id) as IMonitoringConfiguration;
        //        //        if (monitoringConfig != null)
        //        //        {
        //        //            monitoringConfig.Delete();
        //        //        }
        //        //    }
        //        //    notificationEndpoint.Delete();
        //        //}
        //        //IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset) as IAsset[];
        //        //foreach (IAsset asset in assets)
        //        //{
        //        //    DeleteAsset(mediaClient, asset);
        //        //}
        //        //IAccessPolicy[] accessPolicies = mediaClient.GetEntities(MediaEntity.AccessPolicy) as IAccessPolicy[];
        //        //foreach (IAccessPolicy accessPolicy in accessPolicies)
        //        //{
        //        //    accessPolicy.Delete();
        //        //}
        //        //IAssetDeliveryPolicy[] deliveryPolicies = mediaClient.GetEntities(MediaEntity.DeliveryPolicy) as IAssetDeliveryPolicy[];
        //        //foreach (IAssetDeliveryPolicy deliveryPolicy in deliveryPolicies)
        //        //{
        //        //    deliveryPolicy.Delete();
        //        //}
        //    //    IContentKeyAuthorizationPolicy[] contentKeyAuthPolicies = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
        //    //    foreach (IContentKeyAuthorizationPolicy contentKeyAuthPolicy in contentKeyAuthPolicies)
        //    //    {
        //    //        contentKeyAuthPolicy.Delete();
        //    //    }
        //    //    IContentKeyAuthorizationPolicyOption[] contentKeyAuthPolicyOptions = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
        //    //    foreach (IContentKeyAuthorizationPolicyOption contentKeyAuthPolicyOption in contentKeyAuthPolicyOptions)
        //    //    {
        //    //        contentKeyAuthPolicyOption.Delete();
        //    //    }
        //    //    IContentKey[] contentKeys = mediaClient.GetEntities(MediaEntity.ContentKey) as IContentKey[];
        //    //    foreach (IContentKey contentKey in contentKeys)
        //    //    {
        //    //        contentKey.Delete();
        //    //    }
        //    }
        //}

        public static string[][] GetEntityCounts(MediaClient mediaClient)
        {
            IPage<Asset> assets = mediaClient.GetEntities<Asset>(MediaEntity.Asset);
            int assetCount = mediaClient.GetEntityCount<Asset>(MediaEntity.Asset, assets);

            IPage<Transform> transforms = mediaClient.GetEntities<Transform>(MediaEntity.Transform);
            int transformCount = mediaClient.GetEntityCount<Transform>(MediaEntity.Transform, transforms);

            int transformsJobCount = 0;
            foreach (Transform transform in transforms)
            {
                IPage<Job> transformJobs = mediaClient.GetEntities<Job>(MediaEntity.TransformJob, transform.Name);
                int transformJobCount = mediaClient.GetEntityCount<Job>(MediaEntity.TransformJob, transformJobs);
                transformsJobCount = transformsJobCount + transformJobCount;
            }

            IPage<ContentKeyPolicy> contentKeyPolicies = mediaClient.GetEntities<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy);
            int contentKeyPolicyCount = mediaClient.GetEntityCount<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy, contentKeyPolicies);

            IPage<StreamingPolicy> streamingPolicies = mediaClient.GetEntities<StreamingPolicy>(MediaEntity.StreamingPolicy);
            int streamingPolicyCount = mediaClient.GetEntityCount<StreamingPolicy>(MediaEntity.StreamingPolicy, streamingPolicies);

            IPage<StreamingEndpoint> streamingEndpoints = mediaClient.GetEntities<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            int streamingEndpointCount = mediaClient.GetEntityCount<StreamingEndpoint>(MediaEntity.StreamingEndpoint, streamingEndpoints);

            IPage<StreamingLocator> streamingLocators = mediaClient.GetEntities<StreamingLocator>(MediaEntity.StreamingLocator);
            int streamingLocatorCount = mediaClient.GetEntityCount<StreamingLocator>(MediaEntity.StreamingLocator, streamingLocators);

            int streamingFiltersCount = 0;

            IPage<LiveEvent> liveEvents = mediaClient.GetEntities<LiveEvent>(MediaEntity.LiveEvent);
            int liveEventCount = mediaClient.GetEntityCount<LiveEvent>(MediaEntity.LiveEvent, liveEvents);

            int liveEventsOutputCount = 0;
            foreach (LiveEvent liveEvent in liveEvents)
            {
                IPage<LiveOutput> liveEventOutputs = mediaClient.GetEntities<LiveOutput>(MediaEntity.LiveOutput, liveEvent.Name);
                int liveEventOutputCount = mediaClient.GetEntityCount<LiveOutput>(MediaEntity.LiveOutput, liveEventOutputs);
                liveEventsOutputCount = liveEventsOutputCount + liveEventOutputCount;
            }

            List<string[]> entityCounts = new List<string[]>();
            entityCounts.Add(new string[] { "Storage Accounts", mediaClient.StorageAccounts.Count.ToString() });
            entityCounts.Add(new string[] { "Assets", assetCount.ToString(), "/account/assets" });
            entityCounts.Add(new string[] { "Transforms", transformCount.ToString(), "/account/transforms" });
            entityCounts.Add(new string[] { "Transform Jobs", transformsJobCount.ToString(), "/account/transformJobs" });
            entityCounts.Add(new string[] { "Content Key Policies", contentKeyPolicyCount.ToString(), "/account/contentKeyPolicies" });
            entityCounts.Add(new string[] { "Streaming Policies", streamingPolicyCount.ToString(), "/account/streamingPolicies" });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpointCount.ToString(), "/account/streamingEndpoints" });
            entityCounts.Add(new string[] { "Streaming Locators", streamingLocatorCount.ToString(), "/account/streamingLocators" });
            entityCounts.Add(new string[] { "Streaming Filters", streamingFiltersCount.ToString(), "/account/streamingFilters" });
            entityCounts.Add(new string[] { "Live Events", liveEventCount.ToString(), "/account/liveEvents" });
            entityCounts.Add(new string[] { "Live Event Outputs", liveEventsOutputCount.ToString(), "/account/liveEventOutputs" });

            return entityCounts.ToArray();
        }
    }
}