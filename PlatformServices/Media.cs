using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static string[] GetThumbnailUrls(MediaClient mediaClient, StreamingLocator locator)
        {
            List<string> thumbnailUrls = new List<string>();
            Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, locator.AssetName);
            MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, asset);
            BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            foreach (CloudBlockBlob file in mediaAsset.Files)
            {
                string fileName = file.Name;
                if (fileName.Equals(Constant.Media.Thumbnail.Best, stringComparison))
                {
                    string thumbnailUrl = blobClient.GetDownloadUrl(asset.Container, fileName, false);
                    thumbnailUrls.Insert(0, thumbnailUrl);
                }
                else if (fileName.EndsWith(MediaImageFormat.JPG.ToString(), stringComparison) ||
                         fileName.EndsWith(MediaImageFormat.PNG.ToString(), stringComparison) ||
                         fileName.EndsWith(MediaImageFormat.BMP.ToString(), stringComparison))
                {
                    string thumbnailUrl = blobClient.GetDownloadUrl(asset.Container, fileName, false);
                    thumbnailUrls.Add(thumbnailUrl);
                }
            }
            return thumbnailUrls.ToArray();
        }

        private static MediaStream GetMediaStream(string authToken, MediaClient mediaClient, StreamingLocator locator)
        {
            MediaStream mediaStream = null;
            string playerUrl = mediaClient.GetPlayerUrl(locator);
            if (!string.IsNullOrEmpty(playerUrl))
            {
                Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, locator.AssetName);
                mediaStream = new MediaStream()
                {
                    Name = locator.Name,
                    Description = asset.Description,
                    Source = new StreamSource()
                    {
                        Url = playerUrl,
                        ProtectionInfo = mediaClient.GetProtectionInfo(authToken, mediaClient, locator)
                    },
                    TextTracks = Track.GetTextTracks(mediaClient, locator.AssetName),
                    ThumbnailUrls = GetThumbnailUrls(mediaClient, locator)
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
            IEnumerable<StreamingLocator> locators = MediaClient.GetLocators(mediaClient);
            foreach (StreamingLocator locator in locators)
            {
                MediaStream accountStream = GetMediaStream(authToken, mediaClient, locator);
                if (accountStream != null)
                {
                    accountStreams.Add(accountStream);
                }
            }
            return accountStreams.ToArray();
        }

        public static MediaStream[] GetAccountStreams(string authToken, MediaClient mediaClient, int streamNumber, int streamTunerPageSize, out int streamSkipCount, out bool streamLastPage)
        {
            streamSkipCount = 0;
            streamLastPage = false;
            List<MediaStream> accountStreams = new List<MediaStream>();
            IEnumerable<StreamingLocator> locators = MediaClient.GetLocators(mediaClient);
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
                        MediaStream accountStream = GetMediaStream(authToken, mediaClient, locator);
                        if (accountStream != null)
                        {
                            accountStreams.Add(accountStream);
                        }
                    }
                }
                if (locatorsCount - streamSkipCount <= streamTunerPageSize)
                {
                    streamLastPage = true;
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
                    locators = MediaClient.GetLocators(mediaClient);
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