﻿using System;
using System.Net;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private void CreateProgram(IChannel channel, IAsset asset)
        {
            ProgramCreationOptions programOptions = new ProgramCreationOptions();
            programOptions.Name = asset.Name;
            programOptions.AssetId = asset.Id;

            int archiveMinutes;
            string settingKey = Constant.AppSettingKey.MediaChannelProgramArchiveMinutes;
            string settingValue = AppSetting.GetValue(settingKey);
            if (int.TryParse(settingValue, out archiveMinutes))
            {
                programOptions.ArchiveWindowLength = new TimeSpan(0, archiveMinutes, 0);
            }

            channel.Programs.Create(programOptions);
        }

        private void CreatePrograms(IChannel channel)
        {
            string assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramClearSuffix);
            IAsset assetClear = _media.Assets.Create(assetName, AssetCreationOptions.None);

            ContentProtection contentProtectionAes = new ContentProtection();
            contentProtectionAes.ContentAuthTypeToken = true;
            contentProtectionAes.Aes = true;

            assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramAesSuffix);
            IAsset assetAes = _media.Assets.Create(assetName, AssetCreationOptions.StorageEncrypted);
            AddDeliveryPolicies(assetAes, contentProtectionAes);

            ContentProtection contentProtectionDrm = new ContentProtection();
            contentProtectionDrm.ContentAuthTypeToken = true;
            contentProtectionDrm.DrmPlayReady = true;
            contentProtectionDrm.DrmWidevine = true;

            assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramDrmSuffix);
            IAsset assetDrm = _media.Assets.Create(assetName, AssetCreationOptions.StorageEncrypted);
            AddDeliveryPolicies(assetDrm, contentProtectionDrm);

            CreateProgram(channel, assetClear);
            CreateProgram(channel, assetAes);
            CreateProgram(channel, assetDrm);
        }

        private IChannel CreateChannel(string channelName, ChannelEncodingType channelType, StreamingProtocol ingestProtocol, string allowedAddresses)
        {
            ChannelCreationOptions creationOptions = new ChannelCreationOptions();
            creationOptions.EncodingType = channelType;
            if (channelType == ChannelEncodingType.None)
            {
                creationOptions.Name = string.Concat(channelName, Constant.Media.Live.ChannelStreamingSuffix);
            }
            else
            {
                creationOptions.Name = string.Concat(channelName, Constant.Media.Live.ChannelEncodingSuffix);
                creationOptions.Encoding = new ChannelEncoding();
                creationOptions.Encoding.SystemPreset = Constant.Media.Live.ChannelEncodingPreset;
            }

            IPRange allowedAddressRange = new IPRange();
            if (string.IsNullOrEmpty(allowedAddresses))
            {
                allowedAddressRange.Name = Constant.Media.Live.AllowAllAddresses;
                allowedAddressRange.Address = new IPAddress(0);
                allowedAddressRange.SubnetPrefixLength = 0;
            }
            else
            {
                allowedAddressRange.Name = Constant.Media.Live.AllowAddresses;
                allowedAddressRange.Address = IPAddress.Parse(allowedAddresses);
            }

            creationOptions.Input = new ChannelInput();
            creationOptions.Input.AccessControl = new ChannelAccessControl();
            creationOptions.Input.AccessControl.IPAllowList = new List<IPRange>();
            creationOptions.Input.AccessControl.IPAllowList.Add(allowedAddressRange);
            creationOptions.Input.StreamingProtocol = ingestProtocol;

            creationOptions.Preview = new ChannelPreview();
            creationOptions.Preview.AccessControl = new ChannelAccessControl();
            creationOptions.Preview.AccessControl.IPAllowList = new List<IPRange>();
            creationOptions.Preview.AccessControl.IPAllowList.Add(allowedAddressRange);

            return _media.Channels.Create(creationOptions);
        }

        public void CreateChannel(string channelName, MediaEncoding encodingType, string allowedAddresses)
        {
            ChannelEncodingType channelType = (ChannelEncodingType)Enum.Parse(typeof(ChannelEncodingType), encodingType.ToString());
            StreamingProtocol ingestProtocol = StreamingProtocol.RTMP;
            IChannel channel = CreateChannel(channelName, channelType, ingestProtocol, allowedAddresses);
            CreatePrograms(channel);
            foreach (IProgram program in channel.Programs)
            {
                CreateLocator(LocatorType.OnDemandOrigin, program.Asset);
            }
        }
    }
}
