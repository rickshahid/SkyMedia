using System;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public object GetEntityById(MediaEntity entityType, string entityId)
        {
            object entity = null;
            StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
            switch (entityType)
            {
                case MediaEntity.MonitoringConfiguration:
                    entity = _media2.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.StorageAccount:
                    entity = _media2.StorageAccounts.Where(x => x.Name.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ContentKey:
                    entity = _media2.ContentKeys.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    entity = _media2.ContentKeyAuthorizationPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    entity = _media2.ContentKeyAuthorizationPolicyOptions.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Manifest:
                    entity = _media2.IngestManifests.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ManifestAsset:
                    entity = _media2.IngestManifestAssets.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ManifestFile:
                    entity = _media2.IngestManifestFiles.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Asset:
                    entity = _media2.Assets.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.File:
                    entity = _media2.Files.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Channel:
                    entity = _media2.Channels.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Program:
                    entity = _media2.Programs.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Processor:
                    entity = _media2.MediaProcessors.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ProcessorUnit:
                    entity = _media2.EncodingReservedUnits.Where(x => x.AccountId.ToString().Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Job:
                    entity = _media2.Jobs.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.JobTemplate:
                    entity = _media2.JobTemplates.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.NotificationEndpoint:
                    entity = _media2.NotificationEndPoints.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.AccessPolicy:
                    entity = _media2.AccessPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.DeliveryPolicy:
                    entity = _media2.AssetDeliveryPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.StreamingEndpoint:
                    entity = _media2.StreamingEndpoints.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.StreamingLocator:
                    entity = _media2.Locators.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.StreamingFilter:
                    entity = _media2.Filters.Where(x => x.Name.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
            }
            return entity;
        }

        public object GetEntityByName(MediaEntity entityType, string entityName)
        {
            object entity = null;
            switch (entityType)
            {
                case MediaEntity.MonitoringConfiguration:
                    IQueryable<IMonitoringConfiguration> monitoringConfigs = _media2.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Contains(entityName));
                    entity = monitoringConfigs.SingleOrDefault();
                    break;
                case MediaEntity.StorageAccount:
                    IQueryable<IStorageAccount> storageAccounts = _media2.StorageAccounts.Where(x => x.Name.Contains(entityName));
                    entity = storageAccounts.SingleOrDefault();
                    break;
                case MediaEntity.ContentKey:
                    IQueryable<IContentKey> contentKeys = _media2.ContentKeys.Where(x => x.Name.Contains(entityName));
                    entity = contentKeys.SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    IQueryable<IContentKeyAuthorizationPolicy> authPolicies = _media2.ContentKeyAuthorizationPolicies.Where(x => x.Name.Contains(entityName));
                    entity = authPolicies.SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    IQueryable<IContentKeyAuthorizationPolicyOption> policyOptions = _media2.ContentKeyAuthorizationPolicyOptions.Where(x => x.Name.Contains(entityName));
                    entity = policyOptions.SingleOrDefault();
                    break;
                case MediaEntity.Manifest:
                    IQueryable<IIngestManifest> manifests = _media2.IngestManifests.Where(x => x.Name.Contains(entityName));
                    entity = manifests.SingleOrDefault();
                    break;
                case MediaEntity.ManifestAsset:
                    IQueryable<IIngestManifestAsset> manifestAssets = _media2.IngestManifestAssets.Where(x => x.Asset.Name.Contains(entityName));
                    entity = manifestAssets.SingleOrDefault();
                    break;
                case MediaEntity.ManifestFile:
                    IQueryable<IIngestManifestFile> manifestFiles = _media2.IngestManifestFiles.Where(x => x.Name.Contains(entityName));
                    entity = manifestFiles.SingleOrDefault();
                    break;
                case MediaEntity.Asset:
                    IQueryable<IAsset> assets = _media2.Assets.Where(x => x.Name.Contains(entityName));
                    entity = assets.SingleOrDefault();
                    break;
                case MediaEntity.File:
                    IQueryable<IAssetFile> files = _media2.Files.Where(x => x.Name.Contains(entityName));
                    entity = files.SingleOrDefault();
                    break;
                case MediaEntity.Channel:
                    IQueryable<IChannel> channels = _media2.Channels.Where(x => x.Name.Contains(entityName));
                    entity = channels.SingleOrDefault();
                    break;
                case MediaEntity.Program:
                    IQueryable<IProgram> programs = _media2.Programs.Where(x => x.Name.Contains(entityName));
                    entity = programs.SingleOrDefault();
                    break;
                case MediaEntity.Processor:
                    IQueryable<IMediaProcessor> processors = _media2.MediaProcessors.Where(x => x.Name.Contains(entityName));
                    entity = processors.SingleOrDefault();
                    break;
                case MediaEntity.ProcessorUnit:
                    IQueryable<IEncodingReservedUnit> reservedUnits = _media2.EncodingReservedUnits.Where(x => x.ReservedUnitType.ToString().Contains(entityName));
                    entity = reservedUnits.SingleOrDefault();
                    break;
                case MediaEntity.Job:
                    IQueryable<IJob> jobs = _media2.Jobs.Where(x => x.Name.Contains(entityName));
                    entity = jobs.SingleOrDefault();
                    break;
                case MediaEntity.JobTemplate:
                    IQueryable<IJobTemplate> templates = _media2.JobTemplates.Where(x => x.Name.Contains(entityName));
                    entity = templates.SingleOrDefault();
                    break;
                case MediaEntity.NotificationEndpoint:
                    IQueryable<INotificationEndPoint> notifications = _media2.NotificationEndPoints.Where(x => x.Name.Contains(entityName));
                    entity = notifications.SingleOrDefault();
                    break;
                case MediaEntity.AccessPolicy:
                    IQueryable<IAccessPolicy> accessPolicies = _media2.AccessPolicies.Where(x => x.Name.Contains(entityName));
                    entity = accessPolicies.SingleOrDefault();
                    break;
                case MediaEntity.DeliveryPolicy:
                    IQueryable<IAssetDeliveryPolicy> deliveryPolicies = _media2.AssetDeliveryPolicies.Where(x => x.Name.Contains(entityName));
                    entity = deliveryPolicies.SingleOrDefault();
                    break;
                case MediaEntity.StreamingEndpoint:
                    IQueryable<IStreamingEndpoint> streamingEndpoints = _media2.StreamingEndpoints.Where(x => x.Name.Contains(entityName));
                    entity = streamingEndpoints.SingleOrDefault();
                    break;
                case MediaEntity.StreamingLocator:
                    IQueryable<ILocator> locators = _media2.Locators.Where(x => x.Asset.Name.Contains(entityName));
                    entity = locators.SingleOrDefault();
                    break;
                case MediaEntity.StreamingFilter:
                    IQueryable<IStreamingFilter> streamingFilters = _media2.Filters.Where(x => x.Name.Contains(entityName));
                    entity = streamingFilters.SingleOrDefault();
                    break;
            }
            return entity;
        }

        public object[] GetEntities(MediaEntity entityType, string entityName)
        {
            object[] entities = null;
            switch (entityType)
            {
                case MediaEntity.MonitoringConfiguration:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.MonitoringConfigurations.ToArray();
                    }
                    else
                    {
                        entities = _media2.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.StorageAccount:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.StorageAccounts.ToArray();
                    }
                    else
                    {
                        entities = _media2.StorageAccounts.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ContentKey:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.ContentKeys.ToArray();
                    }
                    else
                    {
                        entities = _media2.ContentKeys.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.ContentKeyAuthorizationPolicies.ToArray();
                    }
                    else
                    {
                        entities = _media2.ContentKeyAuthorizationPolicies.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.ContentKeyAuthorizationPolicyOptions.ToArray();
                    }
                    else
                    {
                        entities = _media2.ContentKeyAuthorizationPolicyOptions.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Manifest:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.IngestManifests.ToArray();
                    }
                    else
                    {
                        entities = _media2.IngestManifests.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ManifestAsset:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.IngestManifestAssets.ToArray();
                    }
                    else
                    {
                        entities = _media2.IngestManifestAssets.Where(x => x.Asset.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ManifestFile:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.IngestManifestFiles.ToArray();
                    }
                    else
                    {
                        entities = _media2.IngestManifestFiles.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Asset:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.Assets.ToArray();
                    }
                    else
                    {
                        entities = _media2.Assets.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.File:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.Files.ToArray();
                    }
                    else
                    {
                        entities = _media2.Files.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Channel:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.Channels.ToArray();
                    }
                    else
                    {
                        entities = _media2.Channels.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Program:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.Programs.ToArray();
                    }
                    else
                    {
                        entities = _media2.Programs.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Processor:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.MediaProcessors.ToArray();
                    }
                    else
                    {
                        entities = _media2.MediaProcessors.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ProcessorUnit:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.EncodingReservedUnits.ToArray();
                    }
                    else
                    {
                        entities = _media2.EncodingReservedUnits.Where(x => x.ReservedUnitType.ToString().Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Job:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.Jobs.ToArray();
                    }
                    else
                    {
                        entities = _media2.Jobs.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.JobTemplate:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.JobTemplates.ToArray();
                    }
                    else
                    {
                        entities = _media2.JobTemplates.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.NotificationEndpoint:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.NotificationEndPoints.ToArray();
                    }
                    else
                    {
                        entities = _media2.NotificationEndPoints.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.AccessPolicy:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.AccessPolicies.ToArray();
                    }
                    else
                    {
                        entities = _media2.AccessPolicies.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.DeliveryPolicy:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.AssetDeliveryPolicies.ToArray();
                    }
                    else
                    {
                        entities = _media2.AssetDeliveryPolicies.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.StreamingEndpoint:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.StreamingEndpoints.ToArray();
                    }
                    else
                    {
                        entities = _media2.StreamingEndpoints.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.StreamingLocator:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.Locators.ToArray();
                    }
                    else
                    {
                        entities = _media2.Locators.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.StreamingFilter:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media2.Filters.ToArray();
                    }
                    else
                    {
                        entities = _media2.Filters.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
            }
            return entities;
        }

        public object[] GetEntities(MediaEntity entityType)
        {
            return GetEntities(entityType, null);
        }
    }
}