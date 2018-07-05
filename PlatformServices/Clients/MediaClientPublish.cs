using System;
using System.Text;

using Microsoft.Rest;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;

using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static string GetNotificationMessage(MediaAccount mediaAccount, Job job)
        {
            StringBuilder message = new StringBuilder();
            message.Append("AMS Transform Job Notification");
            message.Append("\n\nMedia Account\n");
            message.Append(mediaAccount.Name);
            message.Append("\n\nJob Name\n");
            message.Append(job.Name);
            message.Append("\n\nJob State\n");
            message.Append(job.State.ToString());
            foreach (JobOutput jobOutput in job.Outputs)
            {
                message.Append("\n\nJob Output State\n");
                message.Append(jobOutput.State.ToString());
                message.Append("\n\nJob Output Progress\n");
                message.Append(jobOutput.Progress);
                if (jobOutput.Error != null)
                {
                    message.Append("\n\nError Category\n");
                    message.Append(jobOutput.Error.Category.ToString());
                    message.Append("\n\nError Code: ");
                    message.Append(jobOutput.Error.Code.ToString());
                    message.Append("\n\nError Message\n");
                    message.Append(jobOutput.Error.Message);
                    foreach (JobErrorDetail errorDetail in jobOutput.Error.Details)
                    {
                        message.Append("\n\nError Detail Code\n");
                        message.Append(errorDetail.Code.ToString());
                        message.Append("\n\nError Detail Message\n");
                        message.Append(errorDetail.Message);
                    }
                }
            }
            return message.ToString();
        }

        private static string GetNotificationMessage(MediaAccount mediaAccount, string indexId, string indexState)
        {
            StringBuilder message = new StringBuilder();
            message.Append("VI Upload Index Notification");
            message.Append("\n\nVideo Indexer Account\n");
            message.Append(mediaAccount.VideoIndexerKey);
            message.Append("\n\nIndex Id\n");
            message.Append(indexId);
            message.Append("\n\nIndex State\n");
            message.Append(indexState);
            return message.ToString();
        }

        public static string SendNotificationMessage(MediaPublish mediaPublish, Job job, string indexId, string indexState)
        {
            string message = Constant.Message.MobileNumberNotAvailable;
            if (!string.IsNullOrEmpty(mediaPublish.UserAccount.MobileNumber))
            {
                if (job == null)
                {
                    message = GetNotificationMessage(mediaPublish.MediaAccount, indexId, indexState);
                }
                else
                {
                    message = GetNotificationMessage(mediaPublish.MediaAccount, job);
                }


                string settingKey = Constant.AppSettingKey.TwilioAccountId;
                string accountId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.TwilioAccountToken;
                string accountToken = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.TwilioMessageFrom;
                string messageFrom = AppSetting.GetValue(settingKey);

                PhoneNumber fromPhoneNumber = new PhoneNumber(messageFrom);
                PhoneNumber toPhoneNumber = new PhoneNumber(mediaPublish.UserAccount.MobileNumber);

                CreateMessageOptions messageOptions = new CreateMessageOptions(toPhoneNumber)
                {
                    From = fromPhoneNumber,
                    Body = message
                };

                TwilioClient.Init(accountId, accountToken);
                MessageResource.CreateAsync(messageOptions);
            }
            return message;
        }

        public static void SetPublishEvent(string authToken)
        {
            try
            {
                EventSubscription eventSubscription = new EventSubscription(name: Constant.Media.Publish.EventTriggerName)
                {
                    Destination = new WebHookEventSubscriptionDestination()
                    {
                        EndpointUrl = AppSetting.GetValue(Constant.AppSettingKey.MediaPublishUrl)
                    },
                    Filter = new EventSubscriptionFilter()
                    {
                        IncludedEventTypes = Constant.Media.Publish.EventTriggerTypes
                    }
                };

                TokenCredentials azureToken = AuthToken.AcquireToken(authToken, out string subscriptionId);
                EventGridManagementClient eventGridClient = new EventGridManagementClient(azureToken)
                {
                    SubscriptionId = subscriptionId
                };

                User authUser = new User(authToken);
                string eventScope = authUser.MediaAccount.ResourceId;
                eventSubscription = eventGridClient.EventSubscriptions.CreateOrUpdate(eventScope, eventSubscription.Name, eventSubscription);
            }
            catch (Exception ex)
            {
                // Log exception in Application Insights
            }
        }

        public static string PublishOutput(MediaPublish mediaPublish)
        {
            string publishMessage = string.Empty;
            using (MediaClient mediaClient = new MediaClient(null, mediaPublish.MediaAccount))
            {
                string jobName = mediaPublish.Id;
                string transformName = mediaPublish.TransformName;
                Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
                if (job != null)
                {
                    string streamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly;
                    foreach (JobOutputAsset jobOutput in job.Outputs)
                    {
                        StreamingLocator locator = mediaClient.GetEntity<StreamingLocator>(MediaEntity.StreamingLocator, jobOutput.AssetName);
                        if (locator == null)
                        {
                            locator = mediaClient.CreateLocator(jobOutput.AssetName, streamingPolicyName);
                        }
                    }
                    publishMessage = SendNotificationMessage(mediaPublish, job, null, null);
                }
            }
            return publishMessage;
        }

        public static void PurgePublish()
        {
            //using (DatabaseClient databaseClient = new DatabaseClient())
            //{
            //    string collectionId = Constant.Database.Collection.OutputInsight;
            //    JObject[] documents = databaseClient.GetDocuments(collectionId);
            //    foreach (JObject document in documents)
            //    {
            //        MediaAccount mediaAccount = GetMediaAccount(document);
            //        using MediaClient mediaClient = new MediaClient(null, mediaAccount);
            //        string assetId = document["id"].ToString();
            //        IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
            //        if (asset == null)
            //        {
            //            databaseClient.DeleteDocument(collectionId, assetId);
            //        }
            //    }

            //    collectionId = Constant.Database.Collection.OutputPublish;
            //    documents = databaseClient.GetDocuments(collectionId);
            //    foreach (JObject document in documents)
            //    {
            //        MediaAccount mediaAccount = GetMediaAccount(document);
            //        using MediaClient mediaClient = new MediaClient(null, mediaAccount);
            //        string jobId = document["id"].ToString();
            //        IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;
            //        if (job == null)
            //        {
            //            JToken taskIds = document["TaskIds"];
            //            foreach (JToken taskId in taskIds)
            //            {
            //                databaseClient.DeleteDocument(collectionId, taskId.ToString());
            //            }
            //            databaseClient.DeleteDocument(collectionId, jobId);
            //        }
            //    }
            //}
        }
    }
}