using System;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private CloudMediaContext _media;

        public MediaClient(string authToken)
        {
            string attributeName = Constants.UserAttribute.MediaAccountName;
            string accountName = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constants.UserAttribute.MediaAccountKey;
            string accountKey = AuthToken.GetClaimValue(authToken, attributeName);

            BindContext(accountName, accountKey);
        }

        public MediaClient(string[] mediaAccount)
        {
            string accountName = mediaAccount[0];
            string accountKey = mediaAccount[1];
            BindContext(accountName, accountKey);
        }

        public MediaClient(ContentPublish contentPublish)
        {
            string accountName = contentPublish.PartitionKey;
            string accountKey = contentPublish.MediaAccountKey;
            BindContext(accountName, accountKey);
        }

        private void BindContext(string accountName, string accountKey)
        {
            MediaServicesCredentials credentials = new MediaServicesCredentials(accountName, accountKey);
            _media = new CloudMediaContext(credentials);

            int settingValue;
            string settingKey = Constants.AppSettings.MediaConcurrentTransferCount;
            string concurrentTransferCount = AppSetting.GetValue(settingKey);
            if (int.TryParse(concurrentTransferCount, out settingValue))
            {
                _media.NumberOfConcurrentTransfers = settingValue;
            }

            settingKey = Constants.AppSettings.MediaParallelTransferThreadCount;
            string parallelTransferThreadCount = AppSetting.GetValue(settingKey);
            if (int.TryParse(parallelTransferThreadCount, out settingValue))
            {
                _media.ParallelTransferThreadCount = settingValue;
            }

            IStorageAccount storageAccount = _media.DefaultStorageAccount;
        }

        public object GetEntities(EntityType entityType)
        {
            object entities = null;
            switch (entityType)
            {
                case EntityType.StorageAccount:
                    entities = (from x in _media.StorageAccounts
                                select x).ToArray();
                    break;
                case EntityType.ContentKey:
                    entities = (from x in _media.ContentKeys
                                select x).ToArray();
                    break;
                case EntityType.ContentKeyAuthPolicy:
                    entities = (from x in _media.ContentKeyAuthorizationPolicies
                                select x).ToArray();
                    break;
                case EntityType.ContentKeyAuthPolicyOption:
                    entities = (from x in _media.ContentKeyAuthorizationPolicyOptions
                                select x).ToArray();
                    break;
                case EntityType.Manifest:
                    entities = (from x in _media.IngestManifests
                                select x).ToArray();
                    break;
                case EntityType.ManifestAsset:
                    entities = (from x in _media.IngestManifestAssets
                                select x).ToArray();
                    break;
                case EntityType.ManifestFile:
                    entities = (from x in _media.IngestManifestFiles
                                select x).ToArray();
                    break;
                case EntityType.Asset:
                    entities = (from x in _media.Assets
                                select x).ToArray();
                    break;
                case EntityType.File:
                    entities = (from x in _media.Files
                                select x).ToArray();
                    break;
                case EntityType.Channel:
                    entities = (from x in _media.Channels
                                select x).ToArray();
                    break;
                case EntityType.Program:
                    entities = (from x in _media.Programs
                                select x).ToArray();
                    break;
                case EntityType.ReservedUnit:
                    entities = (from x in _media.EncodingReservedUnits
                                select x).ToArray();
                    break;
                case EntityType.Processor:
                    entities = (from x in _media.MediaProcessors
                                select x).ToArray();
                    break;
                case EntityType.JobTemplate:
                    entities = (from x in _media.JobTemplates
                                select x).ToArray();
                    break;
                case EntityType.Job:
                    entities = (from x in _media.Jobs
                                select x).ToArray();
                    break;
                case EntityType.NotificationEndpoint:
                    entities = (from x in _media.NotificationEndPoints
                                select x).ToArray();
                    break;
                case EntityType.AccessPolicy:
                    entities = (from x in _media.AccessPolicies
                                select x).ToArray();
                    break;
                case EntityType.DeliveryPolicy:
                    entities = (from x in _media.AssetDeliveryPolicies
                                select x).ToArray();
                    break;
                case EntityType.StreamingEndpoint:
                    entities = (from x in _media.StreamingEndpoints
                                select x).ToArray();
                    break;
                case EntityType.StreamingFilter:
                    entities = (from x in _media.Filters
                                select x).ToArray();
                    break;
                case EntityType.Locator:
                    entities = (from x in _media.Locators
                                select x).ToArray();
                    break;
            }
            return entities;
        }

        public object GetEntityById(EntityType entityType, string entityId)
        {
            object entity = null;
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
            switch (entityType)
            {
                case EntityType.StorageAccount:
                    entity = _media.StorageAccounts.Where(x => x.Name.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.ContentKey:
                    entity = _media.ContentKeys.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.ContentKeyAuthPolicy:
                    entity = _media.ContentKeyAuthorizationPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.ContentKeyAuthPolicyOption:
                    entity = _media.ContentKeyAuthorizationPolicyOptions.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.Manifest:
                    entity = _media.IngestManifests.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.ManifestAsset:
                    entity = _media.IngestManifestAssets.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.ManifestFile:
                    entity = _media.IngestManifestFiles.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.Asset:
                    entity = _media.Assets.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.File:
                    entity = _media.Files.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.Channel:
                    entity = _media.Channels.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.Program:
                    entity = _media.Programs.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.ReservedUnit:
                    entity = _media.EncodingReservedUnits.Where(x => x.AccountId.ToString().Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.Processor:
                    entity = _media.MediaProcessors.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.JobTemplate:
                    entity = _media.JobTemplates.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.Job:
                    entity = _media.Jobs.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.NotificationEndpoint:
                    entity = _media.NotificationEndPoints.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.AccessPolicy:
                    entity = _media.AccessPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.DeliveryPolicy:
                    entity = _media.AssetDeliveryPolicies.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.StreamingEndpoint:
                    entity = _media.StreamingEndpoints.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.StreamingFilter:
                    entity = _media.Filters.Where(x => x.Name.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
                case EntityType.Locator:
                    entity = _media.Locators.Where(x => x.Id.Equals(entityId, comparisonType)).SingleOrDefault();
                    break;
            }
            return entity;
        }

        public object GetEntityByName(EntityType entityType, string entityName, bool requireUnique)
        {
            object entity = null;
            switch (entityType)
            {
                case EntityType.StorageAccount:
                    IQueryable<IStorageAccount> storageAccounts = _media.StorageAccounts.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? storageAccounts.SingleOrDefault() : storageAccounts.FirstOrDefault();
                    break;
                case EntityType.ContentKey:
                    IQueryable<IContentKey> contentKeys = _media.ContentKeys.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? contentKeys.SingleOrDefault() : contentKeys.FirstOrDefault();
                    break;
                case EntityType.ContentKeyAuthPolicy:
                    IQueryable<IContentKeyAuthorizationPolicy> authPolicies = _media.ContentKeyAuthorizationPolicies.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? authPolicies.SingleOrDefault() : authPolicies.FirstOrDefault();
                    break;
                case EntityType.ContentKeyAuthPolicyOption:
                    IQueryable<IContentKeyAuthorizationPolicyOption> policyOptions = _media.ContentKeyAuthorizationPolicyOptions.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? policyOptions.SingleOrDefault() : policyOptions.FirstOrDefault();
                    break;
                case EntityType.Manifest:
                    IQueryable<IIngestManifest> manifests = _media.IngestManifests.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? manifests.SingleOrDefault() : manifests.FirstOrDefault();
                    break;
                case EntityType.ManifestAsset:
                    IQueryable<IIngestManifestAsset> manifestAssets = _media.IngestManifestAssets.Where(x => x.Asset.Name.Contains(entityName));
                    entity = requireUnique ? manifestAssets.SingleOrDefault() : manifestAssets.FirstOrDefault();
                    break;
                case EntityType.ManifestFile:
                    IQueryable<IIngestManifestFile> manifestFiles = _media.IngestManifestFiles.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? manifestFiles.SingleOrDefault() : manifestFiles.FirstOrDefault();
                    break;
                case EntityType.Asset:
                    IQueryable<IAsset> assets = _media.Assets.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? assets.SingleOrDefault() : assets.FirstOrDefault();
                    break;
                case EntityType.File:
                    IQueryable<IAssetFile> files = _media.Files.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? files.SingleOrDefault() : files.FirstOrDefault();
                    break;
                case EntityType.Channel:
                    IQueryable<IChannel> channels = _media.Channels.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? channels.SingleOrDefault() : channels.FirstOrDefault();
                    break;
                case EntityType.Program:
                    IQueryable<IProgram> programs = _media.Programs.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? programs.SingleOrDefault() : programs.FirstOrDefault();
                    break;
                case EntityType.ReservedUnit:
                    IQueryable<IEncodingReservedUnit> reservedUnits = _media.EncodingReservedUnits.Where(x => x.ReservedUnitType.ToString().Contains(entityName));
                    entity = requireUnique ? reservedUnits.SingleOrDefault() : reservedUnits.FirstOrDefault();
                    break;
                case EntityType.Processor:
                    IQueryable<IMediaProcessor> processors = _media.MediaProcessors.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? processors.SingleOrDefault() : processors.FirstOrDefault();
                    break;
                case EntityType.JobTemplate:
                    IQueryable<IJobTemplate> templates = _media.JobTemplates.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? templates.SingleOrDefault() : templates.FirstOrDefault();
                    break;
                case EntityType.Job:
                    IQueryable<IJob> jobs = _media.Jobs.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? jobs.SingleOrDefault() : jobs.FirstOrDefault();
                    break;
                case EntityType.NotificationEndpoint:
                    IQueryable<INotificationEndPoint> notifications = _media.NotificationEndPoints.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? notifications.SingleOrDefault() : notifications.FirstOrDefault();
                    break;
                case EntityType.AccessPolicy:
                    IQueryable<IAccessPolicy> accessPolicies = _media.AccessPolicies.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? accessPolicies.SingleOrDefault() : accessPolicies.FirstOrDefault();
                    break;
                case EntityType.DeliveryPolicy:
                    IQueryable<IAssetDeliveryPolicy> deliveryPolicies = _media.AssetDeliveryPolicies.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? deliveryPolicies.SingleOrDefault() : deliveryPolicies.FirstOrDefault();
                    break;
                case EntityType.StreamingEndpoint:
                    IQueryable<IStreamingEndpoint> streamingEndpoints = _media.StreamingEndpoints.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? streamingEndpoints.SingleOrDefault() : streamingEndpoints.FirstOrDefault();
                    break;
                case EntityType.StreamingFilter:
                    IQueryable<IStreamingFilter> streamingFilters = _media.Filters.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? streamingFilters.SingleOrDefault() : streamingFilters.FirstOrDefault();
                    break;
                case EntityType.Locator:
                    IQueryable<ILocator> locators = _media.Locators.Where(x => x.Name.Contains(entityName));
                    entity = requireUnique ? locators.SingleOrDefault() : locators.FirstOrDefault();
                    break;
            }
            return entity;
        }
    }
}
