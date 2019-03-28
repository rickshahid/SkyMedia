using System;

using Microsoft.Rest;
using Microsoft.Extensions.Logging;
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

        public static void SetMediaSubscription(MediaAccount mediaAccount, ILogger logger)
        {
            try
            {
                TokenCredentials authToken = AuthToken.AcquireToken(mediaAccount);
                EventGridManagementClient eventGridClient = new EventGridManagementClient(authToken)
                {
                    SubscriptionId = mediaAccount.SubscriptionId
                };

                string settingKey = Constant.AppSettingKey.MediaEventGridLiveEventUrl;
                string liveEventUrl = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.MediaEventGridPublishAssetUrl;
                string publishAssetUrl = AppSetting.GetValue(settingKey);

                string eventScope = mediaAccount.ResourceId;
                SetEventSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.LiveEventSubscriptionName, liveEventUrl, Constant.Media.EventGrid.LiveEventFilterTypes);
                SetEventSubscription(eventGridClient, eventScope, Constant.Media.EventGrid.PublishAssetSubscriptionName, publishAssetUrl, Constant.Media.EventGrid.PublishAssetFilterTypes);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogError(ex.ToString());
                }
            }
        }
    }
}