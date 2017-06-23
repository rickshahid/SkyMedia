using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private CloudMediaContext _media;

        public MediaClient(string authToken)
        {
            string attributeName = Constant.UserAttribute.MediaAccountName;
            string accountName = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.MediaAccountKey;
            string accountKey = AuthToken.GetClaimValue(authToken, attributeName);

            BindContext(accountName, accountKey);
        }

        public MediaClient(string accountName, string accountKey)
        {
            BindContext(accountName, accountKey);
        }

        private void BindContext(string accountName, string accountKey)
        {
            MediaServicesCredentials credentials = new MediaServicesCredentials(accountName, accountKey);
            _media = new CloudMediaContext(credentials);
            IStorageAccount storageAccount = this.DefaultStorageAccount;
        }

        public IStorageAccount DefaultStorageAccount
        {
            get { return _media.DefaultStorageAccount; }
        }

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
                    IMediaProcessor[] mediaEntities = entities as IMediaProcessor[];
                    List<IMediaProcessor> mediaProcessors = new List<IMediaProcessor>();
                    string[] processorIds = Processor.GetProcessorIds();
                    foreach (string processorId in processorIds)
                    {
                        foreach (IMediaProcessor mediaEntity in mediaEntities)
                        {
                            if (string.Equals(processorId, mediaEntity.Id, StringComparison.OrdinalIgnoreCase))
                            {
                                mediaProcessors.Add(mediaEntity);
                            }
                        }
                    }
                    entities = mediaProcessors.ToArray();
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

        public object GetEntityByName(MediaEntity entityType, string entityName, bool requireUnique)
        {
            object entity = null;
            switch (entityType)
            {
                case MediaEntity.MonitoringConfiguration:
                    IQueryable<IMonitoringConfiguration> monitoringConfigs = _media.MonitoringConfigurations.Where(x => x.NotificationEndPointId.Contains(entityName));
                    entity = requireUnique ? monitoringConfigs.SingleOrDefault() : monitoringConfigs.FirstOrDefault();
                    break;
                case MediaEntity.StorageAccount:
                    IQueryable<IStorageAccount> storageAccounts = _media.StorageAccounts.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? storageAccounts.SingleOrDefault() : storageAccounts.FirstOrDefault();
                    break;
                case MediaEntity.ContentKey:
                    IQueryable<IContentKey> contentKeys = _media.ContentKeys.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? contentKeys.SingleOrDefault() : contentKeys.FirstOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicy:
                    IQueryable<IContentKeyAuthorizationPolicy> authPolicies = _media.ContentKeyAuthorizationPolicies.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? authPolicies.SingleOrDefault() : authPolicies.FirstOrDefault();
                    break;
                case MediaEntity.ContentKeyAuthPolicyOption:
                    IQueryable<IContentKeyAuthorizationPolicyOption> policyOptions = _media.ContentKeyAuthorizationPolicyOptions.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? policyOptions.SingleOrDefault() : policyOptions.FirstOrDefault();
                    break;
                case MediaEntity.Manifest:
                    IQueryable<IIngestManifest> manifests = _media.IngestManifests.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? manifests.SingleOrDefault() : manifests.FirstOrDefault();
                    break;
                case MediaEntity.ManifestAsset:
                    IQueryable<IIngestManifestAsset> manifestAssets = _media.IngestManifestAssets.Where(x => x.Asset.Name.Contains(entityName));
                    entity = requireUnique ? manifestAssets.SingleOrDefault() : manifestAssets.FirstOrDefault();
                    break;
                case MediaEntity.ManifestFile:
                    IQueryable<IIngestManifestFile> manifestFiles = _media.IngestManifestFiles.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? manifestFiles.SingleOrDefault() : manifestFiles.FirstOrDefault();
                    break;
                case MediaEntity.Asset:
                    IQueryable<IAsset> assets = _media.Assets.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? assets.SingleOrDefault() : assets.FirstOrDefault();
                    break;
                case MediaEntity.File:
                    IQueryable<IAssetFile> files = _media.Files.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? files.SingleOrDefault() : files.FirstOrDefault();
                    break;
                case MediaEntity.Channel:
                    IQueryable<IChannel> channels = _media.Channels.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? channels.SingleOrDefault() : channels.FirstOrDefault();
                    break;
                case MediaEntity.Program:
                    IQueryable<IProgram> programs = _media.Programs.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? programs.SingleOrDefault() : programs.FirstOrDefault();
                    break;
                case MediaEntity.Processor:
                    IQueryable<IMediaProcessor> processors = _media.MediaProcessors.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? processors.SingleOrDefault() : processors.FirstOrDefault();
                    break;
                case MediaEntity.ProcessorUnit:
                    IQueryable<IEncodingReservedUnit> reservedUnits = _media.EncodingReservedUnits.Where(x => x.ReservedUnitType.ToString().Contains(entityName));
                    entity = requireUnique ? reservedUnits.SingleOrDefault() : reservedUnits.FirstOrDefault();
                    break;
                case MediaEntity.Job:
                    IQueryable<IJob> jobs = _media.Jobs.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? jobs.SingleOrDefault() : jobs.FirstOrDefault();
                    break;
                case MediaEntity.JobTemplate:
                    IQueryable<IJobTemplate> templates = _media.JobTemplates.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? templates.SingleOrDefault() : templates.FirstOrDefault();
                    break;
                case MediaEntity.NotificationEndpoint:
                    IQueryable<INotificationEndPoint> notifications = _media.NotificationEndPoints.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? notifications.SingleOrDefault() : notifications.FirstOrDefault();
                    break;
                case MediaEntity.AccessPolicy:
                    IQueryable<IAccessPolicy> accessPolicies = _media.AccessPolicies.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? accessPolicies.SingleOrDefault() : accessPolicies.FirstOrDefault();
                    break;
                case MediaEntity.DeliveryPolicy:
                    IQueryable<IAssetDeliveryPolicy> deliveryPolicies = _media.AssetDeliveryPolicies.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? deliveryPolicies.SingleOrDefault() : deliveryPolicies.FirstOrDefault();
                    break;
                case MediaEntity.StreamingEndpoint:
                    IQueryable<IStreamingEndpoint> streamingEndpoints = _media.StreamingEndpoints.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? streamingEndpoints.SingleOrDefault() : streamingEndpoints.FirstOrDefault();
                    break;
                case MediaEntity.StreamingFilter:
                    IQueryable<IStreamingFilter> streamingFilters = _media.Filters.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? streamingFilters.SingleOrDefault() : streamingFilters.FirstOrDefault();
                    break;
                case MediaEntity.Locator:
                    IQueryable<ILocator> locators = _media.Locators.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? locators.SingleOrDefault() : locators.FirstOrDefault();
                    break;
            }
            return entity;
        }
    }
}
