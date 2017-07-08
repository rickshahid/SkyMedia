using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    public static class Account
    {
        private static string GetProcessorUnitCount(IEncodingReservedUnit[] processorUnits)
        {
            int unitCount = 0;
            foreach (IEncodingReservedUnit processorUnit in processorUnits)
            {
                unitCount = unitCount + processorUnit.CurrentReservedUnits;
            }
            return unitCount.ToString();
        }

        private static string GetStreamingUnitCount(IStreamingEndpoint[] streamingEndpoints)
        {
            int unitCount = 0;
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                unitCount = streamingEndpoint.ScaleUnits.Value;
            }
            return unitCount.ToString();
        }

        private static void DeleteAsset(MediaClient mediaClient, IAsset asset, string indexerKey)
        {
            if (!string.IsNullOrEmpty(indexerKey) && !string.IsNullOrEmpty(asset.AlternateId))
            {
                IndexerClient indexerClient = new IndexerClient(indexerKey);
                indexerClient.DeleteVideo(asset.AlternateId, true);
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

        public static object GetNotificationEndpoints(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(MediaEntity.NotificationEndpoint) as INotificationEndPoint[];
        }

        public static string[][] GetEntityCounts(MediaClient mediaClient)
        {
            IStorageAccount[] storageAccounts = mediaClient.GetEntities(MediaEntity.StorageAccount) as IStorageAccount[];
            IContentKey[] contentKeys = mediaClient.GetEntities(MediaEntity.ContentKey) as IContentKey[];
            IContentKeyAuthorizationPolicy[] contentKeyPolicies = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
            IContentKeyAuthorizationPolicyOption[] contentKeyPolicyOptions = mediaClient.GetEntities(MediaEntity.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
            IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
            IIngestManifest[] manifests = mediaClient.GetEntities(MediaEntity.Manifest) as IIngestManifest[];
            IIngestManifestAsset[] manifestAssets = mediaClient.GetEntities(MediaEntity.ManifestAsset) as IIngestManifestAsset[];
            IIngestManifestFile[] manifestFiles = mediaClient.GetEntities(MediaEntity.ManifestFile) as IIngestManifestFile[];
            IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset) as IAsset[];
            IAssetFile[] files = mediaClient.GetEntities(MediaEntity.File) as IAssetFile[];
            IEncodingReservedUnit[] processorUnits = mediaClient.GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
            MediaProcessor?[] processors = mediaClient.GetEntities(MediaEntity.Processor) as MediaProcessor?[];
            IProgram[] programs = mediaClient.GetEntities(MediaEntity.Program) as IProgram[];
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
            entityCounts.Add(new string[] { "Content Keys", contentKeys.Length.ToString() });
            entityCounts.Add(new string[] { "Content Key Policies", contentKeyPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Content Key Policy Options", contentKeyPolicyOptions.Length.ToString() });
            entityCounts.Add(new string[] { "Channels", channels.Length.ToString() });
            entityCounts.Add(new string[] { "Ingest Manifests", manifests.Length.ToString() });
            entityCounts.Add(new string[] { "Ingest Manifest Assets", manifestAssets.Length.ToString() });
            entityCounts.Add(new string[] { "Ingest Manifest Files", manifestFiles.Length.ToString() });
            entityCounts.Add(new string[] { "Assets", assets.Length.ToString() });
            entityCounts.Add(new string[] { "Asset Files", files.Length.ToString() });
            entityCounts.Add(new string[] { "Processor Units", GetProcessorUnitCount(processorUnits) });
            entityCounts.Add(new string[] { "Media Processors", processors.Length.ToString(), "/account/processors" });
            entityCounts.Add(new string[] { "Channel Programs", programs.Length.ToString() });
            entityCounts.Add(new string[] { "Job Templates", jobTemplates.Length.ToString() });
            entityCounts.Add(new string[] { "Jobs", jobs.Length.ToString() });
            entityCounts.Add(new string[] { "Notification Endpoints", notificationEndpoints.Length.ToString(), "/account/endpoints" });
            entityCounts.Add(new string[] { "Access Policies", accessPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Delivery Policies", deliveryPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpoints.Length.ToString() });
            entityCounts.Add(new string[] { "Streaming Units", GetStreamingUnitCount(streamingEndpoints) });
            entityCounts.Add(new string[] { "Streaming Filters", streamingFilters.Length.ToString() });
            entityCounts.Add(new string[] { "Locators", locators.Length.ToString() });
            return entityCounts.ToArray();
        }

        public static void ClearEntities(MediaClient mediaClient, bool allEntities, string indexerKey)
        {
            if (allEntities)
            {
                IProgram[] programs = mediaClient.GetEntities(MediaEntity.Program) as IProgram[];
                foreach (IProgram program in programs)
                {
                    program.Delete();
                }
                IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
                foreach (IChannel channel in channels)
                {
                    channel.Delete();
                }
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
            }
            IJob[] jobs = mediaClient.GetEntities(MediaEntity.Job) as IJob[];
            foreach (IJob job in jobs)
            {
                job.Delete();
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
                if (asset.ParentAssets.Count > 0 || allEntities)
                {
                    DeleteAsset(mediaClient, asset, indexerKey);
                }
            }
            if (allEntities)
            {
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
    }
}
