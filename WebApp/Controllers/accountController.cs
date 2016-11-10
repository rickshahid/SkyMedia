using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

using SkyMedia.ServiceBroker;

namespace SkyMedia.WebApp.Controllers
{
    public class accountController : Controller
    {
        internal static void CreateChannel(string authToken, string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                string settingKey = Constants.AppSettings.MediaLiveChannelName;
                channelName = AppSetting.GetValue(settingKey);
            }
            MediaClient mediaClient = new MediaClient(authToken);
            ChannelEncodingType channelType = ChannelEncodingType.None;
            StreamingProtocol ingestProtocol = StreamingProtocol.RTMP;
            IChannel channel = mediaClient.CreateChannel(channelName, channelType, ingestProtocol);
            foreach (IProgram program in channel.Programs)
            {
                mediaClient.CreateLocator(null, LocatorType.OnDemandOrigin, program.Asset, null);
            }
        }

        internal static void SignalChannel(string authToken, string channelName, int cueId)
        {
            string settingKey = Constants.AppSettings.MediaChannelAdvertisementSeconds;
            string timeSeconds = AppSetting.GetValue(settingKey);
            TimeSpan timeSpan = new TimeSpan(0, 0, int.Parse(timeSeconds));

            IChannel[] channels;
            MediaClient mediaClient = new MediaClient(authToken);
            if (string.IsNullOrEmpty(channelName))
            {
                IChannel channel = mediaClient.GetEntityByName(EntityType.Channel, channelName, false) as IChannel;
                channels = new IChannel[] { channel };
            }
            else
            {
                channels = mediaClient.GetEntities(EntityType.Channel) as IChannel[];
            }
            foreach (IChannel channel in channels)
            {
                if (channel.EncodingType != ChannelEncodingType.None)
                {
                    channel.StartAdvertisement(timeSpan, cueId, true);
                }
            }
        }

        private static string GetReservedEncodingUnits(IEncodingReservedUnit[] encodingReservedUnits)
        {
            int encodingUnits = 0;
            foreach (IEncodingReservedUnit encodingReservedUnit in encodingReservedUnits)
            {
                encodingUnits = encodingUnits + encodingReservedUnit.CurrentReservedUnits;
            }
            return encodingUnits.ToString();
        }

        private static string GetDefaultStreamingUnits(IStreamingEndpoint[] streamingEndpoints)
        {
            int streamingUnits = 0;
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (string.Equals(streamingEndpoint.Name, Constants.Media.Streaming.DefaultEndpointName, StringComparison.InvariantCultureIgnoreCase) && streamingEndpoint.ScaleUnits.HasValue)
                {
                    streamingUnits = streamingEndpoint.ScaleUnits.Value;
                }
            }
            return streamingUnits.ToString();
        }

        private static void DeleteAsset(MediaClient mediaClient, IAsset asset)
        {
            //if (!mediaClient.LiveAsset(asset))
            //{
                foreach (ILocator locator in asset.Locators)
                {
                    locator.Delete();
                }
                for (int i = asset.DeliveryPolicies.Count - 1; i > -1; i--)
                {
                    asset.DeliveryPolicies.RemoveAt(i);
                }
                asset.Delete();
            //}
        }

        internal static void DeleteEntities(string authToken, bool allAssets)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            IAsset[] assets = mediaClient.GetEntities(EntityType.Asset) as IAsset[];
            if (allAssets)
            {
                IProgram[] programs = mediaClient.GetEntities(EntityType.Program) as IProgram[];
                foreach (IProgram program in programs)
                {
                    program.Delete();
                }
                IChannel[] channels = mediaClient.GetEntities(EntityType.Channel) as IChannel[];
                foreach (IChannel channel in channels)
                {
                    channel.Delete();
                }
            }
            foreach (IAsset asset in assets)
            {
                if (asset.ParentAssets.Count > 0 || allAssets)
                {
                    DeleteAsset(mediaClient, asset);
                }
            }
            if (allAssets)
            {
                IIngestManifest[] manifests = mediaClient.GetEntities(EntityType.Manifest) as IIngestManifest[];
                foreach (IIngestManifest manifest in manifests)
                {
                    manifest.Delete();
                }
                IAccessPolicy[] accessPolicies = mediaClient.GetEntities(EntityType.AccessPolicy) as IAccessPolicy[];
                foreach (IAccessPolicy accessPolicy in accessPolicies)
                {
                    accessPolicy.Delete();
                }
                IAssetDeliveryPolicy[] deliveryPolicies = mediaClient.GetEntities(EntityType.DeliveryPolicy) as IAssetDeliveryPolicy[];
                foreach (IAssetDeliveryPolicy deliveryPolicy in deliveryPolicies)
                {
                    deliveryPolicy.Delete();
                }
                IContentKeyAuthorizationPolicy[] contentKeyAuthPolicies = mediaClient.GetEntities(EntityType.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
                foreach (IContentKeyAuthorizationPolicy contentKeyAuthPolicy in contentKeyAuthPolicies)
                {
                    contentKeyAuthPolicy.Delete();
                }
                IContentKeyAuthorizationPolicyOption[] contentKeyAuthPolicyOptions = mediaClient.GetEntities(EntityType.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
                foreach (IContentKeyAuthorizationPolicyOption contentKeyAuthPolicyOption in contentKeyAuthPolicyOptions)
                {
                    contentKeyAuthPolicyOption.Delete();
                }
            }
            //IJob[] jobs = mediaClient.GetEntities(EntityType.Job) as IJob[];
            //foreach (IJob job in jobs)
            //{
            //    job.Delete();
            //}
            //INotificationEndPoint[] notificationEndpoints = mediaClient.GetEntities(EntityType.NotificationEndpoint) as INotificationEndPoint[];
            //foreach (INotificationEndPoint notificationEndpoint in notificationEndpoints)
            //{
            //    notificationEndpoint.Delete();
            //}
        }

        internal static string[][] GetEntityCounts(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);

            IStorageAccount[] storageAccounts = mediaClient.GetEntities(EntityType.StorageAccount) as IStorageAccount[];
            IContentKey[] contentKeys = mediaClient.GetEntities(EntityType.ContentKey) as IContentKey[];
            IContentKeyAuthorizationPolicy[] contentKeyAuthZPolicies = mediaClient.GetEntities(EntityType.ContentKeyAuthPolicy) as IContentKeyAuthorizationPolicy[];
            IContentKeyAuthorizationPolicyOption[] contentKeyAuthPolicyOptions = mediaClient.GetEntities(EntityType.ContentKeyAuthPolicyOption) as IContentKeyAuthorizationPolicyOption[];
            IChannel[] channels = mediaClient.GetEntities(EntityType.Channel) as IChannel[];
            IIngestManifest[] manifests = mediaClient.GetEntities(EntityType.Manifest) as IIngestManifest[];
            IIngestManifestAsset[] manifestAssets = mediaClient.GetEntities(EntityType.ManifestAsset) as IIngestManifestAsset[];
            IIngestManifestFile[] manifestFiles = mediaClient.GetEntities(EntityType.ManifestFile) as IIngestManifestFile[];
            IAsset[] assets = mediaClient.GetEntities(EntityType.Asset) as IAsset[];
            IAssetFile[] files = mediaClient.GetEntities(EntityType.File) as IAssetFile[];
            IMediaProcessor[] processors = mediaClient.GetEntities(EntityType.Processor) as IMediaProcessor[];
            IProgram[] programs = mediaClient.GetEntities(EntityType.Program) as IProgram[];
            IJobTemplate[] jobTemplates = mediaClient.GetEntities(EntityType.JobTemplate) as IJobTemplate[];
            IJob[] jobs = mediaClient.GetEntities(EntityType.Job) as IJob[];
            INotificationEndPoint[] notificationEndpoints = mediaClient.GetEntities(EntityType.NotificationEndpoint) as INotificationEndPoint[];
            IAccessPolicy[] accessPolicies = mediaClient.GetEntities(EntityType.AccessPolicy) as IAccessPolicy[];
            IAssetDeliveryPolicy[] deliveryPolicies = mediaClient.GetEntities(EntityType.DeliveryPolicy) as IAssetDeliveryPolicy[];
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(EntityType.StreamingEndpoint) as IStreamingEndpoint[];
            IStreamingFilter[] streamingFilters = mediaClient.GetEntities(EntityType.StreamingFilter) as IStreamingFilter[];
            IEncodingReservedUnit[] encodingUnits = mediaClient.GetEntities(EntityType.ReservedUnit) as IEncodingReservedUnit[];
            ILocator[] locators = mediaClient.GetEntities(EntityType.Locator) as ILocator[];

            List<string[]> entityCounts = new List<string[]>();
            entityCounts.Add(new string[] { "Storage Accounts", storageAccounts.Length.ToString() });
            entityCounts.Add(new string[] { "Content Keys", contentKeys.Length.ToString() });
            entityCounts.Add(new string[] { "Content Key AuthZ Policies", contentKeyAuthZPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Content Key AuthZ Policy Options", contentKeyAuthPolicyOptions.Length.ToString() });
            entityCounts.Add(new string[] { "Channels", channels.Length.ToString() });
            entityCounts.Add(new string[] { "Manifests", manifests.Length.ToString() });
            entityCounts.Add(new string[] { "Manifest Assets", manifestAssets.Length.ToString() });
            entityCounts.Add(new string[] { "Manifest Files", manifestFiles.Length.ToString() });
            entityCounts.Add(new string[] { "Assets", assets.Length.ToString() });
            entityCounts.Add(new string[] { "Files", files.Length.ToString() });
            entityCounts.Add(new string[] { "Processors", processors.Length.ToString(), "/dashboard/processors" });
            entityCounts.Add(new string[] { "Programs", programs.Length.ToString() });
            entityCounts.Add(new string[] { "Job Templates", jobTemplates.Length.ToString() });
            entityCounts.Add(new string[] { "Jobs", jobs.Length.ToString() });
            entityCounts.Add(new string[] { "Notification Endpoints", notificationEndpoints.Length.ToString() });
            entityCounts.Add(new string[] { "Access Policies", accessPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Delivery Policies", deliveryPolicies.Length.ToString() });
            entityCounts.Add(new string[] { "Streaming Endpoints", streamingEndpoints.Length.ToString() });
            entityCounts.Add(new string[] { "Streaming Units (Default Endpoint)", GetDefaultStreamingUnits(streamingEndpoints) });
            entityCounts.Add(new string[] { "Streaming Filters", streamingFilters.Length.ToString() });
            entityCounts.Add(new string[] { "Encoding Reserved Units", GetReservedEncodingUnits(encodingUnits) });
            entityCounts.Add(new string[] { "Locators", locators.Length.ToString() });
            return entityCounts.ToArray();
        }

        public void signin(string subdomain)
        {
            string authScheme = OpenIdConnectDefaults.AuthenticationScheme;
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.Authentication.ChallengeAsync(authScheme, authProperties).Wait();
        }

        public void profileedit(string subdomain)
        {
            string authScheme = OpenIdConnectDefaults.AuthenticationScheme;
            AuthenticationProperties authProperties = new AuthenticationProperties();
            authProperties.Items.Add("SubDomain", subdomain);
            HttpContext.Authentication.ChallengeAsync(authScheme, authProperties).Wait();
        }

        public IActionResult signout()
        {
            HttpContext.Authentication.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme).Wait();
            HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("index", "home");
        }

        public IActionResult index()
        {
            return View();
        }
    }
}
