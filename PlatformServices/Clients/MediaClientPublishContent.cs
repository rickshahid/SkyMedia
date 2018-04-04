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
                    if (locatorId == null)
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

        private static void PublishJob(MediaClient mediaClient, IJob job, MediaPublish contentPublish)
        {
            ITask[] encoderTasks = GetEncoderTasks(job);
            if (encoderTasks.Length == 0)
            {
                foreach (IAsset inputAsset in job.InputMediaAssets)
                {
                    //PublishAnalytics(mediaClient, contentPublish, job, encoderTasks);
                }
            }
            else
            {
                foreach (ITask encoderTask in encoderTasks)
                {
                    ContentProtection contentProtection = GetContentProtection(job.Id, encoderTask.Id);
                    foreach (IAsset outputAsset in encoderTask.OutputAssets)
                    {
                        PublishAsset(mediaClient, outputAsset, contentProtection);
                        //PublishAnalytics(mediaClient, contentPublish, job, encoderTasks);
                    }
                }
            }
        }

        public static MediaPublished PublishContent(MediaPublish mediaPublish)
        {
            MediaPublished mediaPublished = null;
            MediaClient mediaClient = new MediaClient(mediaPublish.MediaAccount);
            IJob job = mediaClient.GetEntityById(MediaEntity.Job, mediaPublish.Id) as IJob;
            if (job != null)
            {
                mediaClient.SetProcessorUnits(job, MediaJobNodeType.Basic, false);
                PublishJob(mediaClient, job, mediaPublish);
                mediaPublished = new MediaPublished()
                {
                    MobileNumber = mediaPublish.MobileNumber,
                    StatusMessage = GetNotificationMessage(mediaPublish.MediaAccount.Id, job)
                };
            }
            return mediaPublished;
        }

        public static MediaPublished PublishContent(string queueName)
        {
            MediaPublished mediaPublished = null;
            QueueClient queueClient = new QueueClient();
            MediaPublish contentPublish = queueClient.GetMessage<MediaPublish>(queueName, out string messageId, out string popReceipt);
            if (contentPublish != null)
            {
                mediaPublished = PublishContent(contentPublish);
                queueClient.DeleteMessage(queueName, messageId, popReceipt);
            }
            return mediaPublished;
        }
    }
}