using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static void PublishAsset(MediaClient mediaClient, IAsset asset, ContentProtection contentProtection)
        {
            if (asset.IsStreamable)
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
                foreach (ITask jobTask in jobTasks)
                {
                    ContentProtection contentProtection = GetContentProtection(job.Id, jobTask.Id);
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

        public static MediaPublish PublishContent(string queueName)
        {
            MediaPublish mediaPublish = null;
            QueueClient queueClient = new QueueClient();
            MediaContentPublish contentPublish = queueClient.GetMessage<MediaContentPublish>(queueName, out string messageId, out string popReceipt);
            if (contentPublish != null)
            {
                mediaPublish = PublishContent(contentPublish);
                queueClient.DeleteMessage(queueName, messageId, popReceipt);
            }
            return mediaPublish;
        }
    }
}