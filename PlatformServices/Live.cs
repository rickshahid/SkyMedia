using System;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Live
    {
        public static string GetSourceUrl(string channelName, bool livePreview)
        {
            string sourceUrl = string.Empty;
            string settingKey = Constant.AppSettingKey.AzureMedia;
            string[] liveAccount = AppSetting.GetValue(settingKey, true);
            if (liveAccount.Length > 0)
            {
                MediaClient mediaClient = new MediaClient(liveAccount[0], liveAccount[1]);
                IChannel channel = mediaClient.GetEntityByName(MediaEntity.Channel, channelName, true) as IChannel;
                if (channel != null && channel.State == ChannelState.Running)
                {
                    if (livePreview)
                    {
                        ChannelEndpoint endpoint = channel.Preview.Endpoints.FirstOrDefault();
                        if (endpoint != null)
                        {
                            sourceUrl = endpoint.Url.ToString();
                        }
                    }
                    else
                    {
                        IProgram program = channel.Programs.FirstOrDefault();
                        if (program != null && program.State == ProgramState.Running)
                        {
                            sourceUrl = mediaClient.GetLocatorUrl(program.Asset, LocatorType.OnDemandOrigin, null);
                        }
                    }
                }
            }
            return sourceUrl;
        }

        public static DateTime? GetEventStart(string accountName, string channelName)
        {
            DateTime? eventStart = null;
            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.LiveEvent;
            LiveEvent liveEvent = entityClient.GetEntity<LiveEvent>(tableName, accountName, channelName);
            if (liveEvent != null)
            {
                eventStart = liveEvent.StartTime;
            }
            return eventStart;
        }
    }
}
