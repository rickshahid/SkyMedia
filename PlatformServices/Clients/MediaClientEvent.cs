using System.Text;
using System.Collections.Generic;

using Microsoft.Rest;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static string GetNotificationMessage(MediaAccount mediaAccount, Job job)
        {
            StringBuilder message = new StringBuilder();
            message.Append("AMS Transform Job Notification");
            message.Append("\n\nMedia Account Name: ");
            message.Append(mediaAccount.Name);
            message.Append("\n\nJob Name: ");
            message.Append(job.Name);
            message.Append("\n\nJob State: ");
            message.Append(job.State.ToString());
            foreach (JobOutput jobOutput in job.Outputs)
            {
                message.Append("\n\nJob Output State: ");
                message.Append(jobOutput.State.ToString());
                message.Append("\n\nJob Output Progress: ");
                message.Append(jobOutput.Progress);
                if (jobOutput.Error != null)
                {
                    message.Append("\n\nError Category: ");
                    message.Append(jobOutput.Error.Category.ToString());
                    message.Append("\n\nError Code: ");
                    message.Append(jobOutput.Error.Code.ToString());
                    message.Append("\n\nError Message: ");
                    message.Append(jobOutput.Error.Message);
                    foreach (JobErrorDetail errorDetail in jobOutput.Error.Details)
                    {
                        message.Append("\n\nError Detail Code: ");
                        message.Append(errorDetail.Code.ToString());
                        message.Append("\n\nError Detail Message: ");
                        message.Append(errorDetail.Message);
                    }
                }
            }
            return message.ToString();
        }

        public static void SetEventSubscription(string authToken, string subscriptionName, IList<string> eventTypes)
        {
            EventSubscription eventSubscription = new EventSubscription(name: subscriptionName)
            {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = AppSetting.GetValue(Constant.AppSettingKey.MediaPublishUrl)
                },
                Filter = new EventSubscriptionFilter()
                {
                    IncludedEventTypes = eventTypes
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
    }
}