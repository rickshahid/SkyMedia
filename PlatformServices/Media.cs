using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static string[] GetThumbnailUrls(MediaClient mediaClient, StreamingLocator locator)
        {
            List<string> thumbnailUrls = new List<string>();
            Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, locator.AssetName);
            MediaAsset mediaAsset = new MediaAsset(mediaClient, asset);
            StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            foreach (MediaFile assetFile in mediaAsset.Files)
            {
                if (assetFile.Name.StartsWith(Constant.Media.Thumbnail.FileNamePrefix, stringComparison))
                {
                    string thumbnailUrl = blobClient.GetDownloadUrl(asset.Container, assetFile.Name);
                    thumbnailUrls.Add(thumbnailUrl);
                }
            }
            return thumbnailUrls.ToArray();
        }

        private static MediaInsight GetMediaInsight(MediaClient mediaClient, Asset asset)
        {
            MediaInsight mediaInsight = new MediaInsight();
            if (mediaClient.IndexerEnabled())
            {
                string insightId = asset.AlternateId;
                if (!string.IsNullOrEmpty(insightId))
                {
                    JObject insight = mediaClient.IndexerGetInsight(insightId);
                    mediaInsight.WidgetUrl = mediaClient.IndexerGetInsightUrl(insightId);
                    mediaInsight.ViewToken = insight["videos"][0]["viewToken"].ToString();
                }
            }
            return mediaInsight;
        }

        private static MediaStream GetMediaStream(string authToken, MediaClient mediaClient, Asset asset, AssetFilter assetFilter,
                                                  StreamingLocator streamingLocator, string playerUrl)
        {
            string streamId = assetFilter != null ? assetFilter.Id : asset.AssetId.ToString();
            string streamName = assetFilter != null ? assetFilter.Name : streamingLocator.Name;
            MediaClipType streamType = assetFilter != null ? MediaClipType.Filter : MediaClipType.Asset;
            if (streamType == MediaClipType.Filter)
            {
                playerUrl = string.Concat(playerUrl, "(filter=", assetFilter.Name, ")");
            }
            MediaInsight mediaInsight = GetMediaInsight(mediaClient, asset);
            MediaStream mediaStream = new MediaStream()
            {
                Id = streamId,
                Name = streamName,
                Type = streamType,
                Description = asset.Description,
                Source = new StreamSource()
                {
                    Url = playerUrl,
                    ProtectionInfo = mediaClient.GetProtectionInfo(authToken, mediaClient, mediaInsight, streamingLocator)
                },
                TextTracks = Track.GetTextTracks(mediaClient, asset),
                ThumbnailUrls = GetThumbnailUrls(mediaClient, streamingLocator),
                ContentInsight = mediaInsight
            };
            return mediaStream;
        }

        private static MediaStream[] GetMediaStreams(string authToken, MediaClient mediaClient, StreamingLocator streamingLocator)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string playerUrl = mediaClient.GetPlayerUrl(streamingLocator);
            if (!string.IsNullOrEmpty(playerUrl))
            {
                Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, streamingLocator.AssetName);
                MediaStream mediaStream = GetMediaStream(authToken, mediaClient, asset, null, streamingLocator, playerUrl);
                mediaStreams.Add(mediaStream);
                AssetFilter[] assetFilters = mediaClient.GetAllEntities<AssetFilter>(MediaEntity.StreamingFilterAsset, null, streamingLocator.AssetName);
                foreach (AssetFilter assetFilter in assetFilters)
                {
                    MediaStream mediaFilter = GetMediaStream(authToken, mediaClient, asset, assetFilter, streamingLocator, playerUrl);
                    mediaStreams.Add(mediaFilter);
                }
            }
            return mediaStreams.ToArray();
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
                    Url = sourceUrl
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

            settingKey1 = Constant.AppSettingKey.MediaStream5Name;
            settingKey2 = Constant.AppSettingKey.MediaStream5SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream5TextTracks;
            AddSampleStream(sampleStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream6Name;
            settingKey2 = Constant.AppSettingKey.MediaStream6SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream6TextTracks;
            AddSampleStream(sampleStreams, settingKey1, settingKey2, settingKey3);

            return sampleStreams.ToArray();
        }

        public static MediaStream[] GetAccountStreams(string authToken, MediaClient mediaClient)
        {
            List<MediaStream> accountStreams = new List<MediaStream>();
            IEnumerable<StreamingLocator> locators = mediaClient.GetLocators();
            foreach (StreamingLocator locator in locators)
            {
                MediaStream[] mediaStreams = GetMediaStreams(authToken, mediaClient, locator);
                if (mediaStreams.Length > 0)
                {
                    accountStreams.AddRange(mediaStreams);
                }
            }
            return accountStreams.ToArray();
        }

        public static MediaStream[] GetAccountStreams(string authToken, MediaClient mediaClient, int streamNumber, int streamTunerPageSize, out int streamSkipCount, out bool streamTunerLastPage)
        {
            streamSkipCount = 0;
            streamTunerLastPage = true;
            List<MediaStream> accountStreams = new List<MediaStream>();
            IEnumerable<StreamingLocator> locators = mediaClient.GetLocators();
            int locatorsCount = locators.Count();
            if (locatorsCount > 0)
            {
                if (streamNumber > streamTunerPageSize)
                {
                    streamSkipCount = ((streamNumber - 1) / streamTunerPageSize) * streamTunerPageSize;
                }
                if (streamSkipCount > 0)
                {
                    locators = locators.Skip(streamSkipCount);
                }
                foreach (StreamingLocator locator in locators)
                {
                    if (accountStreams.Count < streamTunerPageSize)
                    {
                        MediaStream[] mediaStreams = GetMediaStreams(authToken, mediaClient, locator);
                        if (mediaStreams != null)
                        {
                            accountStreams.AddRange(mediaStreams);
                        }
                    }
                }
                if (locatorsCount >= streamSkipCount + streamTunerPageSize)
                {
                    streamTunerLastPage = false;
                }
            }
            return accountStreams.ToArray();
        }

        public static MediaStream[] GetClipperStreams(string authToken, string assetName, int skipCount, int takeCount, string streamType)
        {
            List<MediaStream> clipperStreams = new List<MediaStream>();
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                IEnumerable<StreamingLocator> locators;
                if (!string.IsNullOrEmpty(assetName))
                {
                    locators = mediaClient.GetLocators(assetName);
                }
                else
                {
                    locators = mediaClient.GetLocators();
                }
                locators = locators.Skip(skipCount);
                if (takeCount > 0)
                {
                    locators = locators.Take(takeCount);
                }
                foreach (StreamingLocator locator in locators)
                {
                    MediaStream[] mediaStreams = GetMediaStreams(authToken, mediaClient, locator);
                    if (mediaStreams != null)
                    {
                        clipperStreams.AddRange(mediaStreams);
                    }
                }
            }
            return clipperStreams.ToArray();
        }
    }
}