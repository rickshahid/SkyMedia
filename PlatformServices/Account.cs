using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Account
    {
        private static int GetProcessorUnitCount(IEncodingReservedUnit[] processorUnits)
        {
            int unitCount = 0;
            foreach (IEncodingReservedUnit processorUnit in processorUnits)
            {
                unitCount = unitCount + processorUnit.CurrentReservedUnits;
            }
            return unitCount;
        }

        private static int GetStreamingUnitCount(IStreamingEndpoint[] streamingEndpoints)
        {
            int unitCount = 0;
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.ScaleUnits.HasValue)
                {
                    unitCount = streamingEndpoint.ScaleUnits.Value;
                }
            }
            return unitCount;
        }

        private static void DeleteAsset(MediaClient mediaClient, IAsset asset)
        {
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.Equals(Constant.Media.MetadataManifest, StringComparison.OrdinalIgnoreCase))
                {
                    string manifestUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, asset, assetFile.Name, false);
                    Stream manifestStream = WebClient.GetStream(manifestUrl);
                    using (StreamReader manifestReader = new StreamReader(manifestStream))
                    {
                        while (!manifestReader.EndOfStream)
                        {
                            string mediaProcessor = manifestReader.ReadLine();
                            string[] mediaInsight = mediaProcessor.Split(Constant.TextDelimiter.Identifier);
                            bool videoIndexer = string.Equals(mediaInsight[0], MediaProcessor.VideoIndexer.ToString(), StringComparison.OrdinalIgnoreCase);
                            string documentId = mediaInsight[1];

                            DatabaseClient databaseClient = new DatabaseClient();
                            string collectionId = Constant.Database.Collection.MediaInsight;
                            databaseClient.DeleteDocument(collectionId, documentId);

                            if (videoIndexer)
                            {
                                collectionId = Constant.Database.Collection.MediaPublish;
                                databaseClient.DeleteDocument(collectionId, documentId);

                                IndexerClient indexerClient = new IndexerClient(mediaClient.MediaAccount);
                                indexerClient.DeleteVideo(documentId, true);

                                foreach (IAsset parentAsset in asset.ParentAssets)
                                {
                                    if (string.Equals(parentAsset.AlternateId, documentId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        parentAsset.AlternateId = string.Empty;
                                        parentAsset.Update();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (ILocator locator in asset.Locators)
            {
                locator.Delete();
            }
            for (int i = asset.DeliveryPolicies.Count - 1; i >= 0; i--)
            {
                asset.DeliveryPolicies.RemoveAt(i);
            }
            asset.Delete();
        }

        private static void DeleteLive(MediaClient mediaClient, bool deleteAssets)
        {
            IProgram[] programs = mediaClient.GetEntities(MediaEntity.Program) as IProgram[];
            foreach (IProgram program in programs)
            {
                if (program.State == ProgramState.Running)
                {
                    program.Stop();
                }
                if (deleteAssets)
                {
                    DeleteAsset(mediaClient, program.Asset);
                }
                program.Delete();
            }
            IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
            foreach (IChannel channel in channels)
            {
                if (channel.State == ChannelState.Running)
                {
                    channel.Stop();
                }
                channel.Delete();
            }
        }

        private static void DeleteJob(IJob job)
        {
            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constant.Database.Collection.MediaPublish;
            foreach (ITask jobTask in job.Tasks)
            {
                databaseClient.DeleteDocument(collectionId, jobTask.Id);
            }
            databaseClient.DeleteDocument(collectionId, job.Id);
            job.Delete();
        }

        public static void DeleteEntity(string authToken, MediaEntity entityType, string entityId)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            switch (entityType)
            {
                case MediaEntity.ContentKey:
                    IContentKey contentKey = mediaClient.GetEntityById(entityType, entityId) as IContentKey;
                    contentKey.Delete();
                    break;

                case MediaEntity.ContentKeyAuthPolicy:
                    IContentKeyAuthorizationPolicy contentKeyAuthPolicy = mediaClient.GetEntityById(entityType, entityId) as IContentKeyAuthorizationPolicy;
                    contentKeyAuthPolicy.Delete();
                    break;

                case MediaEntity.ContentKeyAuthPolicyOption:
                    IContentKeyAuthorizationPolicyOption contentKeyAuthPolicyOption = mediaClient.GetEntityById(entityType, entityId) as IContentKeyAuthorizationPolicyOption;
                    contentKeyAuthPolicyOption.Delete();
                    break;

                case MediaEntity.NotificationEndpoint:
                    INotificationEndPoint notificationEndpoint = mediaClient.GetEntityById(entityType, entityId) as INotificationEndPoint;
                    notificationEndpoint.Delete();
                    break;

                case MediaEntity.Asset:
                    IAsset asset = mediaClient.GetEntityById(entityType, entityId) as IAsset;
                    DeleteAsset(mediaClient, asset);
                    break;

                case MediaEntity.Job:
                    IJob job = mediaClient.GetEntityById(entityType, entityId) as IJob;
                    DeleteJob(job);
                    break;

                case MediaEntity.DeliveryPolicy:
                    IAssetDeliveryPolicy deliveryPolicy = mediaClient.GetEntityById(entityType, entityId) as IAssetDeliveryPolicy;
                    deliveryPolicy.Delete();
                    break;

                case MediaEntity.AccessPolicy:
                    IAccessPolicy accessPolicy = mediaClient.GetEntityById(entityType, entityId) as IAccessPolicy;
                    accessPolicy.Delete();
                    break;

                case MediaEntity.Locator:
                    ILocator locator = mediaClient.GetEntityById(entityType, entityId) as ILocator;
                    locator.Delete();
                    break;

                case MediaEntity.StreamingFilter:
                    IStreamingFilter filter = mediaClient.GetEntityById(entityType, entityId) as IStreamingFilter;
                    filter.Delete();
                    break;
            }
        }

        public static void DeleteEntities(string authToken, bool allEntities, bool liveChannels)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            if (liveChannels)
            {
                DeleteLive(mediaClient, false);
            }
            else if (!allEntities)
            {
                IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset) as IAsset[];
                foreach (IAsset asset in assets)
                {
                    if (asset.ParentAssets.Count > 0)
                    {
                        DeleteAsset(mediaClient, asset);
                    }
                }
            }
            else
            {
                DeleteLive(mediaClient, true);
                IIngestManifest[] manifests = mediaClient.GetEntities(MediaEntity.Manifest) as IIngestManifest[];
                foreach (IIngestManifest manifest in manifests)
                {
                    manifest.Delete();
                }
                IJobTemplate[] jobTemplates = mediaClient.GetEntities(MediaEntity.JobTemplate) as IJobTemplate[];
                foreach (IJobTemplate jobTemplate in jobTemplates)
                {
                    jobTemplate.Delete();
                }
                IJob[] jobs = mediaClient.GetEntities(MediaEntity.Job) as IJob[];
                foreach (IJob job in jobs)
                {
                    DeleteJob(job);
                }
                INotificationEndPoint[] notificationEndpoints = mediaClient.GetEntities(MediaEntity.NotificationEndpoint) as INotificationEndPoint[];
                foreach (INotificationEndPoint notificationEndpoint in notificationEndpoints)
                {
                    if (notificationEndpoint.EndPointType == NotificationEndPointType.AzureTable)
                    {
                        IMonitoringConfiguration monitoringConfig = mediaClient.GetEntityById(MediaEntity.MonitoringConfiguration, notificationEndpoint.Id) as IMonitoringConfiguration;
                        if (monitoringConfig != null)
                        {
                            monitoringConfig.Delete();
                        }
                    }
                    notificationEndpoint.Delete();
                }
                IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset) as IAsset[];
                foreach (IAsset asset in assets)
                {
                    DeleteAsset(mediaClient, asset);
                }
                IAccessPolicy[] accessPolicies = mediaClient.GetEntities(MediaEntity.AccessPolicy) as IAccessPolicy[];
                foreach (IAccessPolicy accessPolicy in accessPolicies)
                {
                    accessPolicy.Delete();
                }
                IAssetDeliveryPolicy[] deliveryPolicies = mediaClient.GetEntities(MediaEntity.DeliveryPolicy) as IAssetDeliveryPolicy[];
                foreach (IAssetDeliveryPolicy deliveryPolicy in deliveryPolicies)
                {
                    deliveryPolicy.Delete();
                }
                IContentKeyAuthorizationPolicy[] contentKeyAuthPolicies = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
                foreach (IContentKeyAuthorizationPolicy contentKeyAuthPolicy in contentKeyAuthPolicies)
                {
                    contentKeyAuthPolicy.Delete();
                }
                IContentKeyAuthorizationPolicyOption[] contentKeyAuthPolicyOptions = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
                foreach (IContentKeyAuthorizationPolicyOption contentKeyAuthPolicyOption in contentKeyAuthPolicyOptions)
                {
                    contentKeyAuthPolicyOption.Delete();
                }
                IContentKey[] contentKeys = mediaClient.GetEntities(MediaEntity.ContentKey) as IContentKey[];
                foreach (IContentKey contentKey in contentKeys)
                {
                    contentKey.Delete();
                }
            }
        }

        public static object GetEntities(string authToken, MediaEntity entityType)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(entityType);
        }

        public static string[][] GetEntityCounts(MediaClient mediaClient)
        {
            IStorageAccount[] storageAccounts = mediaClient.GetEntities(MediaEntity.StorageAccount) as IStorageAccount[];
            IContentKey[] contentKeys = mediaClient.GetEntities(MediaEntity.ContentKey) as IContentKey[];
            IContentKeyAuthorizationPolicy[] contentKeyAuthPolicies = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
            IContentKeyAuthorizationPolicyOption[] contentKeyAuthPolicyOptions = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
            IIngestManifest[] manifests = mediaClient.GetEntities(MediaEntity.Manifest) as IIngestManifest[];
            IIngestManifestAsset[] manifestAssets = mediaClient.GetEntities(MediaEntity.ManifestAsset) as IIngestManifestAsset[];
            IIngestManifestFile[] manifestFiles = mediaClient.GetEntities(MediaEntity.ManifestFile) as IIngestManifestFile[];
            IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset) as IAsset[];
            IAssetFile[] files = mediaClient.GetEntities(MediaEntity.File) as IAssetFile[];
            IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
            IProgram[] programs = mediaClient.GetEntities(MediaEntity.Program) as IProgram[];
            IMediaProcessor[] processors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
            IEncodingReservedUnit[] processorUnits = mediaClient.GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
            IJobTemplate[] jobTemplates = mediaClient.GetEntities(MediaEntity.JobTemplate) as IJobTemplate[];
            IJob[] jobs = mediaClient.GetEntities(MediaEntity.Job) as IJob[];
            INotificationEndPoint[] notificationEndpoints = mediaClient.GetEntities(MediaEntity.NotificationEndpoint) as INotificationEndPoint[];
            IAccessPolicy[] accessPolicies = mediaClient.GetEntities(MediaEntity.AccessPolicy) as IAccessPolicy[];
            IAssetDeliveryPolicy[] deliveryPolicies = mediaClient.GetEntities(MediaEntity.DeliveryPolicy) as IAssetDeliveryPolicy[];
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            IStreamingFilter[] streamingFilters = mediaClient.GetEntities(MediaEntity.StreamingFilter) as IStreamingFilter[];
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];

            List<string[]> entityCounts = new List<string[]>();
            entityCounts.Add(new string[] { "Storage Accounts", storageAccounts.Length.ToString() });
            entityCounts.Add(new string[] { "Content Keys", contentKeys.Length.ToString(), "/account/contentKeys" });
            entityCounts.Add(new string[] { "Content Key AuthZ Policies", contentKeyAuthPolicies.Length.ToString(), "/account/contentKeyAuthPolicies" });
            entityCounts.Add(new string[] { "Content Key AuthZ Policy Options", contentKeyAuthPolicyOptions.Length.ToString(), "/account/contentKeyAuthPolicyOptions" });
            entityCounts.Add(new string[] { "Ingest Manifests", manifests.Length.ToString() });
            entityCounts.Add(new string[] { "Ingest Manifest Assets", manifestAssets.Length.ToString() });
            entityCounts.Add(new string[] { "Ingest Manifest Files", manifestFiles.Length.ToString() });
            entityCounts.Add(new string[] { "Assets", assets.Length.ToString(), "/account/assets" });
            entityCounts.Add(new string[] { "Asset Files", files.Length.ToString() });
            entityCounts.Add(new string[] { "Media Processors", processors.Length.ToString(), "/account/mediaProcessors" });
            entityCounts.Add(new string[] { "Media Processor Units", GetProcessorUnitCount(processorUnits).ToString() });
            entityCounts.Add(new string[] { "Channels", channels.Length.ToString() });
            entityCounts.Add(new string[] { "Channel Programs", programs.Length.ToString() });
            entityCounts.Add(new string[] { "Job Templates", jobTemplates.Length.ToString() });
            entityCounts.Add(new string[] { "Jobs", jobs.Length.ToString(), "/account/jobs" });
            entityCounts.Add(new string[] { "Notification Endpoints", notificationEndpoints.Length.ToString(), "/account/notificationEndpoints" });
            entityCounts.Add(new string[] { "Access Policies", accessPolicies.Length.ToString(), "/account/accessPolicies" });
            entityCounts.Add(new string[] { "Delivery Policies", deliveryPolicies.Length.ToString(), "/account/deliveryPolicies" });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpoints.Length.ToString() });
            entityCounts.Add(new string[] { "Streaming Units", GetStreamingUnitCount(streamingEndpoints).ToString() });
            entityCounts.Add(new string[] { "Streaming Filters", streamingFilters.Length.ToString(), "/account/filters" });
            entityCounts.Add(new string[] { "Locators", locators.Length.ToString(), "/account/locators" });
            return entityCounts.ToArray();
        }
    }
}