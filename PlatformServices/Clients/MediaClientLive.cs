using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public LiveEvent CreateLiveEvent(string eventName, string eventDescription, LiveEventEncodingType encodingType, string encodingPresetName,
                                         LiveEventInputProtocol inputProtocol, bool lowLatency, bool autoStart)
        {
            LiveEventEncoding eventEncoding = new LiveEventEncoding()
            {
                EncodingType = encodingType,
                PresetName = encodingPresetName
            };
            LiveEventInput eventInput = new LiveEventInput()
            {
                StreamingProtocol = inputProtocol
            };
            List<StreamOptionsFlag?> streamOptions = new List<StreamOptionsFlag?>();
            if (lowLatency)
            {
                streamOptions.Add(StreamOptionsFlag.LowLatency);
            }
            MediaService mediaService = _media.Mediaservices.Get(MediaAccount.ResourceGroupName, MediaAccount.Name);
            LiveEvent liveEvent = new LiveEvent()
            {
                Location = mediaService.Location,
                StreamOptions = streamOptions,
                Description = eventDescription,
                Encoding = eventEncoding,
                Input = eventInput,
                VanityUrl = true
            };
            return _media.LiveEvents.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, liveEvent, autoStart);
        }

        public LiveOutput CreateLiveEventOutput(string eventName, string outputName, string assetName)
        {
            CreateAsset(null, assetName);
            CreateLocator(assetName, assetName, PredefinedStreamingPolicy.ClearStreamingOnly, null);
            LiveOutput eventOutput = new LiveOutput()
            {
                AssetName = assetName
            };
            return _media.LiveOutputs.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, outputName, eventOutput);
        }

        public void UpdateLiveEvent(string eventName, string eventDescription)
        {
            LiveEvent liveEvent = new LiveEvent()
            {
                Description = eventDescription
            };
            _media.LiveEvents.Update(MediaAccount.ResourceGroupName, MediaAccount.Name, eventName, liveEvent);
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
    }
}