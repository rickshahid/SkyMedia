using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static void AddMediaStream(List<MediaStream> mediaStreams, string settingStreamName, string settingSourceUrl, string settingTextTrack)
        {
            string streamName = AppSetting.GetValue(settingStreamName);
            string sourceUrl = AppSetting.GetValue(settingSourceUrl);
            string textTracks = AppSetting.GetValue(settingTextTrack);
            MediaStream mediaStream = new MediaStream()
            {
                Name = streamName,
                Source = new StreamSource()
                {
                    Url = sourceUrl,
                    ProtectionInfo = new StreamProtection[] { }
                },
                TextTracks = Track.GetTextTracks(textTracks)
            };
            mediaStreams.Add(mediaStream);
        }

        public static MediaStream[] GetMediaStreams()
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();

            string settingKey1 = Constant.AppSettingKey.MediaStream1Name;
            string settingKey2 = Constant.AppSettingKey.MediaStream1SourceUrl;
            string settingKey3 = Constant.AppSettingKey.MediaStream1TextTracks;
            AddMediaStream(mediaStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream2Name;
            settingKey2 = Constant.AppSettingKey.MediaStream2SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream2TextTracks;
            AddMediaStream(mediaStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream3Name;
            settingKey2 = Constant.AppSettingKey.MediaStream3SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream3TextTracks;
            AddMediaStream(mediaStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream4Name;
            settingKey2 = Constant.AppSettingKey.MediaStream4SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream4TextTracks;
            AddMediaStream(mediaStreams, settingKey1, settingKey2, settingKey3);

            return mediaStreams.ToArray();
        }



        //private static IEnumerable<ILocator> GetMediaLocators(MediaClient mediaClient, string assetName)
        //{
        //    ILocator[] locators;
        //    if (string.IsNullOrEmpty(assetName))
        //    {
        //        locators = mediaClient.GetEntities(MediaEntity.StreamingLocator) as ILocator[];
        //    }
        //    else
        //    {
        //        IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset, assetName) as IAsset[];
        //        List<ILocator> assetLocators = new List<ILocator>();
        //        foreach (IAsset asset in assets)
        //        {
        //            assetLocators.AddRange(asset.Locators);
        //        }
        //        locators = assetLocators.ToArray();
        //    }
        //    locators = Array.FindAll(locators, FilterByStreaming);
        //    Array.Sort<ILocator>(locators, OrderByDate);
        //    return locators;
        //}

        //private static string[] GetThumbnailUrls(MediaClient mediaClient, IAsset asset)
        //{
        //    List<string> thumbnailUrls = new List<string>();
        //    foreach (IAssetFile assetFile in asset.AssetFiles)
        //    {
        //        string fileName = assetFile.Name.Replace(" ", "%20");
        //        if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
        //            fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
        //            fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
        //        {
        //            string thumbnailUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, asset, fileName, true);
        //            thumbnailUrls.Add(thumbnailUrl);
        //        }
        //    }
        //    thumbnailUrls.Sort();
        //    return thumbnailUrls.ToArray();
        //}

        //private static bool FilterByStreaming(ILocator locator)
        //{
        //    return locator.Type == LocatorType.OnDemandOrigin;
        //}

        //private static int OrderByDate(ILocator leftItem, ILocator rightItem)
        //{
        //    return DateTime.Compare(leftItem.Asset.Created, rightItem.Asset.Created);
        //}

        //private static int OrderByProcessor(MediaInsightSource leftItem, MediaInsightSource rightItem)
        //{
        //    return leftItem.MediaProcessor.CompareTo(rightItem.MediaProcessor);
        //}


        public static MediaStream[] GetMediaStreams(string authToken, MediaClient mediaClient, int streamNumber, out int streamOffset, out int streamIndex, out bool endOfStreams)
        {
            endOfStreams = false;
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string settingKey = Constant.AppSettingKey.MediaStreamPageSize;
            int pageSize = int.Parse(AppSetting.GetValue(settingKey));
            streamOffset = ((streamNumber - 1) / pageSize) * pageSize;
            streamIndex = (streamNumber - 1) % pageSize;
            //IEnumerable<ILocator> locators = GetMediaLocators(mediaClient, null);
            //int locatorsCount = locators.Count();
            //if (locatorsCount > 0)
            //{
            //    if (streamOffset == locatorsCount)
            //    {
            //        streamOffset = streamOffset - tunerPageSize;
            //        streamIndex = tunerPageSize - 1;
            //        endOfStreams = true;
            //    }
            //    else if (streamIndex == locatorsCount)
            //    {
            //        streamIndex = streamIndex - 1;
            //        endOfStreams = true;
            //    }
            //    locators = locators.Skip(streamOffset);
            //    foreach (ILocator locator in locators)
            //    {
            //        MediaStream[] assetStreams = GetMediaStreams(authToken, mediaClient, locator.Asset, false, false);
            //        foreach (MediaStream assetStream in assetStreams)
            //        {
            //            if (mediaStreams.Count < tunerPageSize)
            //            {
            //                mediaStreams.Add(assetStream);
            //            }
            //        }
            //    }
            //}
            return mediaStreams.ToArray();
        }

        public static MediaStream[] GetMediaStreams(string authToken, string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            //MediaClient mediaClient = new MediaClient(authToken);
            //IEnumerable<ILocator> locators = GetMediaLocators(mediaClient, searchCriteria);
            //locators = locators.Skip(skipCount);
            //if (takeCount > 0)
            //{
            //    locators = locators.Take(takeCount);
            //}
            List<MediaStream> mediaStreams = new List<MediaStream>();
            //bool filtersOnly = string.Equals(streamType, "filter", StringComparison.OrdinalIgnoreCase);
            //foreach (ILocator locator in locators)
            //{
            //    MediaStream[] streams = GetMediaStreams(authToken, mediaClient, locator.Asset, true, filtersOnly);
            //    foreach (MediaStream stream in streams)
            //    {
            //        if (stream.Source.ProtectionInfo.Length == 0)
            //        {
            //            stream.Source.ProtectionInfo = null;
            //        }
            //    }
            //    mediaStreams.AddRange(streams);
            //}
            return mediaStreams.ToArray();
        }

        public static bool IsStreamingEnabled(MediaClient mediaClient)
        {
            bool streamingEnabled = false;
            IPage<StreamingEndpoint> streamingEndpoints = mediaClient.GetEntities<StreamingEndpoint>(MediaEntity.StreamingEndpoint);
            foreach (StreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.ResourceState == StreamingEndpointResourceState.Starting ||
                    streamingEndpoint.ResourceState == StreamingEndpointResourceState.Running ||
                    streamingEndpoint.ResourceState == StreamingEndpointResourceState.Scaling)
                {
                    streamingEnabled = true;
                }
            }
            return streamingEnabled;
        }
    }
}