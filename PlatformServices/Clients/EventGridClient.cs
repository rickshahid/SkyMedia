using Microsoft.Rest;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal class EventGridClient
    {
        private static void SetEventGridSubscription(EventGridManagementClient eventGridClient, string eventScope, string eventSubscriptionName, string eventUrl, string[] eventTypes)
        {
            EventSubscription eventSubscription = new EventSubscription(name: eventSubscriptionName)
            {
                Destination = new WebHookEventSubscriptionDestination()
                {
                    EndpointUrl = eventUrl
                },
                Filter = new EventSubscriptionFilter()
                {
                    IncludedEventTypes = eventTypes
                }
            };
            eventGridClient.EventSubscriptions.CreateOrUpdate(eventScope, eventSubscription.Name, eventSubscription);
        }

        public static void SetEventGridSubscriptions(string authToken)
        {
            string settingKey = Constant.AppSettingKey.MediaEventGridLiveUrl;
            string liveUrl = AppSetting.GetValue(settingKey);

            settingKey = Constant.AppSettingKey.MediaEventGridPublishUrl;
            string publishUrl = AppSetting.GetValue(settingKey);

            TokenCredentials azureToken = AuthToken.AcquireToken(authToken, out string subscriptionId);
            EventGridManagementClient eventGridClient = new EventGridManagementClient(azureToken)
            {
                SubscriptionId = subscriptionId
            };

            User userProfile = new User(authToken);
            string eventScope = userProfile.MediaAccount.ResourceId;

            SetEventGridSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.LiveSubscriptionName, liveUrl, Constant.Media.EventGrid.LiveEventTypes);
            SetEventGridSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.PublishSubscriptionName, publishUrl, Constant.Media.EventGrid.PublishEventTypes);
        }
    }
}