using System;
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

        private static int OrderByDate(ILocator leftSide, ILocator rightSide)
        {
            return DateTime.Compare(leftSide.Asset.Created, rightSide.Asset.Created);
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
            string documentId = DocumentClient.GetDocumentId(asset, out bool videoIndexer);
            if (!string.IsNullOrEmpty(documentId) && videoIndexer && indexerClient.IndexerEnabled)
            {
                string webVttUrl = indexerClient.GetWebVttUrl(documentId, null);
                JObject index = indexerClient.GetIndex(documentId, null, false);
                string languageLabel = IndexerClient.GetLanguageLabel(index);
                MediaTrack textTrack = new MediaTrack()
                {
                    Type = Constant.Media.Stream.TextTrackCaptions,
                    Label = languageLabel,
                    SourceUrl = webVttUrl,
                };
                textTracks.Add(textTrack);
            }
            else
            {
                string[] webVttUrls = mediaClient.GetWebVttUrls(asset);
                for (int i = 0; i < webVttUrls.Length; i++)
                {
                    string webVttUrl = webVttUrls[i];
                    string languageLabel = Language.GetLanguageLabel(webVttUrl);
                    if (!string.IsNullOrEmpty(webVttUrl))
                    {
                        MediaTrack textTrack = new MediaTrack()
                        {
                            Type = Constant.Media.Stream.TextTrackCaptions,
                            Label = languageLabel,
                            SourceUrl = webVttUrl,
                        };
                        textTracks.Add(textTrack);
                    }
                }
            }
            return textTracks.ToArray();
        }

        private static void AddBaseStream(List<MediaStream> mediaStreams, string settingStreamName, string settingSourceUrl, string settingTextTrack)
        {
            string streamName = AppSetting.GetValue(settingStreamName);
            string sourceUrl = AppSetting.GetValue(settingSourceUrl);
            string textTracks = AppSetting.GetValue(settingTextTrack);
            if (!string.IsNullOrEmpty(streamName))
            {
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
        }

        private static MediaStream GetMediaStream(string authToken, MediaClient mediaClient, IAsset asset)
        {
            List<MediaInsight> contentInsight = new List<MediaInsight>();

            IndexerClient indexerClient = new IndexerClient(authToken);
            string documentId = DocumentClient.GetDocumentId(asset, out bool videoIndexer);
            if (!string.IsNullOrEmpty(documentId) && videoIndexer && indexerClient.IndexerEnabled)
            {
                MediaInsight insight = new MediaInsight()
                {
                    MediaProcessor = Processor.GetProcessorName(MediaProcessor.VideoIndexer),
                    DocumentId = documentId,
                    SourceUrl = indexerClient.GetInsightUrl(documentId, false)
                };
                contentInsight.Add(insight);
            }

            string[] fileNames = MediaClient.GetFileNames(asset, Constant.Media.FileExtension.Json);
            foreach (string fileName in fileNames)
            {
                string[] fileNameInfo = fileName.Split(Constant.TextDelimiter.Identifier);
                if (fileNameInfo.Length == 2)
                {
                    MediaProcessor processor = (MediaProcessor)Enum.Parse(typeof(MediaProcessor), fileNameInfo[0]);
                    documentId = fileNameInfo[1].Replace(Constant.Media.FileExtension.Json, string.Empty);
                    MediaInsight insight = new MediaInsight()
                    {
                        MediaProcessor = Processor.GetProcessorName(processor),
                        DocumentId = documentId,
                        SourceUrl = string.Empty
                    };
                    contentInsight.Add(insight);
                }
            }

            MediaStream mediaStream = new MediaStream()
            {
                Name = asset.Name,
                Source = new StreamSource()
                {
                    Src = mediaClient.GetLocatorUrl(asset),
                    ProtectionInfo = mediaClient.GetStreamProtections(authToken, asset)
                },
                TextTracks = GetTextTracks(mediaClient, indexerClient, asset),
                ContentInsight = contentInsight.ToArray(),
            };
            return mediaStream;
        }

        private static MediaStream[] GetLiveStreams(string authToken, MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
            foreach (IChannel channel in channels)
            {
                foreach (IProgram program in channel.Programs)
                {
                    MediaStream mediaStream = GetMediaStream(authToken, mediaClient, program.Asset);
                    mediaStreams.Add(mediaStream);
                }
            }
            return mediaStreams.ToArray();
        }

        public static MediaStream[] GetMediaStreams(string authToken, MediaClient mediaClient, bool liveStreams)
        {
            IndexerClient indexerClient = new IndexerClient(authToken);

            if (liveStreams)
            {
                return GetLiveStreams(authToken, mediaClient);
            }

            List<MediaStream> mediaStreams = new List<MediaStream>();
            string settingKey = Constant.AppSettingKey.MediaLocatorMaxStreamCount;
            int maxStreamCount = int.Parse(AppSetting.GetValue(settingKey));
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            locators = Array.FindAll(locators, FilterByStreaming);
            Array.Sort<ILocator>(locators, OrderByDate);
            foreach (ILocator locator in locators)
            {
                IAsset asset = locator.Asset;
                if (asset.IsStreamable)
                {
                    MediaStream mediaStream = GetMediaStream(authToken, mediaClient, asset);
                    mediaStreams.Add(mediaStream);
                    if (!string.IsNullOrEmpty(mediaStream.Source.Src))
                    {
                        foreach (IStreamingAssetFilter filter in asset.AssetFilters)
                        {
                            MediaStream streamFilter = mediaStream;
                            streamFilter.Name = string.Concat(streamFilter.Name, Constant.Media.Stream.AssetFilteredSuffix);
                            streamFilter.Source.Src = string.Concat(streamFilter.Source.Src, "(filter=", filter.Name, ")");
                            mediaStreams.Add(streamFilter);
                        }
                    }
                    if (mediaStreams.Count == maxStreamCount)
                    {
                        break;
                    }
                }
            }
            return mediaStreams.ToArray();
        }

        public static MediaStream[] GetMediaStreams()
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();

            string settingKey1 = Constant.AppSettingKey.MediaStream1Name;
            string settingKey2 = Constant.AppSettingKey.MediaStream1SourceUrl;
            string settingKey3 = Constant.AppSettingKey.MediaStream1TextTracks;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream2Name;
            settingKey2 = Constant.AppSettingKey.MediaStream2SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream2TextTracks;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream3Name;
            settingKey2 = Constant.AppSettingKey.MediaStream3SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream3TextTracks;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3);

            settingKey1 = Constant.AppSettingKey.MediaStream4Name;
            settingKey2 = Constant.AppSettingKey.MediaStream4SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream4TextTracks;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3);

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

        public static bool IsStreamingStarting(MediaClient mediaClient)
        {
            bool streamingStarting = false;
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.State == StreamingEndpointState.Starting)
                {
                    streamingStarting = true;
                }
            }
            return streamingStarting;
        }

        public static string StartStreamingEndpoint(MediaClient mediaClient)
        {
            string endpointName = string.Empty;
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            if (streamingEndpoints.Length > 0)
            {
                IStreamingEndpoint streamingEndpoint = streamingEndpoints[0];
                streamingEndpoint.StartAsync();
                endpointName = streamingEndpoint.Name;
            }
            return endpointName;
        }
    }
}