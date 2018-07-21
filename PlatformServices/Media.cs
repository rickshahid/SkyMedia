using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static int OrderByDate(StreamingLocator leftItem, StreamingLocator rightItem)
        {
            return DateTime.Compare(leftItem.Created, rightItem.Created);
        }

        private static IEnumerable<StreamingLocator> GetStreamingLocators(MediaClient mediaClient)
        {
            StreamingLocator[] locators = mediaClient.GetEntities<StreamingLocator>(MediaEntity.StreamingLocator).ToArray();
            Array.Sort<StreamingLocator>(locators, OrderByDate);
            return locators;
        }

        private static void AddSampleStream(List<MediaStream> sampleStreams, string settingStreamName, string settingSourceUrl, string settingTextTrack)
        {
            string streamName = AppSetting.GetValue(settingStreamName);
            string sourceUrl = AppSetting.GetValue(settingSourceUrl);
            string textTracks = AppSetting.GetValue(settingTextTrack);
            MediaStream sampleStream = new MediaStream()
            {
                Name = streamName,
                Source = new StreamSource()
                {
                    Url = sourceUrl,
                    ProtectionInfo = new StreamProtection[] { }
                },
                TextTracks = Track.GetMediaTracks(textTracks)
            };
            sampleStreams.Add(sampleStream);
        }

        public static MediaStream[] GetSampleStreams()
        {
            List<MediaStream> sampleStreams = new List<MediaStream>();

            string settingKey1 = Constant.AppSettingKey.MediaStream1Name;
            string settingKey2 = Constant.AppSettingKey.MediaStream1SourceUrl;
            string settingKey3 = Constant.AppSettingKey.MediaStream1TextTracks;
            AddSampleStream(sampleStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream2Name;
            settingKey2 = Constant.AppSettingKey.MediaStream2SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream2TextTracks;
            AddSampleStream(sampleStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream3Name;
            settingKey2 = Constant.AppSettingKey.MediaStream3SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream3TextTracks;
            AddSampleStream(sampleStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream4Name;
            settingKey2 = Constant.AppSettingKey.MediaStream4SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream4TextTracks;
            AddSampleStream(sampleStreams, settingKey1, settingKey2, settingKey3);

            return sampleStreams.ToArray();
        }
 
        public static MediaStream[] GetAccountStreams(string authToken, MediaClient mediaClient, int streamNumber, out int streamOffset, out int streamIndex, out bool endOfStreams)
        {
            endOfStreams = false;
            List<MediaStream> accountStreams = new List<MediaStream>();
            int pageSize = Constant.Media.Stream.TunerPageSize;
            streamOffset = ((streamNumber - 1) / pageSize) * pageSize;
            streamIndex = (streamNumber - 1) % pageSize;
            IEnumerable<StreamingLocator> locators = GetStreamingLocators(mediaClient);
            int locatorsCount = locators.Count();
            if (locatorsCount > 0)
            {
                if (streamOffset == locatorsCount)
                {
                    streamOffset = streamOffset - pageSize;
                    streamIndex = pageSize - 1;
                    endOfStreams = true;
                }
                else if (streamIndex == locatorsCount)
                {
                    streamIndex = streamIndex - 1;
                    endOfStreams = true;
                }
                locators = locators.Skip(streamOffset);
                foreach (StreamingLocator locator in locators)
                {
                    if (accountStreams.Count < pageSize)
                    {
                        string playerUrl = mediaClient.GetPlayerUrl(locator);
                        if (!string.IsNullOrEmpty(playerUrl))
                        {
                            MediaStream accountStream = new MediaStream()
                            {
                                Name = locator.AssetName,
                                Source = new StreamSource()
                                {
                                    Url = playerUrl,
                                    ProtectionInfo = new StreamProtection[] { }
                                },
                                //TextTracks = Track.GetMediaTracks(textTracks)
                            };
                            accountStreams.Add(accountStream);
                        }
                    }
                }
            }
            return accountStreams.ToArray();
        }

        public static MediaStream[] GetClipperStreams(string authToken, string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            using (MediaClient mediaClient = new MediaClient(authToken))
            {

            }
            //IEnumerable<ILocator> locators = GetMediaLocators(mediaClient, searchCriteria);
            //locators = locators.Skip(skipCount);
            //if (takeCount > 0)
            //{
            //    locators = locators.Take(takeCount);
            //}
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