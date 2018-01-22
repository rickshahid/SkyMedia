using System;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public object GetEntities(MediaEntity entityType)
        {
            object entities = null;
            switch (entityType)
            {
                case MediaEntity.MonitoringConfiguration:
                    entities = (from x in _media.MonitoringConfigurations
                                select x).ToArray();
                    break;
                case MediaEntity.StorageAccount:
                    entities = (from x in _media.StorageAccounts
                                select x).ToArray();
                    break;
                case MediaEntity.ContentKey:
                    entities = (from x in _media.ContentKeys
                                select x).ToArray();
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    entities = (from x in _media.ContentKeyAuthorizationPolicies
                                select x).ToArray();
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    entities = (from x in _media.ContentKeyAuthorizationPolicyOptions
                                select x).ToArray();
                    break;
                case MediaEntity.Manifest:
                    entities = (from x in _media.IngestManifests
                                select x).ToArray();
                    break;
                case MediaEntity.ManifestAsset:
                    entities = (from x in _media.IngestManifestAssets
                                select x).ToArray();
                    break;
                case MediaEntity.ManifestFile:
                    entities = (from x in _media.IngestManifestFiles
                                select x).ToArray();
                    break;
                case MediaEntity.Asset:
                    entities = (from x in _media.Assets
                                select x).ToArray();
                    break;
                case MediaEntity.File:
                    entities = (from x in _media.Files
                                select x).ToArray();
                    break;
                case MediaEntity.Channel:
                    entities = (from x in _media.Channels
                                select x).ToArray();
                    break;
                case MediaEntity.Program:
                    entities = (from x in _media.Programs
                                select x).ToArray();
                    break;
                case MediaEntity.Processor:
                    entities = (from x in _media.MediaProcessors
                                select x).ToArray();
                    break;
                case MediaEntity.ProcessorUnit:
                    entities = (from x in _media.EncodingReservedUnits
                                select x).ToArray();
                    break;
                case MediaEntity.Job:
                    entities = (from x in _media.Jobs
                                select x).ToArray();
                    break;
                case MediaEntity.JobTemplate:
                    entities = (from x in _media.JobTemplates
                                select x).ToArray();
                    break;
                case MediaEntity.NotificationEndpoint:
                    entities = (from x in _media.NotificationEndPoints
                                select x).ToArray();
                    break;
                case MediaEntity.AccessPolicy:
                    entities = (from x in _media.AccessPolicies
                                select x).ToArray();
                    break;
                case MediaEntity.DeliveryPolicy:
                    entities = (from x in _media.AssetDeliveryPolicies
                                select x).ToArray();
                    break;
                case MediaEntity.StreamingEndpoint:
                    entities = (from x in _media.StreamingEndpoints
                                select x).ToArray();
                    break;
                case MediaEntity.StreamingFilter:
                    entities = (from x in _media.Filters
                                select x).ToArray();
                    break;
                case MediaEntity.Locator:
                    entities = (from x in _media.Locators
                                select x).ToArray();
                    break;
            }
            return entities;
        }

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

        public object GetEntityByName(MediaEntity entityType, string entityName, bool allEntities)
        {
            object entity = null;
            switch (entityType)
            {
                case MediaEntity.MonitoringConfiguration:
                    IQueryable<IMonitoringConfiguration> monitoringConfigs = _media.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Contains(entityName));
                    if (allEntities)
                    {
                        entity = monitoringConfigs.AsEnumerable();
                    }
                    else
                    {
                        entity = monitoringConfigs.SingleOrDefault();
                    }
                    break;
                case MediaEntity.StorageAccount:
                    IQueryable<IStorageAccount> storageAccounts = _media.StorageAccounts.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = storageAccounts.AsEnumerable();
                    }
                    else
                    {
                        entity = storageAccounts.SingleOrDefault();
                    }
                    break;
                case MediaEntity.ContentKey:
                    IQueryable<IContentKey> contentKeys = _media.ContentKeys.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = contentKeys.AsEnumerable();
                    }
                    else
                    {
                        entity = contentKeys.SingleOrDefault();
                    }
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    IQueryable<IContentKeyAuthorizationPolicy> authPolicies = _media.ContentKeyAuthorizationPolicies.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = authPolicies.AsEnumerable();
                    }
                    else
                    {
                        entity = authPolicies.SingleOrDefault();
                    }
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    IQueryable<IContentKeyAuthorizationPolicyOption> policyOptions = _media.ContentKeyAuthorizationPolicyOptions.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = policyOptions.AsEnumerable();
                    }
                    else
                    {
                        entity = policyOptions.SingleOrDefault();
                    }
                    break;
                case MediaEntity.Manifest:
                    IQueryable<IIngestManifest> manifests = _media.IngestManifests.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = manifests.AsEnumerable();
                    }
                    else
                    {
                        entity = manifests.SingleOrDefault();
                    }
                    break;
                case MediaEntity.ManifestAsset:
                    IQueryable<IIngestManifestAsset> manifestAssets = _media.IngestManifestAssets.Where(x => x.Asset.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = manifestAssets.AsEnumerable();
                    }
                    else
                    {
                        entity = manifestAssets.SingleOrDefault();
                    }
                    break;
                case MediaEntity.ManifestFile:
                    IQueryable<IIngestManifestFile> manifestFiles = _media.IngestManifestFiles.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = manifestFiles.AsEnumerable();
                    }
                    else
                    {
                        entity = manifestFiles.SingleOrDefault();
                    }
                    break;
                case MediaEntity.Asset:
                    IQueryable<IAsset> assets = _media.Assets.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = assets.AsEnumerable();
                    }
                    else
                    {
                        entity = assets.SingleOrDefault();
                    }
                    break;
                case MediaEntity.File:
                    IQueryable<IAssetFile> files = _media.Files.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = files.AsEnumerable();
                    }
                    else
                    {
                        entity = files.SingleOrDefault();
                    }
                    break;
                case MediaEntity.Channel:
                    IQueryable<IChannel> channels = _media.Channels.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = channels.AsEnumerable();
                    }
                    else
                    {
                        entity = channels.SingleOrDefault();
                    }
                    break;
                case MediaEntity.Program:
                    IQueryable<IProgram> programs = _media.Programs.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = programs.AsEnumerable();
                    }
                    else
                    {
                        entity = programs.SingleOrDefault();
                    }
                    break;
                case MediaEntity.Processor:
                    IQueryable<IMediaProcessor> processors = _media.MediaProcessors.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = processors.AsEnumerable();
                    }
                    else
                    {
                        entity = processors.SingleOrDefault();
                    }
                    break;
                case MediaEntity.ProcessorUnit:
                    IQueryable<IEncodingReservedUnit> reservedUnits = _media.EncodingReservedUnits.Where(x => x.ReservedUnitType.ToString().Contains(entityName));
                    if (allEntities)
                    {
                        entity = reservedUnits.AsEnumerable();
                    }
                    else
                    {
                        entity = reservedUnits.SingleOrDefault();
                    }
                    break;
                case MediaEntity.Job:
                    IQueryable<IJob> jobs = _media.Jobs.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = jobs.AsEnumerable();
                    }
                    else
                    {
                        entity = jobs.SingleOrDefault();
                    }
                    break;
                case MediaEntity.JobTemplate:
                    IQueryable<IJobTemplate> templates = _media.JobTemplates.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = templates.AsEnumerable();
                    }
                    else
                    {
                        entity = templates.SingleOrDefault();
                    }
                    break;
                case MediaEntity.NotificationEndpoint:
                    IQueryable<INotificationEndPoint> notifications = _media.NotificationEndPoints.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = notifications.AsEnumerable();
                    }
                    else
                    {
                        entity = notifications.SingleOrDefault();
                    }
                    break;
                case MediaEntity.AccessPolicy:
                    IQueryable<IAccessPolicy> accessPolicies = _media.AccessPolicies.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = accessPolicies.AsEnumerable();
                    }
                    else
                    {
                        entity = accessPolicies.SingleOrDefault();
                    }
                    break;
                case MediaEntity.DeliveryPolicy:
                    IQueryable<IAssetDeliveryPolicy> deliveryPolicies = _media.AssetDeliveryPolicies.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = deliveryPolicies.AsEnumerable();
                    }
                    else
                    {
                        entity = deliveryPolicies.SingleOrDefault();
                    }
                    break;
                case MediaEntity.StreamingEndpoint:
                    IQueryable<IStreamingEndpoint> streamingEndpoints = _media.StreamingEndpoints.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = streamingEndpoints.AsEnumerable();
                    }
                    else
                    {
                        entity = streamingEndpoints.SingleOrDefault();
                    }
                    break;
                case MediaEntity.StreamingFilter:
                    IQueryable<IStreamingFilter> streamingFilters = _media.Filters.Where(x => x.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = streamingFilters.AsEnumerable();
                    }
                    else
                    {
                        entity = streamingFilters.SingleOrDefault();
                    }
                    break;
                case MediaEntity.Locator:
                    IQueryable<ILocator> locators = _media.Locators.Where(x => x.Asset.Name.Contains(entityName));
                    if (allEntities)
                    {
                        entity = locators.AsEnumerable();
                    }
                    else
                    {
                        entity = locators.SingleOrDefault();
                    }
                    break;
            }
            return entity;
        }

        public object GetEntityByName(MediaEntity entityType, string entityName)
        {
            return GetEntityByName(entityType, entityName, false);
        }
    }
}