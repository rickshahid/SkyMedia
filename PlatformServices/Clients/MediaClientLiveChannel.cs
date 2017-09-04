using System;
using System.Net;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private void CreateProgram(IChannel channel, IAsset asset)
        {
            ProgramCreationOptions programOptions = new ProgramCreationOptions()
            {
                Name = asset.Name,
                AssetId = asset.Id
            };

            string settingKey = Constant.AppSettingKey.MediaChannelProgramArchiveMinutes;
            string settingValue = AppSetting.GetValue(settingKey);
            if (int.TryParse(settingValue, out int archiveMinutes))
            {
                programOptions.ArchiveWindowLength = new TimeSpan(0, archiveMinutes, 0);
            }

            channel.Programs.Create(programOptions);
        }

        private void CreatePrograms(IChannel channel)
        {
            string assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramClearSuffix);
            IAsset assetClear = _media.Assets.Create(assetName, AssetCreationOptions.None);

            ContentProtection contentProtectionAes = new ContentProtection()
            {
                ContentAuthTypeToken = true,
                Aes = true
            };

            assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramAesSuffix);
            IAsset assetAes = _media.Assets.Create(assetName, AssetCreationOptions.StorageEncrypted);
            AddDeliveryPolicies(assetAes, contentProtectionAes);

            ContentProtection contentProtectionDrm = new ContentProtection()
            {
                ContentAuthTypeToken = true,
                DrmPlayReady = true,
                DrmWidevine = true
            };

            assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramDrmSuffix);
            IAsset assetDrm = _media.Assets.Create(assetName, AssetCreationOptions.StorageEncrypted);
            AddDeliveryPolicies(assetDrm, contentProtectionDrm);

            CreateProgram(channel, assetClear);
            CreateProgram(channel, assetAes);
            CreateProgram(channel, assetDrm);
        }

        private IChannel CreateChannel(string channelName, ChannelEncodingType channelType, StreamingProtocol ingestProtocol, string allowedAddresses)
        {
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

            ChannelCreationOptions channelOptions = new ChannelCreationOptions()
            {
                EncodingType = channelType,
                Name = channelName,
                Input = new ChannelInput()
                {
                    StreamingProtocol = ingestProtocol,
                    AccessControl = new ChannelAccessControl()
                    {
                        IPAllowList = new IPRange[] { allowedAddressRange }
                    }
                },
                Preview = new ChannelPreview()
                {
                    AccessControl = new ChannelAccessControl()
                    {
                        IPAllowList = new IPRange[] { allowedAddressRange }
                    }
                }
            };

            if (channelType != ChannelEncodingType.None)
            {
                channelOptions.Encoding = new ChannelEncoding()
                {
                    SystemPreset = Constant.Media.Live.ChannelEncodingPreset
                };
            }

            return _media.Channels.Create(channelOptions);
        }

        public string CreateChannel(string channelName, MediaEncoding channelEncoding, string allowedAddresses)
        {
            ChannelEncodingType channelType = (ChannelEncodingType)Enum.Parse(typeof(ChannelEncodingType), channelEncoding.ToString());
            StreamingProtocol ingestProtocol = StreamingProtocol.RTMP;
            IChannel channel = CreateChannel(channelName, channelType, ingestProtocol, allowedAddresses);
            CreatePrograms(channel);
            foreach (IProgram program in channel.Programs)
            {
                CreateLocator(LocatorType.OnDemandOrigin, program.Asset);
            }
            return channel.Id;
        }
    }
}