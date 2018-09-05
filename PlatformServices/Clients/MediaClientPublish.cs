using System.Text;

using Microsoft.Rest;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;

using Newtonsoft.Json.Linq;

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

        public static void SetEventSubscription(string authToken)
        {
            string settingKey = Constant.AppSettingKey.MediaPublishJobUrl;
            string publishJobUrl = AppSetting.GetValue(settingKey);

            EventSubscription eventSubscription = new EventSubscription(name: Constant.Media.Publish.EventGrid.SubscriptionName)
            {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = publishJobUrl
                },
                Filter = new EventSubscriptionFilter()
                {
                    IncludedEventTypes = Constant.Media.Publish.EventGrid.EventTypes
                }
            };

            TokenCredentials azureToken = AuthToken.AcquireToken(authToken, out string subscriptionId);
            EventGridManagementClient eventGridClient = new EventGridManagementClient(azureToken)
            {
                SubscriptionId = subscriptionId
            };

            User userProfile = new User(authToken);
            string eventScope = userProfile.MediaAccount.ResourceId;
            eventGridClient.EventSubscriptions.CreateOrUpdate(eventScope, eventSubscription.Name, eventSubscription);
        }

        public static MediaPublish PublishJobOutput(MediaPublish mediaPublish)
        {
            using (MediaClient mediaClient = new MediaClient(null, mediaPublish.MediaAccount))
            {
                if (!string.IsNullOrEmpty(mediaPublish.ProcessState))
                {
                    string indexId = mediaPublish.Id;
                    JObject insight = mediaClient.IndexerGetInsight(indexId);
                    if (insight != null)
                    {
                        using (DatabaseClient databaseClient = new DatabaseClient())
                        {
                            string collectionId = Constant.Database.Collection.MediaInsight;
                            databaseClient.UpsertDocument(collectionId, insight);
                        }
                    }
                }
                else
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
                        mediaPublish.UserContact.NotificationMessage = GetNotificationMessage(mediaPublish, job);
                    }
                }
            }
            return mediaPublish;
        }
    }
}