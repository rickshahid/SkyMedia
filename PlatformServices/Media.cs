using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static MediaInsight GetMediaInsight(MediaClient mediaClient, Asset asset)
        {
            MediaInsight mediaInsight = new MediaInsight();
            if (mediaClient.IndexerEnabled())
            {
                using (DatabaseClient databaseClient = new DatabaseClient(false))
                {
                    string collectionId = Constant.Database.Collection.MediaAssets;
                    MediaAssetLink assetLink = databaseClient.GetDocument<MediaAssetLink>(collectionId, asset.Name);
                    if (assetLink != null)
                    {
                        string insightId = assetLink.JobOutputs[MediaTransformPreset.VideoIndexer];
                        if (string.IsNullOrEmpty(insightId))
                        {
                            insightId = assetLink.JobOutputs[MediaTransformPreset.AudioIndexer];
                        }
                        if (!string.IsNullOrEmpty(insightId) && mediaClient.IndexerInsightExists(insightId, out JObject insight))
                        {
                            mediaInsight.WidgetUrl = mediaClient.IndexerGetInsightUrl(insightId);
                            mediaInsight.ViewToken = insight["videos"][0]["viewToken"].ToString();
                        }
                    }
                }
            }
            return mediaInsight;
        }

        private static MediaStream GetMediaStream(string authToken, MediaClient mediaClient, Asset asset, AssetFilter assetFilter,
                                                  StreamingLocator streamingLocator, string streamingUrl)
        {
            MediaInsight mediaInsight = GetMediaInsight(mediaClient, asset);
            MediaStream mediaStream = new MediaStream()
            {
                Name = assetFilter != null ? assetFilter.Name : streamingLocator.Name,
                Url = streamingUrl,
                Poster = mediaClient.GetLocatorUrl(streamingLocator, Constant.Media.Thumbnail.FileName, false),
                Tracks = Track.GetMediaTracks(mediaClient, asset),
                Insight = mediaInsight,
                Protection = mediaClient.GetStreamProtection(authToken, mediaClient, mediaInsight, streamingLocator)
            };
            return mediaStream;
        }

        private static MediaStream[] GetMediaStreams(string authToken, MediaClient mediaClient, StreamingLocator streamingLocator)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string streamingUrl = mediaClient.GetLocatorUrl(streamingLocator, null, true);
            if (!string.IsNullOrEmpty(streamingUrl))
            {
                Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, streamingLocator.AssetName);
                MediaStream mediaStream = GetMediaStream(authToken, mediaClient, asset, null, streamingLocator, streamingUrl);
                mediaStreams.Add(mediaStream);
                AssetFilter[] assetFilters = mediaClient.GetAllEntities<AssetFilter>(MediaEntity.StreamingFilterAsset, null, streamingLocator.AssetName);
                foreach (AssetFilter assetFilter in assetFilters)
                {
                    MediaStream mediaFilter = GetMediaStream(authToken, mediaClient, asset, assetFilter, streamingLocator, streamingUrl);
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
                Url = sourceUrl,
                Tracks = Track.GetMediaTracks(sourceUrl, textTracks)
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
            IEnumerable<StreamingLocator> streamingLocators = mediaClient.GetStreamingLocators();
            foreach (StreamingLocator streamingLocator in streamingLocators)
            {
                MediaStream[] mediaStreams = GetMediaStreams(authToken, mediaClient, streamingLocator);
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
            IEnumerable<StreamingLocator> streamingLocators = mediaClient.GetStreamingLocators();
            int locatorsCount = streamingLocators.Count();
            if (locatorsCount > 0)
            {
                if (streamNumber > streamTunerPageSize)
                {
                    streamSkipCount = ((streamNumber - 1) / streamTunerPageSize) * streamTunerPageSize;
                }
                if (streamSkipCount > 0)
                {
                    streamingLocators = streamingLocators.Skip(streamSkipCount);
                }
                foreach (StreamingLocator streamingLocator in streamingLocators)
                {
                    if (accountStreams.Count < streamTunerPageSize && streamingLocator.StreamingPolicyName != PredefinedStreamingPolicy.DownloadOnly)
                    {
                        MediaStream[] mediaStreams = GetMediaStreams(authToken, mediaClient, streamingLocator);
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
    }
}