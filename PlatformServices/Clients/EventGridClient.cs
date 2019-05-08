using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal class EventGridClient
    {
        private static void SetEventSubscription(EventGridManagementClient eventGridClient, string eventScope, string eventSubscriptionName, string eventHandlerUrl, string[] eventFilterTypes)
        {
            EventSubscription eventSubscription = new EventSubscription(name: eventSubscriptionName)
            {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = eventHandlerUrl
                },
                Filter = new EventSubscriptionFilter()
                {
                    IncludedEventTypes = eventFilterTypes
                }
            };
            eventGridClient.EventSubscriptions.CreateOrUpdate(eventScope, eventSubscription.Name, eventSubscription);
        }

        public static void SetMediaSubscription(MediaAccount mediaAccount)
        {
            ServiceClientCredentials clientCredentials = ApplicationTokenProvider.LoginSilentAsync(mediaAccount.DirectoryTenantId, mediaAccount.ServicePrincipalId, mediaAccount.ServicePrincipalKey).Result;
            EventGridManagementClient eventGridClient = new EventGridManagementClient(clientCredentials)
            {
                SubscriptionId = mediaAccount.SubscriptionId
            };

            string settingKey = Constant.AppSettingKey.EventGridLiveEventUrl;
            string jobOutputProgressUrl = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.EventGridJobStateFinalUrl;
            string jobStateFinalUrl = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.EventGridLiveEventUrl;
            string liveEventUrl = AppSetting.GetValue(settingKey);

            string eventScope = mediaAccount.ResourceId;
            SetEventSubscription(eventGridClient, eventScope, Constant.EventGrid.JobOutputProgressSubscriptionName, jobOutputProgressUrl, Constant.EventGrid.JobOutputProgressSubscriptionEvents);
            SetEventSubscription(eventGridClient, eventScope, Constant.EventGrid.JobStateFinalSubscriptionName, jobStateFinalUrl, Constant.EventGrid.JobStateFinalSubscriptionEvents);
            SetEventSubscription(eventGridClient, eventScope, Constant.EventGrid.LiveEventSubscriptionName, liveEventUrl, Constant.EventGrid.LiveEventSubscriptionEvents);
        }
    }
}