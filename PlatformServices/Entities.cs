using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

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
                storageInfo = string.Concat(storageInfo, ", Storage Used: ", storageUsed, ")");
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
            string fileExtension = Constants.Media.AssetMetadata.VttExtension;
            string[] fileNames = MediaClient.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                MediaTrack track = new MediaTrack();
                track.Type = Constants.Media.TrackSubtitles;
                track.Source = mediaClient.GetLocatorUrl(asset, locatorType, fileName);
                track.Language = GetLanguageCode(track.Source);
                tracks.Add(track);
            }
            return tracks.ToArray();
        }

        private static int CompareStreams(MediaStream leftSide, MediaStream rightSide)
        {
            int comparison = string.Compare(leftSide.Name, rightSide.Name);
            if (comparison == 0)
            {
                if (leftSide.ProtectionTypes.Length == 0)
                {
                    comparison = -1;
                }
                else if (rightSide.ProtectionTypes.Length == 0)
                {
                    comparison = 1;
                }
                else
                {
                    string leftType = leftSide.ProtectionTypes[0];
                    string rightType = rightSide.ProtectionTypes[0];
                    StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;
                    if (string.Equals(leftType, MediaProtection.AES.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, MediaProtection.AES.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                    else if (string.Equals(leftType, MediaProtection.PlayReady.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, MediaProtection.PlayReady.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                    else if (string.Equals(leftType, MediaProtection.Widevine.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, MediaProtection.Widevine.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                }
            }
            return comparison;
        }

        private static void DeleteAsset(MediaClient mediaClient, IAsset asset)
        {
            DatabaseClient databaseClient = new DatabaseClient(true);
            string collectionId = Constants.Database.CollectionName.Metadata;
            foreach (IAssetFile file in asset.AssetFiles)
            {
                if (file.Name.EndsWith(Constants.Media.AssetMetadata.JsonExtension))
                {
                    string[] fileNameInfo = file.Name.Split(Constants.NamedItemsSeparator);
                    string documentId = fileNameInfo[0];
                    databaseClient.DeleteDocument(collectionId, documentId);
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
                if (notificationEndpoint.EndPointType != NotificationEndPointType.AzureTable)
                {
                    notificationEndpoint.Delete();
                }
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
            entityCounts.Add(new string[] { "Processor (Reserved) Units", GetProcessorUnitCount(processorUnits) });
            entityCounts.Add(new string[] { "Media Processors", processors.Length.ToString(), "/account/processors" });
            entityCounts.Add(new string[] { "Channel Programs", programs.Length.ToString() });
            entityCounts.Add(new string[] { "Job Templates", jobTemplates.Length.ToString() });
            entityCounts.Add(new string[] { "Jobs", jobs.Length.ToString() });
            entityCounts.Add(new string[] { "Notification Endpoints", notificationEndpoints.Length.ToString(), "/account/notifications" });
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

        public static NameValueCollection GetAnalyticsProcessors(IAsset asset)
        {
            NameValueCollection analyticsProcessors = new NameValueCollection();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(Constants.Media.AssetMetadata.JsonExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] fileNameInfo = assetFile.Name.Split('_');
                    string processorName = fileNameInfo[fileNameInfo.Length - 1];
                    processorName = processorName.Replace(Constants.Media.AssetMetadata.JsonExtension, string.Empty);
                    processorName = processorName.Replace(Constants.NamedItemSeparator, ' ');
                    analyticsProcessors.Add(processorName, assetFile.Name);
                }
            }
            return analyticsProcessors;
        }

        public static object GetMediaProcessors(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            return mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
        }

        public static object GetMediaProcessors(string accountName, string accountKey)
        {
            MediaClient mediaClient = new MediaClient(accountName, accountKey);
            return mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
        }

        public static List<MediaStream> GetMediaStreams(MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
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
            mediaStreams.Sort(CompareStreams);
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

        public static string GetLiveSourceUrl(bool livePreview)
        {
            string liveSourceUrl = string.Empty;
            string settingKey = Constants.AppSettings.MediaLiveAccount;
            string[] liveAccount = AppSetting.GetValue(settingKey, true);
            if (liveAccount.Length > 0)
            {
                settingKey = Constants.AppSettings.MediaLiveChannelName;
                string channelName = AppSetting.GetValue(settingKey);
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
    }
}
