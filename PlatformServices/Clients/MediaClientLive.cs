using System;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public LiveEvent CreateLiveEvent(string eventName, string eventDescription, string eventTags, string inputStreamId,
                                         LiveEventInputProtocol inputProtocol, LiveEventEncodingType encodingType, string encodingPresetName,
                                         string streamingPolicyName, bool lowLatency, bool autoStart)
        {
            LiveEventInput eventInput = new LiveEventInput()
            {
                AccessToken = inputStreamId,
                StreamingProtocol = inputProtocol,
                AccessControl = new LiveEventInputAccessControl()
                {
                    Ip = new IPAccessControl()
                    {
                        Allow = new IPRange[]
                        {
                            new IPRange()
                            {
                                Name = "Any IP Address",
                                Address = "0.0.0.0",
                                SubnetPrefixLength = 0
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
            LiveEventPreview eventPreview = new LiveEventPreview()
            {
                PreviewLocator = inputStreamId,
                StreamingPolicyName = streamingPolicyName
            };
            List<StreamOptionsFlag?> streamOptions = new List<StreamOptionsFlag?>();
            if (lowLatency)
            {
                streamOptions.Add(StreamOptionsFlag.LowLatency);
            }
            else
            {
                streamOptions.Add(StreamOptionsFlag.Default);
            }
            MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
            LiveEvent liveEvent = new LiveEvent()
            {
                VanityUrl = true,
                Description = eventDescription,
                Tags = GetCorrelationData(eventTags, false),
                Input = eventInput,
                Encoding = eventEncoding,
                Preview = eventPreview,
                StreamOptions = streamOptions,
                Location = mediaService.Location
            };
            return _media.LiveEvents.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, liveEvent, autoStart);
        }

        public LiveOutput CreateLiveOutput(string eventName, string eventOutputName, string eventOutputDescription, string eventOutputAssetStorage,
                                           string eventOutputAssetName, int eventArchiveWindowMinutes)
        {
            CreateAsset(eventOutputAssetStorage, eventOutputAssetName);
            LiveOutput eventOutput = new LiveOutput()
            {
                Description = eventOutputDescription,
                AssetName = eventOutputAssetName,
                ArchiveWindowLength = new TimeSpan(0, eventArchiveWindowMinutes, 0)
            };
            return _media.LiveOutputs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, eventOutputName, eventOutput);
        }

        public LiveEvent UpdateLiveEvent(string eventName, string eventDescription, string eventTags,
                                         LiveEventEncodingType encodingType, string encodingPresetName,
                                         string keyFrameIntervalDuration, CrossSiteAccessPolicies crossSiteAccessPolicies)
        {
            MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
            LiveEvent liveEvent = new LiveEvent()
            {
                Description = eventDescription,
                Tags = GetCorrelationData(eventTags, false),
                CrossSiteAccessPolicies = crossSiteAccessPolicies,
                Location = mediaService.Location
            };
            liveEvent.Encoding = new LiveEventEncoding()
            {
                EncodingType = encodingType,
                PresetName = encodingPresetName
            };
            liveEvent.Input = new LiveEventInput()
            {
                KeyFrameIntervalDuration = keyFrameIntervalDuration
            };
            return _media.LiveEvents.Update(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, liveEvent);
        }

        public void StartLiveEvent(string eventName)
        {
            _media.LiveEvents.Start(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName);
        }

        public void StopLiveEvent(string eventName)
        {
            _media.LiveEvents.Stop(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName);
        }

        public void ResetLiveEvent(string eventName)
        {
            _media.LiveEvents.Reset(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName);
        }

        public string GetLiveOutputUrl(LiveEvent liveEvent)
        {
            string liveOutputUrl = string.Empty;
            MediaLiveEvent mediaLiveEvent = new MediaLiveEvent(this, liveEvent);
            if (mediaLiveEvent.Outputs.Length > 0)
            {
                LiveOutput liveOutput = mediaLiveEvent.Outputs[0];
                string[] streamingUrls = GetStreamingUrls(liveOutput.AssetName);
                if (streamingUrls.Length > 0)
                {
                    liveOutputUrl = streamingUrls[0];
                }
            }
            return liveOutputUrl;
        }

        public void InsertLiveEventSignal(string eventName, string signalId, int signalDurationSeconds)
        {
            // TODO: Implement v3 live event (encoding) ad signaling
        }
    }
}