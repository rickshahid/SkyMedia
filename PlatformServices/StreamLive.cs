using System;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class StreamLive
    {
        public static string GetSourceUrl(string channelName, bool livePreview)
        {
            string liveSourceUrl = string.Empty;
            string settingKey = Constants.AppSettingKey.AzureMedia;
            string[] liveAccount = AppSetting.GetValue(settingKey, true);
            if (liveAccount.Length > 0)
            {
                MediaClient mediaClient = new MediaClient(liveAccount[0], liveAccount[1]);
                IChannel channel = mediaClient.GetEntityByName(MediaEntity.Channel, channelName, true) as IChannel;
                if (channel != null && channel.State == ChannelState.Running)
                {
                    if (livePreview)
                    {
                        liveSourceUrl = channel.Preview.Endpoints.First().Url.ToString();
                    }
                    else
                    {
                        IProgram program = channel.Programs.First();
                        if (program.State == ProgramState.Running)
                        {
                            liveSourceUrl = mediaClient.GetLocatorUrl(program.Asset, LocatorType.OnDemandOrigin, null);
                        }
                    }
                }
            }
            return liveSourceUrl;
        }

        public static DateTime? GetEventStart(string accountName, string channelName)
        {
            DateTime? eventStart = null;
            EntityClient entityClient = new EntityClient();
            string tableName = Constants.Storage.TableName.LiveEvent;
            MediaLiveEvent liveEvent = entityClient.GetEntity<MediaLiveEvent>(tableName, accountName, channelName);
            if (liveEvent != null)
            {
                eventStart = liveEvent.EventStart;
            }
            return eventStart;
        }
    }
}
