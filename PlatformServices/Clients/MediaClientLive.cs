using System;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public LiveEvent CreateLiveEvent(string eventName, string eventDescription, string eventTags, LiveEventInputProtocol inputProtocol,
                                         LiveEventEncodingType encodingType, string encodingPresetName, string streamingPolicyName,
                                         bool lowLatency, bool autoStart)
        {
            LiveEventInput eventInput = new LiveEventInput()
            {
                StreamingProtocol = inputProtocol
            };
            LiveEventEncoding eventEncoding = new LiveEventEncoding()
            {
                EncodingType = encodingType,
                PresetName = encodingPresetName
            };
            LiveEventPreview eventPreview = new LiveEventPreview()
            {
                StreamingPolicyName = streamingPolicyName
            };
            List<StreamOptionsFlag?> streamOptions = new List<StreamOptionsFlag?>();
            if (lowLatency)
            {
               streamOptions.Add(StreamOptionsFlag.LowLatency);
            }
            MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
            LiveEvent liveEvent = new LiveEvent()
            {
                VanityUrl = true,
                Description = eventDescription,
                Tags = GetDataItems(eventTags),
                Input = eventInput,
                Encoding = eventEncoding,
                Preview = eventPreview,
                StreamOptions = streamOptions,
                Location = mediaService.Location
            };
            return _media.LiveEvents.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, liveEvent, autoStart);
        }

        public LiveOutput CreateLiveOutput(string eventName, string eventOutputName, string eventOutputDescription,
                                           string eventOutputAssetName, int eventArchiveWindowMinutes)
        {
            CreateAsset(null, eventOutputAssetName);
            CreateLocator(eventOutputAssetName, eventOutputAssetName, PredefinedStreamingPolicy.ClearStreamingOnly, null);
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
                Tags = GetDataItems(eventTags),
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

        public void InsertLiveEventSignal(string eventName, int signalId, int durationSeconds)
        {
            // TODO: Implement live event ad signaling
        }
    }
}