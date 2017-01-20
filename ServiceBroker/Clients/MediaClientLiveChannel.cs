using System;
using System.Net;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.ServiceBroker
{
    public partial class MediaClient
    {
        private void CreateProgram(IChannel channel)
        {
            string assetName = channel.Name;
            AssetCreationOptions assetEncryption = AssetCreationOptions.None;
            IAsset asset = _media.Assets.Create(assetName, assetEncryption);

            ProgramCreationOptions creationOptions = new ProgramCreationOptions();
            creationOptions.Name = channel.Name;
            creationOptions.AssetId = asset.Id;

            int archiveMinutesInt;
            string settingKey = Constants.AppSettings.MediaChannelProgramArchiveMinutes;
            string archiveMinutes = AppSetting.GetValue(settingKey);
            if (int.TryParse(archiveMinutes, out archiveMinutesInt))
            {
                creationOptions.ArchiveWindowLength = new TimeSpan(0, archiveMinutesInt, 0);
            }
            channel.Programs.Create(creationOptions);
        }

        private IChannel CreateChannel(string channelName, ChannelEncodingType channelType, StreamingProtocol ingestProtocol)
        {
            ChannelCreationOptions creationOptions = new ChannelCreationOptions();
            creationOptions.Name = channelName;
            creationOptions.EncodingType = channelType;

            IPRange allAddresses = new IPRange();
            allAddresses.Address = new IPAddress(0);
            allAddresses.Name = Constants.Media.AddressesAll;
            allAddresses.SubnetPrefixLength = 0;

            creationOptions.Input = new ChannelInput();
            creationOptions.Input.AccessControl = new ChannelAccessControl();
            creationOptions.Input.AccessControl.IPAllowList = new List<IPRange>();
            creationOptions.Input.AccessControl.IPAllowList.Add(allAddresses);
            creationOptions.Input.StreamingProtocol = ingestProtocol;

            creationOptions.Preview = new ChannelPreview();
            creationOptions.Preview.AccessControl = new ChannelAccessControl();
            creationOptions.Preview.AccessControl.IPAllowList = new List<IPRange>();
            creationOptions.Preview.AccessControl.IPAllowList.Add(allAddresses);

            IChannel channel = _media.Channels.Create(creationOptions);
            CreateProgram(channel);
            return channel;
        }

        public void CreateChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                string settingKey = Constants.AppSettings.MediaLiveChannelName;
                channelName = AppSetting.GetValue(settingKey);
            }
            ChannelEncodingType channelType = ChannelEncodingType.None;
            StreamingProtocol ingestProtocol = StreamingProtocol.RTMP;
            IChannel channel = CreateChannel(channelName, channelType, ingestProtocol);
            foreach (IProgram program in channel.Programs)
            {
                CreateLocator(null, LocatorType.OnDemandOrigin, program.Asset, null);
            }
        }

        public void SignalChannel(string channelName, int cueId)
        {
            string settingKey = Constants.AppSettings.MediaChannelAdvertisementSeconds;
            string timeSeconds = AppSetting.GetValue(settingKey);
            TimeSpan timeSpan = new TimeSpan(0, 0, int.Parse(timeSeconds));

            IChannel[] channels;
            if (string.IsNullOrEmpty(channelName))
            {
                IChannel channel = GetEntityByName(MediaEntity.Channel, channelName, false) as IChannel;
                channels = new IChannel[] { channel };
            }
            else
            {
                channels = GetEntities(MediaEntity.Channel) as IChannel[];
            }
            foreach (IChannel channel in channels)
            {
                if (channel.EncodingType != ChannelEncodingType.None)
                {
                    channel.StartAdvertisement(timeSpan, cueId, true);
                }
            }
        }
    }
}
