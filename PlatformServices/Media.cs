using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static bool FilterByStreaming(ILocator locator)
        {
            return locator.Type == LocatorType.OnDemandOrigin;
        }

        private static int OrderByDate(ILocator leftItem, ILocator rightItem)
        {
            return DateTime.Compare(leftItem.Asset.Created, rightItem.Asset.Created);
        }

        private static int OrderByProcessor(MediaInsight leftItem, MediaInsight rightItem)
        {
            MediaProcessor leftProcessor = Processor.GetMediaProcessor(leftItem.ProcessorId).Value;
            MediaProcessor rightProcessor = Processor.GetMediaProcessor(rightItem.ProcessorId).Value;
            return leftProcessor.CompareTo(rightProcessor);
        }

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
                    Src = sourceUrl,
                    ProtectionInfo = new StreamProtection[] { }
                },
                TextTracks = GetTextTracks(textTracks)
            };
            mediaStreams.Add(mediaStream);
        }

        private static IEnumerable<ILocator> GetMediaLocators(MediaClient mediaClient, string assetName)
        {
            ILocator[] locators;
            if (string.IsNullOrEmpty(assetName))
            {
                locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            }
            else
            {
                IAsset[] assets = mediaClient.GetEntities(MediaEntity.Asset, assetName) as IAsset[];
                List<ILocator> assetLocators = new List<ILocator>();
                foreach (IAsset asset in assets)
                {
                    assetLocators.AddRange(asset.Locators);
                }
                locators = assetLocators.ToArray();
            }
            locators = Array.FindAll(locators, FilterByStreaming);
            Array.Sort<ILocator>(locators, OrderByDate);
            return locators;
        }

        private static string[] GetThumbnailUrls(MediaClient mediaClient, IAsset asset)
        {
            List<string> thumbnailUrls = new List<string>();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                string fileName = assetFile.Name.Replace(" ", "%20");
                if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    string thumbnailUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, asset, fileName, true);
                    thumbnailUrls.Add(thumbnailUrl);
                }
            }
            thumbnailUrls.Sort();
            return thumbnailUrls.ToArray();
        }

        private static string GetTextTrackType(string webVttData)
        {
            string webVttType;
            if (webVttData.Contains(Constant.Media.Stream.TextTrack.ThumbnailsData))
            {
                webVttType = Constant.Media.Stream.TextTrack.Thumbnails;
            }
            else
            {
                webVttType = Constant.Media.Stream.TextTrack.Captions;
            }
            return webVttType;
        }

        private static MediaTrack[] GetTextTracks(string tracks)
        {
            List<MediaTrack> textTracks = new List<MediaTrack>();
            if (!string.IsNullOrEmpty(tracks))
            {
                string[] tracksInfo = tracks.Split(Constant.TextDelimiter.Connection);
                foreach (string trackInfo in tracksInfo)
                {
                    string[] track = trackInfo.Split(Constant.TextDelimiter.Application);
                    MediaTrack textTrack = new MediaTrack()
                    {
                        Type = track[0],
                        Label = track[1],
                        SourceUrl = track[2]
                    };
                    textTracks.Add(textTrack);
                }
            }
            return textTracks.ToArray();
        }

        private static MediaTrack[] GetTextTracks(MediaClient mediaClient, IndexerClient indexerClient, IAsset asset)
        {
            List<MediaTrack> textTracks = new List<MediaTrack>();
            string indexId = asset.AlternateId;
            if (!string.IsNullOrEmpty(indexId))
            {
                string webVttUrl = indexerClient.GetWebVttUrl(indexId, null);
                string webVttData = WebClient.GetData(webVttUrl);
                JObject index = indexerClient.GetIndex(indexId, null, false);
                string languageLabel = IndexerClient.GetLanguageLabel(index);
                MediaTrack textTrack = new MediaTrack()
                {
                    Type = GetTextTrackType(webVttData),
                    Label = languageLabel,
                    SourceUrl = webVttUrl,
                };
                textTracks.Add(textTrack);
            }
            string[] webVttUrls = mediaClient.GetWebVttUrls(asset);
            for (int i = 0; i < webVttUrls.Length; i++)
            {
                string webVttUrl = webVttUrls[i];
                string webVttData = WebClient.GetData(webVttUrl);
                string languageLabel = Language.GetLanguageLabel(webVttUrl);
                if (!string.IsNullOrEmpty(webVttUrl))
                {
                    MediaTrack textTrack = new MediaTrack()
                    {
                        Type = GetTextTrackType(webVttData),
                        Label = languageLabel,
                        SourceUrl = webVttUrl,
                    };
                    textTracks.Add(textTrack);
                }
            }
            return textTracks.ToArray();
        }

        private static MediaStream[] GetMediaStreams(string authToken, MediaClient mediaClient, IAsset asset, bool clipperView, bool filtersOnly)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();

            IndexerClient indexerClient = new IndexerClient(mediaClient.MediaAccount);

            List<MediaInsight> contentInsight = new List<MediaInsight>();
            if (!clipperView)
            {
                string indexId = asset.AlternateId;
                if (!string.IsNullOrEmpty(indexId))
                {
                    MediaInsight insight = new MediaInsight()
                    {
                        ProcessorId = Processor.GetProcessorId(MediaProcessor.VideoIndexer, null),
                        ProcessorName = Processor.GetProcessorName(MediaProcessor.VideoIndexer),
                        DocumentId = indexId,
                        SourceUrl = indexerClient.GetInsightUrl(indexId, true)
                    };
                    contentInsight.Add(insight);
                }

                string[] fileNames = MediaClient.GetFileNames(asset, Constant.Media.FileExtension.Json);
                foreach (string fileName in fileNames)
                {
                    string[] fileNameInfo = fileName.Split(Constant.TextDelimiter.Identifier);
                    if (Enum.TryParse(fileNameInfo[0], out MediaProcessor processor) && fileNameInfo.Length == 2)
                    {
                        string documentId = fileNameInfo[1].Replace(Constant.Media.FileExtension.Json, string.Empty);
                        MediaInsight insight = new MediaInsight()
                        {
                            ProcessorId = Processor.GetProcessorId(processor, null),
                            ProcessorName = Processor.GetProcessorName(processor),
                            DocumentId = documentId,
                            SourceUrl = string.Empty
                        };
                        contentInsight.Add(insight);
                    }
                }

                contentInsight.Sort(OrderByProcessor);
            }

            MediaStream mediaStream = new MediaStream()
            {
                Id = asset.Id,
                Name = asset.Name,
                Type = "asset",
                Source = new StreamSource()
                {
                    Src = mediaClient.GetLocatorUrl(asset),
                    ProtectionInfo = mediaClient.GetStreamProtections(authToken, asset)
                },
                Thumbnails = GetThumbnailUrls(mediaClient, asset),
                TextTracks = GetTextTracks(mediaClient, indexerClient, asset),
                ContentInsight = contentInsight.ToArray(),
            };
            if (!filtersOnly)
            {
                mediaStreams.Add(mediaStream);
            }

            foreach (IStreamingAssetFilter assetFilter in asset.AssetFilters)
            {
                MediaStream filteredStream = mediaStream.DeepCopy();
                filteredStream.Id = assetFilter.Id;
                filteredStream.Name = string.Concat(mediaStream.Name, "<br><br>+ ", assetFilter.Name);
                filteredStream.Type = "filter";
                filteredStream.Source.Src = string.Concat(filteredStream.Source.Src, "(filter=", assetFilter.Name, ")");
                mediaStreams.Add(filteredStream);
            }

            return mediaStreams.ToArray();
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

        public static MediaStream[] GetMediaStreams(string authToken, MediaClient mediaClient, int streamNumber, out int streamOffset, out int streamIndex, out bool endOfStreams)
        {
            endOfStreams = false;
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string settingKey = Constant.AppSettingKey.MediaLocatorTunerPageSize;
            int tunerPageSize = int.Parse(AppSetting.GetValue(settingKey));
            streamOffset = ((streamNumber - 1) / tunerPageSize) * tunerPageSize;
            streamIndex = (streamNumber - 1) % tunerPageSize;
            IEnumerable<ILocator> locators = GetMediaLocators(mediaClient, null);
            int locatorsCount = locators.Count();
            if (locatorsCount > 0)
            {
                if (streamOffset == locatorsCount)
                {
                    streamOffset = streamOffset - tunerPageSize;
                    streamIndex = tunerPageSize - 1;
                    endOfStreams = true;
                }
                else if (streamIndex == locatorsCount)
                {
                    streamIndex = streamIndex - 1;
                    endOfStreams = true;
                }
                locators = locators.Skip(streamOffset);
                foreach (ILocator locator in locators)
                {
                    MediaStream[] assetStreams = GetMediaStreams(authToken, mediaClient, locator.Asset, false, false);
                    foreach (MediaStream assetStream in assetStreams)
                    {
                        if (mediaStreams.Count < tunerPageSize)
                        {
                            mediaStreams.Add(assetStream);
                        }
                    }
                }
            }
            return mediaStreams.ToArray();
        }

        public static MediaStream[] GetMediaStreams(string authToken, string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            IEnumerable<ILocator> locators = GetMediaLocators(mediaClient, searchCriteria);
            locators = locators.Skip(skipCount);
            if (takeCount > 0)
            {
                locators = locators.Take(takeCount);
            }
            List<MediaStream> mediaStreams = new List<MediaStream>();
            bool filtersOnly = string.Equals(streamType, "filter", StringComparison.OrdinalIgnoreCase);
            foreach (ILocator locator in locators)
            {
                MediaStream[] streams = GetMediaStreams(authToken, mediaClient, locator.Asset, true, filtersOnly);
                foreach (MediaStream stream in streams)
                {
                    if (stream.Source.ProtectionInfo.Length == 0)
                    {
                        stream.Source.ProtectionInfo = null;
                    }
                }
                mediaStreams.AddRange(streams);
            }
            return mediaStreams.ToArray();
        }

        public static bool IsStreamingEnabled(MediaClient mediaClient)
        {
            bool streamingEnabled = false;
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.State == StreamingEndpointState.Starting ||
                    streamingEndpoint.State == StreamingEndpointState.Running ||
                    streamingEndpoint.State == StreamingEndpointState.Scaling)
                {
                    streamingEnabled = true;
                }
            }
            return streamingEnabled;
        }
    }
}