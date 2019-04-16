using System.Linq;

using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal class EventGridClient
    {
        private static EventGridManagementClient GetEventGridClient(MediaAccount mediaAccount)
        {
            //TokenCredentials authToken = AuthToken.AcquireToken(mediaAccount);
            ServiceClientCredentials clientCredentials = ApplicationTokenProvider.LoginSilentAsync(mediaAccount.DirectoryTenantId, mediaAccount.ServicePrincipalId, mediaAccount.ServicePrincipalKey).Result;
            EventGridManagementClient eventGridClient = new EventGridManagementClient(clientCredentials)
            {
                SubscriptionId = mediaAccount.SubscriptionId
            };
            return eventGridClient;
        }

        private static void SetEventSubscription(EventGridManagementClient eventGridClient, string eventScope, string eventSubscriptionName, string eventHandlerUrl, string[] eventFilterTypes, string subjectBeginsWith)
        {
            EventSubscription eventSubscription = new EventSubscription(name: eventSubscriptionName)
            {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = eventHandlerUrl
                },
                Filter = new EventSubscriptionFilter()
                {
                    IncludedEventTypes = eventFilterTypes,
                    SubjectBeginsWith = subjectBeginsWith
                }
            };
            eventGridClient.EventSubscriptions.CreateOrUpdate(eventScope, eventSubscription.Name, eventSubscription);
        }

        public static void SetStorageSubscription(MediaAccount mediaAccount)
        {
            EventGridManagementClient eventGridClient = GetEventGridClient(mediaAccount);

            StorageBlobClient blobClient = new StorageBlobClient(mediaAccount, null);
            blobClient.ListBlobContainer(Constant.Storage.Blob.WorkflowContainerName, null);

            string settingKey = Constant.AppSettingKey.MediaEventGridStorageBlobCreatedUrl;
            string storageBlobCreatedUrl = AppSetting.GetValue(settingKey);

            string storageAccount = mediaAccount.StorageAccounts.First().Key;
            string eventScope = string.Format(Constant.Storage.AccountResourceId, mediaAccount.SubscriptionId, mediaAccount.ResourceGroupName, storageAccount);
            string subjectBeginsWith = string.Concat(Constant.Storage.Blob.WorkflowContainers, Constant.Storage.Blob.WorkflowContainerName);
            SetEventSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.StorageBlobCreatedSubscriptionName, storageBlobCreatedUrl, Constant.Media.EventGrid.StorageBlobCreatedSubscriptionEvents, subjectBeginsWith);
        }

        public static void SetMediaSubscription(MediaAccount mediaAccount)
        {
            EventGridManagementClient eventGridClient = GetEventGridClient(mediaAccount);

            string settingKey = Constant.AppSettingKey.MediaEventGridLiveEventUrl;
            string jobOutputProgressUrl = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.MediaEventGridJobStateFinalUrl;
            string jobStateFinalUrl = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.MediaEventGridLiveEventUrl;
            string liveEventUrl = AppSetting.GetValue(settingKey);

            string eventScope = mediaAccount.ResourceId;
            SetEventSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.JobOutputProgressSubscriptionName, jobOutputProgressUrl, Constant.Media.EventGrid.JobOutputProgressSubscriptionEvents, null);
            SetEventSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.JobStateFinalSubscriptionName, jobStateFinalUrl, Constant.Media.EventGrid.JobStateFinalSubscriptionEvents, null);
            SetEventSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.LiveEventSubscriptionName, liveEventUrl, Constant.Media.EventGrid.LiveEventSubscriptionEvents, null);
        }
    }
}