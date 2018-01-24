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
                    entity = _media.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.StorageAccount:
                    entity = _media.StorageAccounts.Where(x => x.Name.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ContentKey:
                    entity = _media.ContentKeys.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    entity = _media.ContentKeyAuthorizationPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    entity = _media.ContentKeyAuthorizationPolicyOptions.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Manifest:
                    entity = _media.IngestManifests.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ManifestAsset:
                    entity = _media.IngestManifestAssets.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ManifestFile:
                    entity = _media.IngestManifestFiles.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Asset:
                    entity = _media.Assets.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.File:
                    entity = _media.Files.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Channel:
                    entity = _media.Channels.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Program:
                    entity = _media.Programs.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Processor:
                    entity = _media.MediaProcessors.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.ProcessorUnit:
                    entity = _media.EncodingReservedUnits.Where(x => x.AccountId.ToString().Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Job:
                    entity = _media.Jobs.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.JobTemplate:
                    entity = _media.JobTemplates.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.NotificationEndpoint:
                    entity = _media.NotificationEndPoints.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.AccessPolicy:
                    entity = _media.AccessPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.DeliveryPolicy:
                    entity = _media.AssetDeliveryPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.StreamingEndpoint:
                    entity = _media.StreamingEndpoints.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.StreamingFilter:
                    entity = _media.Filters.Where(x => x.Name.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case MediaEntity.Locator:
                    entity = _media.Locators.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
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
                    IQueryable<IMonitoringConfiguration> monitoringConfigs = _media.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Contains(entityName));
                    entity = monitoringConfigs.SingleOrDefault();
                    break;
                case MediaEntity.StorageAccount:
                    IQueryable<IStorageAccount> storageAccounts = _media.StorageAccounts.Where(x => x.Name.Contains(entityName));
                    entity = storageAccounts.SingleOrDefault();
                    break;
                case MediaEntity.ContentKey:
                    IQueryable<IContentKey> contentKeys = _media.ContentKeys.Where(x => x.Name.Contains(entityName));
                    entity = contentKeys.SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    IQueryable<IContentKeyAuthorizationPolicy> authPolicies = _media.ContentKeyAuthorizationPolicies.Where(x => x.Name.Contains(entityName));
                    entity = authPolicies.SingleOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    IQueryable<IContentKeyAuthorizationPolicyOption> policyOptions = _media.ContentKeyAuthorizationPolicyOptions.Where(x => x.Name.Contains(entityName));
                    entity = policyOptions.SingleOrDefault();
                    break;
                case MediaEntity.Manifest:
                    IQueryable<IIngestManifest> manifests = _media.IngestManifests.Where(x => x.Name.Contains(entityName));
                    entity = manifests.SingleOrDefault();
                    break;
                case MediaEntity.ManifestAsset:
                    IQueryable<IIngestManifestAsset> manifestAssets = _media.IngestManifestAssets.Where(x => x.Asset.Name.Contains(entityName));
                    entity = manifestAssets.SingleOrDefault();
                    break;
                case MediaEntity.ManifestFile:
                    IQueryable<IIngestManifestFile> manifestFiles = _media.IngestManifestFiles.Where(x => x.Name.Contains(entityName));
                    entity = manifestFiles.SingleOrDefault();
                    break;
                case MediaEntity.Asset:
                    IQueryable<IAsset> assets = _media.Assets.Where(x => x.Name.Contains(entityName));
                    entity = assets.SingleOrDefault();
                    break;
                case MediaEntity.File:
                    IQueryable<IAssetFile> files = _media.Files.Where(x => x.Name.Contains(entityName));
                    entity = files.SingleOrDefault();
                    break;
                case MediaEntity.Channel:
                    IQueryable<IChannel> channels = _media.Channels.Where(x => x.Name.Contains(entityName));
                    entity = channels.SingleOrDefault();
                    break;
                case MediaEntity.Program:
                    IQueryable<IProgram> programs = _media.Programs.Where(x => x.Name.Contains(entityName));
                    entity = programs.SingleOrDefault();
                    break;
                case MediaEntity.Processor:
                    IQueryable<IMediaProcessor> processors = _media.MediaProcessors.Where(x => x.Name.Contains(entityName));
                    entity = processors.SingleOrDefault();
                    break;
                case MediaEntity.ProcessorUnit:
                    IQueryable<IEncodingReservedUnit> reservedUnits = _media.EncodingReservedUnits.Where(x => x.ReservedUnitType.ToString().Contains(entityName));
                    entity = reservedUnits.SingleOrDefault();
                    break;
                case MediaEntity.Job:
                    IQueryable<IJob> jobs = _media.Jobs.Where(x => x.Name.Contains(entityName));
                    entity = jobs.SingleOrDefault();
                    break;
                case MediaEntity.JobTemplate:
                    IQueryable<IJobTemplate> templates = _media.JobTemplates.Where(x => x.Name.Contains(entityName));
                    entity = templates.SingleOrDefault();
                    break;
                case MediaEntity.NotificationEndpoint:
                    IQueryable<INotificationEndPoint> notifications = _media.NotificationEndPoints.Where(x => x.Name.Contains(entityName));
                    entity = notifications.SingleOrDefault();
                    break;
                case MediaEntity.AccessPolicy:
                    IQueryable<IAccessPolicy> accessPolicies = _media.AccessPolicies.Where(x => x.Name.Contains(entityName));
                    entity = accessPolicies.SingleOrDefault();
                    break;
                case MediaEntity.DeliveryPolicy:
                    IQueryable<IAssetDeliveryPolicy> deliveryPolicies = _media.AssetDeliveryPolicies.Where(x => x.Name.Contains(entityName));
                    entity = deliveryPolicies.SingleOrDefault();
                    break;
                case MediaEntity.StreamingEndpoint:
                    IQueryable<IStreamingEndpoint> streamingEndpoints = _media.StreamingEndpoints.Where(x => x.Name.Contains(entityName));
                    entity = streamingEndpoints.SingleOrDefault();
                    break;
                case MediaEntity.StreamingFilter:
                    IQueryable<IStreamingFilter> streamingFilters = _media.Filters.Where(x => x.Name.Contains(entityName));
                    entity = streamingFilters.SingleOrDefault();
                    break;
                case MediaEntity.Locator:
                    IQueryable<ILocator> locators = _media.Locators.Where(x => x.Asset.Name.Contains(entityName));
                    entity = locators.SingleOrDefault();
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
                        entities = _media.MonitoringConfigurations.ToArray();
                    }
                    else
                    {
                        entities = _media.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.StorageAccount:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.StorageAccounts.ToArray();
                    }
                    else
                    {
                        entities = _media.StorageAccounts.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ContentKey:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.ContentKeys.ToArray();
                    }
                    else
                    {
                        entities = _media.ContentKeys.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.ContentKeyAuthorizationPolicies.ToArray();
                    }
                    else
                    {
                        entities = _media.ContentKeyAuthorizationPolicies.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.ContentKeyAuthorizationPolicyOptions.ToArray();
                    }
                    else
                    {
                        entities = _media.ContentKeyAuthorizationPolicyOptions.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Manifest:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.IngestManifests.ToArray();
                    }
                    else
                    {
                        entities = _media.IngestManifests.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ManifestAsset:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.IngestManifestAssets.ToArray();
                    }
                    else
                    {
                        entities = _media.IngestManifestAssets.Where(x => x.Asset.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ManifestFile:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.IngestManifestFiles.ToArray();
                    }
                    else
                    {
                        entities = _media.IngestManifestFiles.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Asset:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.Assets.ToArray();
                    }
                    else
                    {
                        entities = _media.Assets.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.File:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.Files.ToArray();
                    }
                    else
                    {
                        entities = _media.Files.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Channel:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.Channels.ToArray();
                    }
                    else
                    {
                        entities = _media.Channels.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Program:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.Programs.ToArray();
                    }
                    else
                    {
                        entities = _media.Programs.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Processor:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.MediaProcessors.ToArray();
                    }
                    else
                    {
                        entities = _media.MediaProcessors.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.ProcessorUnit:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.EncodingReservedUnits.ToArray();
                    }
                    else
                    {
                        entities = _media.EncodingReservedUnits.Where(x => x.ReservedUnitType.ToString().Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Job:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.Jobs.ToArray();
                    }
                    else
                    {
                        entities = _media.Jobs.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.JobTemplate:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.JobTemplates.ToArray();
                    }
                    else
                    {
                        entities = _media.JobTemplates.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.NotificationEndpoint:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.NotificationEndPoints.ToArray();
                    }
                    else
                    {
                        entities = _media.NotificationEndPoints.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.AccessPolicy:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.AccessPolicies.ToArray();
                    }
                    else
                    {
                        entities = _media.AccessPolicies.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.DeliveryPolicy:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.AssetDeliveryPolicies.ToArray();
                    }
                    else
                    {
                        entities = _media.AssetDeliveryPolicies.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.StreamingEndpoint:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.StreamingEndpoints.ToArray();
                    }
                    else
                    {
                        entities = _media.StreamingEndpoints.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.StreamingFilter:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.Filters.ToArray();
                    }
                    else
                    {
                        entities = _media.Filters.Where(x => x.Name.Contains(entityName)).ToArray();
                    }
                    break;
                case MediaEntity.Locator:
                    if (string.IsNullOrEmpty(entityName))
                    {
                        entities = _media.Locators.ToArray();
                    }
                    else
                    {
                        entities = _media.Locators.Where(x => x.Name.Contains(entityName)).ToArray();
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