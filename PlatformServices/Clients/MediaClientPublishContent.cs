using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

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
                LocatorType locatorType = LocatorType.OnDemandOrigin;
                for (int i = asset.Locators.Count - 1; i >= 0; i--)
                {
                    if (asset.Locators[i].Type == locatorType)
                    {
                        asset.Locators[i].Delete();
                    }
                }
                for (int i = asset.DeliveryPolicies.Count - 1; i >= 0; i--)
                {
                    asset.DeliveryPolicies.RemoveAt(i);
                }
                if (contentProtection != null)
                {
                    mediaClient.AddDeliveryPolicies(asset, contentProtection);
                }
                mediaClient.CreateLocator(locatorType, asset);
            }
        }

        private static void PublishJob(MediaClient mediaClient, IJob job, MediaContentPublish contentPublish)
        {
            string processorId1 = Constant.Media.ProcessorId.EncoderStandard;
            string processorId2 = Constant.Media.ProcessorId.EncoderPremium;
            string processorId3 = Constant.Media.ProcessorId.EncoderUltra;
            string[] processorIds = new string[] { processorId1, processorId2, processorId3 };
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

        public static MediaPublish PublishContent(string queueName)
        {
            MessageClient messageClient = new MessageClient();
            MediaContentPublish contentPublish = messageClient.GetMessage<MediaContentPublish>(queueName, out string messageId, out string popReceipt);

            MediaPublish mediaPublish = null;
            if (contentPublish != null)
            {
                string accountId = contentPublish.PartitionKey;
                string accountKey = contentPublish.MediaAccountKey;
                string jobId = contentPublish.RowKey;

                MediaClient mediaClient = new MediaClient(accountId, accountKey);
                IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;
                if (job != null)
                {
                    mediaClient.SetProcessorUnits(job, null, ReservedUnitType.Basic);
                    PublishJob(mediaClient, job, contentPublish);

                    mediaPublish = new MediaPublish()
                    {
                        UserId = contentPublish.UserId,
                        MobileNumber = contentPublish.MobileNumber,
                        StatusMessage = GetNotificationMessage(accountId, job)
                    };
                }
                messageClient.DeleteMessage(queueName, messageId, popReceipt);
            }
            return mediaPublish;
        }
    }
}