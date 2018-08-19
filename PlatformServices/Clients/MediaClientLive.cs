using System;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public LiveEvent CreateLiveEvent(string eventName, string eventDescription, string inputAccessToken, LiveEventInputProtocol inputProtocol, LiveEventEncodingType encodingType,
                                         string encodingPresetName, string previewStreamingPolicyName, string previewAllowedIpAddress, int previewAllowedSubnetPrefixLength,
                                         bool lowLatency, bool autoStart)
        {
            LiveEventInput eventInput = new LiveEventInput()
            {
                AccessToken = inputAccessToken,
                StreamingProtocol = inputProtocol
            };
            LiveEventPreview eventPreview = new LiveEventPreview
            {
                StreamingPolicyName = previewStreamingPolicyName,
                AccessControl = new LiveEventPreviewAccessControl()
                {
                    Ip = new IPAccessControl()
                    {
                        Allow = new List<IPRange>()
                        {
                            new IPRange()
                            {
                                Name = Constant.Media.Live.AllowedPreviewAccess,
                                Address = previewAllowedIpAddress,
                                SubnetPrefixLength = previewAllowedSubnetPrefixLength
                            }
                        }
                    }
                }
            };
            LiveEventEncoding eventEncoding = new LiveEventEncoding()
            {
                EncodingType = encodingType,
                PresetName = encodingPresetName
            };
            List<StreamOptionsFlag?> eventStreamOptions = new List<StreamOptionsFlag?>();
            if (lowLatency)
            {
                eventStreamOptions.Add(StreamOptionsFlag.LowLatency);
            }
            MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
            LiveEvent liveEvent = new LiveEvent()
            {
                Description = eventDescription,
                Preview = eventPreview,
                Input = eventInput,
                Encoding = eventEncoding,
                StreamOptions = eventStreamOptions,
                Location = mediaService.Location,
                VanityUrl = true
            };
            return _media.LiveEvents.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, liveEvent, autoStart);
        }

        public void StartLiveEvent(string eventName)
        {
            _media.LiveEvents.Start(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName);
        }

        public void StopLiveEvent(string eventName)
        {
            _media.LiveEvents.Stop(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName);
        }

        public LiveOutput CreateLiveEventOutput(string eventName, string eventOutputName, string eventOutputDescription, string manifestName, string archiveAssetName, int archiveAssetWindowMinutes)
        {
            CreateAsset(null, archiveAssetName);
            LiveOutput eventOutput = new LiveOutput()
            {
                Description = eventOutputDescription,
                ManifestName = manifestName,
                AssetName = archiveAssetName,
                ArchiveWindowLength = new TimeSpan(0, archiveAssetWindowMinutes, 0)
            };
            return _media.LiveOutputs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, eventOutputName, eventOutput);
        }
    }
}