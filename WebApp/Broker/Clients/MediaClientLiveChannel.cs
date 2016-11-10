using System;
using System.Net;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        public IChannel CreateChannel(string channelName, ChannelEncodingType channelType, StreamingProtocol ingestProtocol)
        {
            ChannelCreationOptions creationOptions = new ChannelCreationOptions();
            creationOptions.Name = channelName;
            creationOptions.EncodingType = channelType;

            IPRange allAddresses = new IPRange();
            allAddresses.Address = new IPAddress(0);
            allAddresses.Name = Constants.Media.AddressesAll;
                                        
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
    }
}
