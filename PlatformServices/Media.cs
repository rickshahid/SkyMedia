using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Media
    {
        private static int OrderLocators(ILocator leftSide, ILocator rightSide)
        {
            return DateTime.Compare(leftSide.Asset.Created, rightSide.Asset.Created);
        }

        private static string[] GetProtectionTypes(MediaClient mediaClient, IAsset asset)
        {
            List<string> protectionTypeList = new List<string>();
            MediaProtection[] protectionTypes = mediaClient.GetProtectionTypes(asset);
            foreach (MediaProtection protectionType in protectionTypes)
            {
                protectionTypeList.Add(protectionType.ToString());
            }
            return protectionTypeList.ToArray();
        }

        private static MediaTextTrack[] GetTextTracks(MediaClient mediaClient, IAsset asset)
        {
            List<MediaTextTrack> textTracks = new List<MediaTextTrack>();
            if (asset.ParentAssets.Count() > 0)
            {
                string parentAssetId = asset.ParentAssets[0].Id;
                MediaAsset[] childAssets = mediaClient.GetAssets(parentAssetId);
                foreach (MediaAsset childAsset in childAssets)
                {
                    string webVtt = childAsset.WebVtt;
                    if (!string.IsNullOrEmpty(webVtt))
                    {
                        MediaTextTrack textTrack = new MediaTextTrack();
                        textTrack.Type = Constant.Media.Stream.TextTrackCaptions;
                        textTrack.SourceUrl = mediaClient.GetLocatorUrl(childAsset.Asset, webVtt);
                        textTrack.LanguageCode = childAsset.AlternateId;
                        textTrack.Label = Language.GetLanguageLabel(textTrack.LanguageCode);
                        textTracks.Add(textTrack);
                    }
                }
            }
            return textTracks.ToArray();
        }

        private static MediaStream GetMediaStream(MediaClient mediaClient, IAsset asset)
        {
            MediaStream mediaStream = new MediaStream();
            mediaStream.Name = asset.Name;
            mediaStream.SourceUrl = mediaClient.GetLocatorUrl(asset);
            mediaStream.ProtectionTypes = GetProtectionTypes(mediaClient, asset);
            mediaStream.TextTracks = GetTextTracks(mediaClient, asset);
            mediaStream.AnalyticsMetadata = new MediaMetadata[] { };
            return mediaStream;
        }

        public static MediaStream[] GetMediaStreams(MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string settingKey = Constant.AppSettingKey.MediaLocatorMaxStreamCount;
            int maxStreamCount = int.Parse(AppSetting.GetValue(settingKey));
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            Array.Sort<ILocator>(locators, OrderLocators);
            foreach (ILocator locator in locators)
            {
                IAsset asset = locator.Asset;
                if (asset.IsStreamable && asset.AssetFiles.Count() > 1)
                {
                    MediaStream mediaStream = GetMediaStream(mediaClient, asset);
                    if (!string.IsNullOrEmpty(mediaStream.SourceUrl))
                    {
                        foreach (IStreamingAssetFilter filter in asset.AssetFilters)
                        {
                            mediaStream = new MediaStream();
                            mediaStream.Name = string.Concat(asset.Name, Constant.Media.Stream.AssetFilteredSuffix);
                            mediaStream.SourceUrl = string.Concat(mediaStream.SourceUrl, "(filter=", filter.Name, ")");
                            mediaStream.ProtectionTypes = new string[] { };
                            mediaStream.TextTracks = new MediaTextTrack[] { };
                            mediaStream.AnalyticsMetadata = new MediaMetadata[] { };
                            mediaStreams.Add(mediaStream);
                        }
                    }
                }
                if (mediaStreams.Count == maxStreamCount)
                {
                    break;
                }
            }
            return mediaStreams.ToArray();
        }

        public static MediaStream[] GetLiveStreams(MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
            foreach (IChannel channel in channels)
            {
                foreach (IProgram program in channel.Programs)
                {
                    MediaStream mediaStream = GetMediaStream(mediaClient, program.Asset);
                    mediaStreams.Add(mediaStream);
                }
            }
            return mediaStreams.ToArray();
        }

        public static bool IsStreamingEnabled(MediaClient mediaClient)
        {
            bool streamingEnabled = false;
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.State == StreamingEndpointState.Running ||
                    streamingEndpoint.State == StreamingEndpointState.Scaling)
                {
                    streamingEnabled = true;
                }
            }
            return streamingEnabled;
        }
    }
}
