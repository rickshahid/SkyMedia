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
        private static string GetNotificationMessage(MediaPublish mediaPublish, Job job)
        {
            StringBuilder message = new StringBuilder();
            message.Append("AMS Transform Job Notification");
            message.Append("\n\nMedia Account\n");
            message.Append(mediaPublish.MediaAccount.Name);
            message.Append("\n\nJob Name\n");
            message.Append(job.Name);
            message.Append("\n\nJob State\n");
            message.Append(job.State.ToString());
            message.Append("\n\nTransform Name\n");
            message.Append(mediaPublish.TransformName);
            message.Append("\n\nStreaming Policy Name\n");
            message.Append(mediaPublish.StreamingPolicyName);
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

        private static string GetNotificationMessage(MediaPublish mediaPublish, string indexId, string indexState)
        {
            StringBuilder message = new StringBuilder();
            message.Append("VI Upload Index Notification");
            message.Append("\n\nVideo Indexer Account\n");
            message.Append(mediaPublish.MediaAccount.VideoIndexerKey);
            message.Append("\n\nIndex Id\n");
            message.Append(indexId);
            message.Append("\n\nIndex State\n");
            message.Append(indexState);
            return message.ToString();
        }

        public static string SendNotificationMessage(MediaPublish mediaPublish, Job job, string indexId, string indexState)
        {
            string message = Constant.Message.MobileNumberNotAvailable;
            if (mediaPublish.UserAccount != null && !string.IsNullOrEmpty(mediaPublish.UserAccount.MobileNumber))
            {
                if (job == null)
                {
                    message = GetNotificationMessage(mediaPublish, indexId, indexState);
                }
                else
                {
                    message = GetNotificationMessage(mediaPublish, job);
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

        public static void SetEventSubscription(string authToken)
        {
            EventSubscription eventSubscription = new EventSubscription(name: Constant.Media.Publish.EventGrid.SubscriptionName)
            {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = AppSetting.GetValue(Constant.AppSettingKey.MediaPublishUrl)
                },
                Filter = new EventSubscriptionFilter()
                {
                    IncludedEventTypes = Constant.Media.Publish.EventGrid.SubscriptionTypes
                }
            };

            TokenCredentials azureToken = AuthToken.AcquireToken(authToken, out string subscriptionId);
            EventGridManagementClient eventGridClient = new EventGridManagementClient(azureToken)
            {
                SubscriptionId = subscriptionId
            };

            User authUser = new User(authToken);
            string eventScope = authUser.MediaAccount.ResourceId;
            eventGridClient.EventSubscriptions.CreateOrUpdate(eventScope, eventSubscription.Name, eventSubscription);
        }

        public static string PublishJobOutput(MediaPublish mediaPublish)
        {
            string publishMessage = string.Empty;
            using (MediaClient mediaClient = new MediaClient(null, mediaPublish.MediaAccount))
            {
                string jobName = mediaPublish.Id;
                string transformName = mediaPublish.TransformName;
                Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
                if (job != null)
                {
                    string streamingPolicyName = mediaPublish.StreamingPolicyName;
                    if (string.IsNullOrEmpty(streamingPolicyName))
                    {
                        streamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly;
                    }
                    foreach (JobOutputAsset jobOutput in job.Outputs)
                    {
                        StreamingLocator locator = mediaClient.GetEntity<StreamingLocator>(MediaEntity.StreamingLocator, jobOutput.AssetName);
                        if (locator == null)
                        {
                            mediaClient.CreateLocator(jobOutput.AssetName, jobOutput.AssetName, streamingPolicyName, mediaPublish.ContentProtection);
                        }
                    }
                    publishMessage = SendNotificationMessage(mediaPublish, job, null, null);
                }
            }
            return publishMessage;
        }
    }
}