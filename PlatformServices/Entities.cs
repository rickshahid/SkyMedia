using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    public static class Entities
    {
        private static string GetStorageInfo(string authToken, IStorageAccount storageAccount)
        {
            string storageInfo = string.Concat("Account: ", storageAccount.Name);
            string storageUsed = Storage.GetCapacityUsed(authToken, storageAccount.Name);
            if (!string.IsNullOrEmpty(storageUsed))
            {
                storageInfo = string.Concat(storageInfo, ", Usage: ", storageUsed, ")");
            }
            return storageInfo;
        }

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

        private static MediaTrack[] GetTextTracks(MediaClient mediaClient, IAsset asset, LocatorType locatorType)
        {
            List<MediaTrack> tracks = new List<MediaTrack>();
            string fileExtension = Constants.Media.AssetFile.VttExtension;
            string[] fileNames = MediaClient.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                MediaTrack track = new MediaTrack();
                track.Type = Constants.Media.Stream.TextTrackSubtitles;
                track.Source = mediaClient.GetLocatorUrl(asset, locatorType, fileName);
                track.Language = GetLanguageCode(track.Source);
                tracks.Add(track);
            }
            return tracks.ToArray();
        }

        private static int OrderLocators(ILocator leftSide, ILocator rightSide)
        {
            return DateTime.Compare(leftSide.Asset.Created, rightSide.Asset.Created);
        }

        private static void DeleteAsset(MediaClient mediaClient, IAsset asset)
        {
            using (DatabaseClient databaseClient = new DatabaseClient(true))
            {
                string collectionId = Constants.Database.DocumentCollection.Metadata;
                foreach (IAssetFile file in asset.AssetFiles)
                {
                    if (file.Name.EndsWith(Constants.Media.AssetFile.JsonExtension))
                    {
                        string[] fileNameInfo = file.Name.Split(Constants.NamedItemsSeparator);
                        string documentId = fileNameInfo[0];
                        databaseClient.DeleteDocument(collectionId, documentId);
                    }
                }
            }
            foreach (ILocator locator in asset.Locators)
            {
                locator.Delete();
            }
            for (int i = asset.DeliveryPolicies.Count - 1; i > -1; i--)
            {
                asset.DeliveryPolicies.RemoveAt(i);
            }
            asset.Delete();
        }
        
        public static void ClearAccount(MediaClient mediaClient, bool allEntities)
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
                    DeleteAsset(mediaClient, asset);
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
            }
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
            IMediaProcessor[] processors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
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

        public static NameValueCollection GetStorageAccounts(string authToken)
        {
            NameValueCollection storageAccounts = new NameValueCollection();
            MediaClient mediaClient = new MediaClient(authToken);
            IStorageAccount storageAccount = mediaClient.DefaultStorageAccount;
            string storageInfo = GetStorageInfo(authToken, storageAccount);
            storageAccounts.Add(storageInfo, storageAccount.Name);
            IStorageAccount[] accounts = mediaClient.GetEntities(MediaEntity.StorageAccount) as IStorageAccount[];
            foreach (IStorageAccount account in accounts)
            {
                if (!account.IsDefault)
                {
                    storageInfo = GetStorageInfo(authToken, account);
                    storageAccounts.Add(storageInfo, account.Name);
                }
            }
            return storageAccounts;
        }

        public static object GetMediaProcessors(string authToken, bool gridView)
        {
            object mediaProcessors = null;
            if (gridView)
            {
                MediaClient mediaClient = new MediaClient(authToken);
                mediaProcessors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
            }
            else
            {
                CacheClient cacheClient = new CacheClient(authToken);
                string itemKey = Constants.Cache.ItemKey.MediaProcessors;
                MediaProcessor[] mediaProcessorTypes = cacheClient.GetValue<MediaProcessor[]>(itemKey);
                if (mediaProcessorTypes == null)
                {
                    mediaProcessorTypes = cacheClient.Initialize(authToken);
                }
                NameValueCollection processors = new NameValueCollection();
                foreach (MediaProcessor mediaProcessorType in mediaProcessorTypes)
                {
                    string processorName = GetMediaProcessorName(mediaProcessorType);
                    processors.Add(processorName, mediaProcessorType.ToString());
                }
                mediaProcessors = processors;
            }
            return mediaProcessors;
        }

        public static string GetMediaProcessorId(MediaProcessor mediaProcessor)
        {
            string processorId = null;
            switch (mediaProcessor)
            {
                case MediaProcessor.EncoderStandard:
                    processorId = Constants.Media.ProcessorId.EncoderStandard;
                    break;
                case MediaProcessor.EncoderPremium:
                    processorId = Constants.Media.ProcessorId.EncoderPremium;
                    break;
                case MediaProcessor.EncoderUltra:
                    processorId = Constants.Media.ProcessorId.EncoderUltra;
                    break;
                case MediaProcessor.IndexerV1:
                    processorId = Constants.Media.ProcessorId.IndexerV1;
                    break;
                case MediaProcessor.IndexerV2:
                    processorId = Constants.Media.ProcessorId.IndexerV2;
                    break;
                case MediaProcessor.FaceDetection:
                    processorId = Constants.Media.ProcessorId.FaceDetection;
                    break;
                case MediaProcessor.FaceRedaction:
                    processorId = Constants.Media.ProcessorId.FaceRedaction;
                    break;
                case MediaProcessor.MotionDetection:
                    processorId = Constants.Media.ProcessorId.MotionDetection;
                    break;
                case MediaProcessor.MotionHyperlapse:
                    processorId = Constants.Media.ProcessorId.MotionHyperlapse;
                    break;
                case MediaProcessor.MotionStabilization:
                    processorId = Constants.Media.ProcessorId.MotionStabilization;
                    break;
                case MediaProcessor.VideoAnnotation:
                    processorId = Constants.Media.ProcessorId.VideoAnnotation;
                    break;
                case MediaProcessor.VideoSummarization:
                    processorId = Constants.Media.ProcessorId.VideoSummarization;
                    break;
                case MediaProcessor.ThumbnailGeneration:
                    processorId = Constants.Media.ProcessorId.ThumbnailGeneration;
                    break;
                case MediaProcessor.CharacterRecognition:
                    processorId = Constants.Media.ProcessorId.CharacterRecognition;
                    break;
                case MediaProcessor.ContentModeration:
                    processorId = Constants.Media.ProcessorId.ContentModeration;
                    break;
            }
            return processorId;
        }

        public static string GetMediaProcessorName(MediaProcessor mediaProcessor)
        {
            return Regex.Replace(mediaProcessor.ToString(), Constants.CapitalSpacingExpression, Constants.CapitalSpacingReplacement);
        }

        public static MediaProcessor GetMediaProcessorType(string processorId)
        {
            MediaProcessor mediaProcessor = MediaProcessor.None;
            switch (processorId)
            {
                case Constants.Media.ProcessorId.EncoderStandard:
                    mediaProcessor = MediaProcessor.EncoderStandard;
                    break;
                case Constants.Media.ProcessorId.EncoderPremium:
                    mediaProcessor = MediaProcessor.EncoderPremium;
                    break;
                case Constants.Media.ProcessorId.EncoderUltra:
                    mediaProcessor = MediaProcessor.EncoderUltra;
                    break;
                case Constants.Media.ProcessorId.IndexerV1:
                    mediaProcessor = MediaProcessor.IndexerV1;
                    break;
                case Constants.Media.ProcessorId.IndexerV2:
                    mediaProcessor = MediaProcessor.IndexerV2;
                    break;
                case Constants.Media.ProcessorId.FaceDetection:
                    mediaProcessor = MediaProcessor.FaceDetection;
                    break;
                case Constants.Media.ProcessorId.FaceRedaction:
                    mediaProcessor = MediaProcessor.FaceRedaction;
                    break;
                case Constants.Media.ProcessorId.MotionDetection:
                    mediaProcessor = MediaProcessor.MotionDetection;
                    break;
                case Constants.Media.ProcessorId.MotionHyperlapse:
                    mediaProcessor = MediaProcessor.MotionHyperlapse;
                    break;
                case Constants.Media.ProcessorId.MotionStabilization:
                    mediaProcessor = MediaProcessor.MotionStabilization;
                    break;
                case Constants.Media.ProcessorId.VideoAnnotation:
                    mediaProcessor = MediaProcessor.VideoAnnotation;
                    break;
                case Constants.Media.ProcessorId.VideoSummarization:
                    mediaProcessor = MediaProcessor.VideoSummarization;
                    break;
                case Constants.Media.ProcessorId.ThumbnailGeneration:
                    mediaProcessor = MediaProcessor.ThumbnailGeneration;
                    break;
                case Constants.Media.ProcessorId.CharacterRecognition:
                    mediaProcessor = MediaProcessor.CharacterRecognition;
                    break;
                case Constants.Media.ProcessorId.ContentModeration:
                    mediaProcessor = MediaProcessor.ContentModeration;
                    break;
            }
            return mediaProcessor;
        }

        public static MediaMetadata[] GetAnalyticsProcessors(IAsset asset)
        {
            List<MediaMetadata> analyticsProcessors = new List<MediaMetadata>();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(Constants.Media.AssetFile.JsonExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] fileNameInfo = assetFile.Name.Split('_');
                    string processorName = fileNameInfo[fileNameInfo.Length - 1];
                    processorName = processorName.Replace(Constants.Media.AssetFile.JsonExtension, string.Empty);
                    processorName = processorName.Replace(Constants.NamedItemSeparator, ' ');

                    MediaMetadata mediaMetadata = new MediaMetadata();
                    mediaMetadata.ProcessorName = processorName;
                    mediaMetadata.MetadataFile = assetFile.Name;
                    analyticsProcessors.Add(mediaMetadata);
                }
            }
            return analyticsProcessors.ToArray();
        }

        public static object GetNotificationEndpoints(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(MediaEntity.NotificationEndpoint) as INotificationEndPoint[];
        }

        public static object GetNotificationEndpoints(string accountName, string accountKey)
        {
            MediaClient mediaClient = new MediaClient(accountName, accountKey);
            return mediaClient.GetEntities(MediaEntity.NotificationEndpoint) as INotificationEndPoint[];
        }

        public static List<MediaStream> GetMediaStreams(MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            Array.Sort<ILocator>(locators, OrderLocators);
            foreach (ILocator locator in locators)
            {
                IAsset asset = locator.Asset;
                if (asset.IsStreamable && asset.AssetFiles.Count() > 1)
                {
                    string locatorUrl = mediaClient.GetLocatorUrl(asset, locator.Type, null);
                    if (!string.IsNullOrEmpty(locatorUrl))
                    {
                        MediaStream mediaStream = new MediaStream();
                        mediaStream.Name = asset.Name;
                        mediaStream.SourceUrl = locatorUrl;
                        mediaStream.TextTracks = GetTextTracks(mediaClient, asset, locator.Type);
                        mediaStream.ProtectionTypes = mediaClient.GetProtectionTypes(asset);
                        mediaStream.AnalyticsProcessors = GetAnalyticsProcessors(asset);
                        mediaStreams.Add(mediaStream);
                    }
                }
                if (mediaStreams.Count == 5)
                {
                    break;
                }
            }
            return mediaStreams;
        }

        public static bool StreamingEnabled(MediaClient mediaClient)
        {
            bool streamingEnabled = false;
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.State == StreamingEndpointState.Running)
                {
                    streamingEnabled = true;
                }
            }
            return streamingEnabled;
        }

        public static string GetLanguageCode(string sourceUrl)
        {
            string[] sourceInfo = sourceUrl.Split('.');
            string fileName = sourceInfo[sourceInfo.Length - 2];
            return fileName.Substring(fileName.Length - 2);
        }

        public static string GetLiveSourceUrl(string channelName, bool livePreview)
        {
            string liveSourceUrl = string.Empty;
            string settingKey = Constants.AppSettingKey.AzureMedia;
            string[] liveAccount = AppSetting.GetValue(settingKey, true);
            if (liveAccount.Length > 0)
            {
                MediaClient mediaClient = new MediaClient(liveAccount[0], liveAccount[1]);
                IChannel channel = mediaClient.GetEntityByName(MediaEntity.Channel, channelName, true) as IChannel;
                if (channel != null && channel.State == ChannelState.Running)
                {
                    if (livePreview)
                    {
                        liveSourceUrl = channel.Preview.Endpoints.First().Url.ToString();
                    }
                    else
                    {
                        IProgram program = channel.Programs.First();
                        if (program.State == ProgramState.Running)
                        {
                            liveSourceUrl = mediaClient.GetLocatorUrl(program.Asset, LocatorType.OnDemandOrigin, null);
                        }
                    }
                }
            }
            return liveSourceUrl;
        }

        public static DateTime? GetLiveEventStart(string accountName, string channelName)
        {
            DateTime? eventStart = null;
            EntityClient entityClient = new EntityClient();
            string tableName = Constants.Storage.TableName.LiveEvent;
            MediaLiveEvent liveEvent = entityClient.GetEntity<MediaLiveEvent>(tableName, accountName, channelName);
            if (liveEvent != null)
            {
                eventStart = liveEvent.EventStart;
            }
            return eventStart;
        }
    }
}
