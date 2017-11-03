using System;
using System.Net;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private void CreateProgram(IChannel channel, IAsset asset, int archiveWindowMinutes)
        {
            if (archiveWindowMinutes == 0)
            {
                string settingKey = Constant.AppSettingKey.MediaChannelProgramArchiveMinutes;
                string programArchiveMinutes = AppSetting.GetValue(settingKey);
                archiveWindowMinutes = int.Parse(programArchiveMinutes);
            }
            ProgramCreationOptions programOptions = new ProgramCreationOptions()
            {
                Name = asset.Name,
                AssetId = asset.Id,
                ArchiveWindowLength = new TimeSpan(0, archiveWindowMinutes, 0)
            };
            channel.Programs.Create(programOptions);
        }

        private void CreatePrograms(IChannel channel, int archiveWindowMinutes)
        {
            string assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramClearSuffix);
            IAsset assetClear = _media.Assets.Create(assetName, AssetCreationOptions.None);

            assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramAesSuffix);
            IAsset assetAes = _media.Assets.Create(assetName, AssetCreationOptions.StorageEncrypted);
            ContentProtection contentProtectionAes = new ContentProtection()
            {
                ContentAuthTypeToken = true,
                Aes = true
            };
            AddDeliveryPolicies(assetAes, contentProtectionAes);

            assetName = string.Concat(channel.Name, Constant.Media.Live.ProgramDrmSuffix);
            IAsset assetDrm = _media.Assets.Create(assetName, AssetCreationOptions.StorageEncrypted);
            ContentProtection contentProtectionDrm = new ContentProtection()
            {
                ContentAuthTypeToken = true,
                DrmPlayReady = true,
                DrmWidevine = true
            };
            AddDeliveryPolicies(assetDrm, contentProtectionDrm);

            CreateProgram(channel, assetClear, archiveWindowMinutes);
            CreateProgram(channel, assetAes, archiveWindowMinutes);
            CreateProgram(channel, assetDrm, archiveWindowMinutes);
        }

        private IChannel CreateChannel(string channelName, ChannelEncodingType channelType, StreamingProtocol channelProtocol,
                                       string authInputAddress, string authPreviewAddress)
        {
            IPRange inputAddressRange = new IPRange();
            inputAddressRange.Name = Constant.Media.Live.AllowAnyAddress;
            inputAddressRange.Address = new IPAddress(0);
            inputAddressRange.SubnetPrefixLength = 0;
            if (!string.IsNullOrEmpty(authInputAddress))
            {
                inputAddressRange.Name = Constant.Media.Live.AllowAuthorizedAddress;
                inputAddressRange.Address = IPAddress.Parse(authInputAddress);
            }

            IPRange previewAddressRange = new IPRange();
            previewAddressRange.Name = Constant.Media.Live.AllowAnyAddress;
            previewAddressRange.Address = new IPAddress(0);
            previewAddressRange.SubnetPrefixLength = 0;
            if (!string.IsNullOrEmpty(authPreviewAddress))
            {
                previewAddressRange.Name = Constant.Media.Live.AllowAuthorizedAddress;
                previewAddressRange.Address = IPAddress.Parse(authPreviewAddress);
            }

            ChannelCreationOptions channelOptions = new ChannelCreationOptions()
            {
                EncodingType = channelType,
                Name = channelName,
                Input = new ChannelInput()
                {
                    StreamingProtocol = channelProtocol,
                    AccessControl = new ChannelAccessControl()
                    {
                        IPAllowList = new IPRange[] { inputAddressRange }
                    }
                },
                Preview = new ChannelPreview()
                {
                    AccessControl = new ChannelAccessControl()
                    {
                        IPAllowList = new IPRange[] { previewAddressRange }
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

        public string CreateChannel(string channelName, MediaEncoding channelEncoding, MediaProtocol inputProtocol,
                                    string authInputAddress, string authPreviewAddress, int archiveWindowMinutes)
        {
            ChannelEncodingType channelType = (ChannelEncodingType)channelEncoding;
            StreamingProtocol channelProtocol = (StreamingProtocol)inputProtocol;
            IChannel channel = CreateChannel(channelName, channelType, channelProtocol, authInputAddress, authPreviewAddress);
            CreatePrograms(channel, archiveWindowMinutes);
            foreach (IProgram program in channel.Programs)
            {
                CreateLocator(LocatorType.OnDemandOrigin, program.Asset);
            }
            return channel.Id;
        }
    }
}