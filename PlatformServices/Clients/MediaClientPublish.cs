using System.Text;

using Microsoft.Rest;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static string GetNotificationMessage(Job job, MediaJobAccount jobAccount, MediaJobPublish jobPublish)
        {
            StringBuilder message = new StringBuilder();
            message.Append("AMS Job Notification");
            message.Append("\n\nMedia Services Account\n");
            message.Append(jobAccount.MediaAccount.Name);
            message.Append("\n\nTransform Name\n");
            message.Append(jobAccount.TransformName);
            message.Append("\n\nJob Name\n");
            message.Append(job.Name);
            message.Append("\n\nJob State\n");
            message.Append(job.State.ToString());
            message.Append("\n\nJob Priority\n");
            message.Append(job.Priority);
            message.Append("\n\nJob Created\n");
            message.Append(job.Created);
            message.Append("\n\nJob Last Modified\n");
            message.Append(job.LastModified);
            foreach (JobOutput jobOutput in job.Outputs)
            {
                message.Append("\n\nJob Output Label\n");
                message.Append(jobOutput.Label);
                message.Append("\n\nJob Output State\n");
                message.Append(jobOutput.State.ToString());
                message.Append("\n\nJob Output Progress\n");
                message.Append(jobOutput.Progress);
                if (jobOutput.Error != null)
                {
                    message.Append("\n\nOutput Error Category\n");
                    message.Append(jobOutput.Error.Category.ToString());
                    message.Append("\n\nOutput Error Code\n");
                    message.Append(jobOutput.Error.Code.ToString());
                    message.Append("\n\nOutput Error Message\n");
                    message.Append(jobOutput.Error.Message);
                    message.Append("\n\nOutput Error Retry\n");
                    message.Append(jobOutput.Error.Retry);
                    foreach (JobErrorDetail errorDetail in jobOutput.Error.Details)
                    {
                        message.Append("\n\nOutput Error Detail Code\n");
                        message.Append(errorDetail.Code.ToString());
                        message.Append("\n\nOutput Error Detail Message\n");
                        message.Append(errorDetail.Message);
                    }
                }
            }
            message.Append("\n\nStreaming Policy Name\n");
            message.Append(jobPublish.StreamingPolicyName);
            return message.ToString();
        }

        private static MediaJobPublish GetJobPublish(Job job)
        {
            string jobPublish = job.CorrelationData[Constant.Media.Job.OutputPublish];
            return JsonConvert.DeserializeObject<MediaJobPublish>(jobPublish);
        }

        public static JObject SetJobPublish(string authToken, JObject jobData, string streamingPolicyName)
        {
            string mobilePhoneNumber = null;
            if (!string.IsNullOrEmpty(authToken))
            {
                User userProfile = new User(authToken);
                mobilePhoneNumber = userProfile.MobilePhoneNumber;
            }
            MediaJobPublish jobPublish = new MediaJobPublish()
            {
                StreamingPolicyName = streamingPolicyName,
                ContentProtection = null,
                UserNotification = new UserNotification()
                {
                    MobilePhoneNumber = mobilePhoneNumber
                }
            };
            if (jobData == null)
            {
                jobData = new JObject();
            }
            jobData[Constant.Media.Job.OutputPublish] = JObject.FromObject(jobPublish);
            return jobData;
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

        public static MediaJobPublish PublishJobOutput(string jobName, string indexId)
        {
            MediaJobAccount jobAccount;
            MediaJobPublish jobPublish = null;
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.MediaJobAccount;
                string documentId = string.IsNullOrEmpty(jobName) ? indexId : jobName;
                jobAccount = databaseClient.GetDocument<MediaJobAccount>(collectionId, documentId);
            }
            if (jobAccount != null)
            {
                using (MediaClient mediaClient = new MediaClient(null, jobAccount.MediaAccount))
                {
                    if (!string.IsNullOrEmpty(indexId))
                    {
                        JObject insight = mediaClient.IndexerGetInsight(indexId);
                        if (insight != null)
                        {
                            using (DatabaseClient databaseClient = new DatabaseClient())
                            {
                                string collectionId = Constant.Database.Collection.MediaContentInsight;
                                databaseClient.UpsertDocument(collectionId, insight);
                            }
                        }
                    }
                    else
                    {
                        Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, jobAccount.TransformName);
                        if (job != null)
                        {
                            jobPublish = GetJobPublish(job);
                            jobPublish.UserNotification.JobOutputMessage = GetNotificationMessage(job, jobAccount, jobPublish);
                            string streamingPolicyName = jobPublish.StreamingPolicyName;
                            if (string.IsNullOrEmpty(streamingPolicyName))
                            {
                                streamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly;
                            }
                            foreach (JobOutputAsset jobOutput in job.Outputs)
                            {
                                StreamingLocator locator = mediaClient.GetEntity<StreamingLocator>(MediaEntity.StreamingLocator, jobOutput.AssetName);
                                if (locator == null)
                                {
                                    mediaClient.CreateLocator(jobOutput.AssetName, jobOutput.AssetName, streamingPolicyName, jobPublish.ContentProtection);
                                }
                            }
                        }
                    }
                }
            }
            return jobPublish;
        }
    }
}