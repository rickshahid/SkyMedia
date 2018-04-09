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

        private static void DeleteAsset(MediaAccount mediaAccount, IAsset asset)
        {
            string documentId = Media.GetDocumentId(asset, out bool videoIndexer);
            if (!string.IsNullOrEmpty(documentId))
            {
                DatabaseClient databaseClient = new DatabaseClient();
                string collectionId = Constant.Database.Collection.MediaInsight;
                databaseClient.DeleteDocument(collectionId, documentId);
                if (videoIndexer)
                {
                    collectionId = Constant.Database.Collection.MediaPublish;
                    databaseClient.DeleteDocument(collectionId, documentId);
                    IndexerClient indexerClient = new IndexerClient(mediaAccount);
                    if (indexerClient.IndexerEnabled)
                    {
                        indexerClient.DeleteVideo(documentId, true);
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
                program.Delete();
                if (deleteAssets)
                {
                    DeleteAsset(mediaClient.MediaAccount, program.Asset);
                }
            }
            IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
            foreach (IChannel channel in channels)
            {
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

        public static void DeleteEntities(string authToken, bool allEntities, bool liveOnly)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            if (liveOnly)
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
                        DeleteAsset(mediaClient.MediaAccount, asset);
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
                    DeleteAsset(mediaClient.MediaAccount, asset);
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

        public static object GetContentKeys(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(MediaEntity.ContentKey) as IContentKey[];
        }

        public static object GetContentKeyAuthZPolicies(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
        }

        public static object GetContentKeyAuthZPolicyOptions(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
        }

        public static object GetNotificationEndpoints(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(MediaEntity.NotificationEndpoint) as INotificationEndPoint[];
        }

        public static string[][] GetEntityCounts(MediaClient mediaClient)
        {
            IStorageAccount[] storageAccounts = mediaClient.GetEntities(MediaEntity.StorageAccount) as IStorageAccount[];
            IContentKey[] contentKeys = mediaClient.GetEntities(MediaEntity.ContentKey) as IContentKey[];
            IContentKeyAuthorizationPolicy[] contentKeyAuthZPolicies = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
            IContentKeyAuthorizationPolicyOption[] contentKeyAuthZPolicyOptions = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
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
            entityCounts.Add(new string[] { "Content Key AuthZ Policies", contentKeyAuthZPolicies.Length.ToString(), "/account/contentKeyAuthZPolicies" });
            entityCounts.Add(new string[] { "Content Key AuthZ Policy Options", contentKeyAuthZPolicyOptions.Length.ToString(), "/account/contentKeyAuthZPolicyOptions" });
            entityCounts.Add(new string[] { "Ingest Manifests", manifests.Length.ToString() });
            entityCounts.Add(new string[] { "Ingest Manifest Assets", manifestAssets.Length.ToString() });
            entityCounts.Add(new string[] { "Ingest Manifest Files", manifestFiles.Length.ToString() });
            entityCounts.Add(new string[] { "Assets", assets.Length.ToString() });
            entityCounts.Add(new string[] { "Asset Files", files.Length.ToString() });
            entityCounts.Add(new string[] { "Media Processors", processors.Length.ToString(), "/account/mediaProcessors" });
            entityCounts.Add(new string[] { "Media Processor Units", GetProcessorUnitCount(processorUnits).ToString() });
            entityCounts.Add(new string[] { "Channels", channels.Length.ToString() });
            entityCounts.Add(new string[] { "Channel Programs", programs.Length.ToString() });
            entityCounts.Add(new string[] { "Job Templates", jobTemplates.Length.ToString() });
            entityCounts.Add(new string[] { "Jobs", jobs.Length.ToString() });
            entityCounts.Add(new string[] { "Notification Endpoints", notificationEndpoints.Length.ToString(), "/account/notificationEndpoints" });
            entityCounts.Add(new string[] { "Access Policies", accessPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Delivery Policies", deliveryPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpoints.Length.ToString() });
            entityCounts.Add(new string[] { "Streaming Units", GetStreamingUnitCount(streamingEndpoints).ToString() });
            entityCounts.Add(new string[] { "Streaming Filters", streamingFilters.Length.ToString() });
            entityCounts.Add(new string[] { "Locators", locators.Length.ToString() });
            return entityCounts.ToArray();
        }
    }
}