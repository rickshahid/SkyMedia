using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static int OrderByCreated(StreamingLocator leftItem, StreamingLocator rightItem)
        {
            return DateTime.Compare(leftItem.Created, rightItem.Created);
        }

        private static IEnumerable<StreamingLocator> GetStreamingLocators(MediaClient mediaClient)
        {
            StreamingLocator[] locators = mediaClient.GetEntities<StreamingLocator>(MediaEntity.StreamingLocator).ToArray();
            Array.Sort<StreamingLocator>(locators, OrderByCreated);
            return locators;
        }

        private static MediaStream GetMediaStream(string authToken, MediaClient mediaClient, StreamingLocator locator)
        {
            MediaStream mediaStream = null;
            string playerUrl = mediaClient.GetPlayerUrl(locator);
            if (!string.IsNullOrEmpty(playerUrl))
            {
                mediaStream = new MediaStream()
                {
                    Name = locator.AssetName,
                    Source = new StreamSource()
                    {
                        Url = playerUrl,
                        ProtectionInfo = mediaClient.GetProtectionInfo(authToken, mediaClient, locator)
                    },
                    TextTracks = Track.GetTextTracks(mediaClient, locator.AssetName)
                };
            }
            return mediaStream;
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
                TextTracks = Track.GetTextTracks(textTracks)
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

        public static MediaStream[] GetAccountStreams(string authToken, MediaClient mediaClient)
        {
            List<MediaStream> accountStreams = new List<MediaStream>();
            IEnumerable<StreamingLocator> locators = GetStreamingLocators(mediaClient);
            foreach (StreamingLocator locator in locators)
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
                            ProtectionInfo = mediaClient.GetProtectionInfo(authToken, mediaClient, locator)
                        },
                        TextTracks = Track.GetTextTracks(mediaClient, locator.AssetName)
                    };
                    accountStreams.Add(accountStream);
                }
            }
            return accountStreams.ToArray();
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
                        MediaStream accountStream = GetMediaStream(authToken, mediaClient, locator);
                        if (accountStream != null)
                        {
                            accountStreams.Add(accountStream);
                        }
                    }
                }
            }
            return accountStreams.ToArray();
        }

        public static MediaStream[] GetClipperStreams(string authToken, string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            List<MediaStream> clipperStreams = new List<MediaStream>();
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                IEnumerable<StreamingLocator> locators;
                if (!string.IsNullOrEmpty(searchCriteria))
                {
                    locators = mediaClient.GetEntities<StreamingLocator>(MediaEntity.StreamingLocator, searchCriteria);
                }
                else
                {
                    locators = GetStreamingLocators(mediaClient);
                }
                locators = locators.Skip(skipCount);
                if (takeCount > 0)
                {
                    locators = locators.Take(takeCount);
                }
                foreach (StreamingLocator locator in locators)
                {
                    MediaStream clipperStream = GetMediaStream(authToken, mediaClient, locator);
                    if (clipperStream != null)
                    {
                        clipperStreams.Add(clipperStream);
                    }
                }
            }
            return clipperStreams.ToArray();
        }
    }
}