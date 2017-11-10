using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static ITask[] GetJobTasks(IJob job, string[] processorIds)
        {
            List<ITask> jobTasks = new List<ITask>();
            foreach (ITask jobTask in job.Tasks)
            {
                if (processorIds.Contains(jobTask.MediaProcessorId, StringComparer.OrdinalIgnoreCase))
                {
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static string GetNotificationMessage(string accountId, IJob job)
        {
            string messageText = string.Concat("Azure Media Services Job ", job.State.ToString(), ".");
            messageText = string.Concat(messageText, " Account Id: ", accountId);
            messageText = string.Concat(messageText, ", Job Id: ", job.Id);
            messageText = string.Concat(messageText, ", Job Name: ", job.Name);
            return string.Concat(messageText, ", Job Running Duration: ", job.RunningDuration.ToString(Constant.TextFormatter.ClockTime));
        }

        private static void PublishAsset(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
        {
            if (asset.IsStreamable || asset.AssetType == AssetType.MP4)
            {
                string locatorId = null;
                LocatorType locatorType = LocatorType.OnDemandOrigin;
                List<ILocator> locators = asset.Locators.Where(l => l.Type == locatorType).ToList();
                foreach (ILocator locator in locators)
                {
                    if (string.IsNullOrEmpty(locatorId))
                    {
                        locatorId = locator.Id;
                    }
                    locator.Delete();
                }
                List<IAssetDeliveryPolicy> deliveryPolicies = asset.DeliveryPolicies.ToList();
                foreach (IAssetDeliveryPolicy deliveryPolicy in deliveryPolicies)
                {
                    asset.DeliveryPolicies.Remove(deliveryPolicy);
                }
                if (contentProtection != null)
                {
                    mediaClient.AddDeliveryPolicies(asset, contentProtection);
                }
                mediaClient.CreateLocator(locatorId, locatorType, asset, false);
            }
        }

        private static void PublishJob(MediaClient mediaClient, IJob job, MediaContentPublish contentPublish)
        {
            string processorId1 = Constant.Media.ProcessorId.EncoderStandard;
            string processorId2 = Constant.Media.ProcessorId.EncoderPremium;
            string[] processorIds = new string[] { processorId1, processorId2 };
            ITask[] jobTasks = GetJobTasks(job, processorIds);
            if (jobTasks.Length == 0)
            {
                foreach (IAsset inputAsset in job.InputMediaAssets)
                {
                    PublishAnalytics(mediaClient, job, contentPublish, inputAsset.Id);
                }
            }
            else
            {
                ContentProtection contentProtection = GetContentProtection(contentPublish);
                foreach (ITask jobTask in jobTasks)
                {
                    foreach (IAsset outputAsset in jobTask.OutputAssets)
                    {
                        PublishAsset(mediaClient, outputAsset, contentProtection);
                        PublishAnalytics(mediaClient, job, contentPublish, outputAsset.Id);
                    }
                }
            }
        }

        public static MediaPublish PublishContent(MediaContentPublish contentPublish)
        {
            string accountId = contentPublish.PartitionKey;
            string accountDomain = contentPublish.MediaAccountDomainName;
            string accountEndpoint = contentPublish.MediaAccountEndpointUrl;
            string clientId = contentPublish.MediaAccountClientId;
            string clientKey = contentPublish.MediaAccountClientKey;
            string jobId = contentPublish.RowKey;

            MediaClient mediaClient = new MediaClient(accountDomain, accountEndpoint, clientId, clientKey);
            IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;

            MediaPublish mediaPublish = null;
            if (job != null)
            {
                mediaClient.SetProcessorUnits(job, null, ReservedUnitType.Basic, false);
                PublishJob(mediaClient, job, contentPublish);
                mediaPublish = new MediaPublish()
                {
                    UserId = contentPublish.UserId,
                    MobileNumber = contentPublish.MobileNumber,
                    StatusMessage = GetNotificationMessage(accountId, job)
                };
            }
            return mediaPublish;
        }
    }
}