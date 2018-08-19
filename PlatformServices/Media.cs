using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static void UploadIngestManifest(BlobClient blobClient, MediaIngestManifest ingestManifest)
        {
            string ingestManifestJson = JsonConvert.SerializeObject(ingestManifest);
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string assetName = Path.GetFileNameWithoutExtension(ingestManifest.JobInputFileUrl);
            string fileName = string.Concat(Constant.Media.IngestManifest.TriggerPrefix, Constant.TextDelimiter.File, assetName, Constant.Media.IngestManifest.FileExtension);
            CloudBlockBlob manifest = blobClient.GetBlockBlob(containerName, fileName);
            manifest.Properties.ContentType = Constant.Media.ContentType.IngestManifest;
            manifest.UploadTextAsync(ingestManifestJson).Wait();
        }

        private static string[] GetThumbnailUrls(MediaClient mediaClient, StreamingLocator locator)
        {
            List<string> thumbnailUrls = new List<string>();
            Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, locator.AssetName);
            MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, asset);
            BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            foreach (MediaFile assetFile in mediaAsset.Files)
            {
                if (assetFile.Name.Equals(Constant.Media.Thumbnail.Best, stringComparison))
                {
                    string thumbnailUrl = blobClient.GetDownloadUrl(asset.Container, assetFile.Name, false);
                    thumbnailUrls.Insert(0, thumbnailUrl);
                }
                else if (assetFile.Name.EndsWith(MediaImageFormat.JPG.ToString(), stringComparison) ||
                         assetFile.Name.EndsWith(MediaImageFormat.PNG.ToString(), stringComparison) ||
                         assetFile.Name.EndsWith(MediaImageFormat.BMP.ToString(), stringComparison))
                {
                    string thumbnailUrl = blobClient.GetDownloadUrl(asset.Container, assetFile.Name, false);
                    thumbnailUrls.Add(thumbnailUrl);
                }
            }
            return thumbnailUrls.ToArray();
        }

        private static MediaInsight GetMediaInsight(MediaClient mediaClient, Asset asset)
        {
            string indexerUrl = null;
            if (mediaClient.IndexerIsEnabled() && !string.IsNullOrEmpty(asset.AlternateId))
            {
                indexerUrl = mediaClient.IndexerGetInsightUrl(asset.AlternateId);
            }
            MediaInsight mediaInsight = new MediaInsight()
            {
                IndexerUrl = indexerUrl
            };
            return mediaInsight;
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
                    TextTracks = Track.GetTextTracks(mediaClient, asset),
                    ThumbnailUrls = GetThumbnailUrls(mediaClient, locator),
                    ContentInsight = GetMediaInsight(mediaClient, asset)
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

        public static void UploadIngestManifests(MediaIngestManifest ingestManifest, string rssUrl)
        {
            XmlDocument rssDocument;
            BlobClient blobClient = new BlobClient();
            using (WebClient webClient = new WebClient(null))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, rssUrl);
                rssDocument = webClient.GetResponse<XmlDocument>(webRequest);
            }
            XmlElement channel = rssDocument.DocumentElement["channel"];
            XmlNodeList videos = channel.SelectNodes("item");
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(rssDocument.NameTable);
            namespaceManager.AddNamespace(Constant.Media.Channel9.NamespacePrefix, Constant.Media.Channel9.NamespaceUrl);
            for (int i = 0; i < Constant.Media.Channel9.IngestVideoCount; i++)
            {
                XmlNode video = videos[i];
                XmlNode videoContent = video.SelectSingleNode(Constant.Media.Channel9.XPathQuery, namespaceManager);
                string videoUrl = videoContent.Attributes["url"].Value;
                string videoDescription = video.SelectSingleNode("title").InnerText;
                videoUrl = videoUrl.Replace(Constant.Media.Channel9.UrlHttp, Constant.Media.Channel9.UrlHttps);
                videoUrl = videoUrl.Replace(Constant.Media.Channel9.Http, Constant.Media.Channel9.Https);
                ingestManifest.JobInputFileUrl = videoUrl;
                ingestManifest.JobOutputAssetDescription = videoDescription;
                UploadIngestManifest(blobClient, ingestManifest);
            }
        }
    }
}