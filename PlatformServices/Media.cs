using System.Collections.Generic;

using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
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
                        string insightId = null;
                        if (assetLink.JobOutputs.ContainsKey(MediaTransformPreset.VideoIndexer))
                        {
                            insightId = assetLink.JobOutputs[MediaTransformPreset.VideoIndexer];
                        }
                        else if (assetLink.JobOutputs.ContainsKey(MediaTransformPreset.AudioIndexer))
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

        private static MediaStream GetMediaStream(string authToken, MediaClient mediaClient, StreamingLocator streamingLocator)
        {
            MediaStream mediaStream = null;
            string streamingUrl = mediaClient.GetStreamingUrl(streamingLocator, null, true);
            if (!string.IsNullOrEmpty(streamingUrl))
            {
                Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, streamingLocator.AssetName);
                MediaInsight mediaInsight = GetMediaInsight(mediaClient, asset);
                authToken = !string.IsNullOrEmpty(mediaInsight.ViewToken) ? mediaInsight.ViewToken : string.Concat("Bearer=", authToken);
                mediaStream = new MediaStream()
                {
                    Name = string.IsNullOrEmpty(streamingLocator.Name) ? asset.Name : streamingLocator.Name,
                    Url = streamingUrl,
                    Poster = mediaClient.GetStreamingUrl(streamingLocator, Constant.Media.Thumbnail.FileName, false),
                    Tracks = Track.GetMediaTracks(mediaClient, asset),
                    Insight = mediaInsight,
                    Protection = mediaClient.GetStreamProtection(authToken, mediaClient, streamingLocator)
                };
                List<string> assetFilterNames = new List<string>();
                AssetFilter[] assetFilters = mediaClient.GetAllEntities<AssetFilter>(MediaEntity.StreamingFilterAsset, streamingLocator.AssetName);
                foreach (AssetFilter assetFilter in assetFilters)
                {
                    assetFilterNames.Add(assetFilter.Name);
                }
                mediaStream.Filters = assetFilterNames.ToArray();
            }
            return mediaStream;
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

        public static MediaStream[] GetAccountStreams(string authToken, MediaClient mediaClient, int streamNumber, out int streamSkipCount,
                                                      out int streamTunerPageSize, out bool streamTunerLastPage)
        {
            string settingKey = Constant.AppSettingKey.MediaStreamTunerPageSize;
            streamTunerPageSize = int.Parse(AppSetting.GetValue(settingKey));
            streamTunerLastPage = true;
            streamSkipCount = 0;
            if (streamNumber > streamTunerPageSize)
            {
                streamSkipCount = ((streamNumber - 1) / streamTunerPageSize) * streamTunerPageSize;
            }
            List<MediaStream> accountStreams = new List<MediaStream>();
            StreamingLocator[] streamingLocators = mediaClient.GetStreamingLocators(streamSkipCount);
            foreach (StreamingLocator streamingLocator in streamingLocators)
            {
                MediaStream mediaStream = GetMediaStream(authToken, mediaClient, streamingLocator);
                if (mediaStream != null && accountStreams.Count < streamTunerPageSize)
                {
                    accountStreams.Add(mediaStream);
                }
            }
            if (streamingLocators.Length > streamTunerPageSize)
            {
                streamTunerLastPage = false;
            }
            return accountStreams.ToArray();
        }
    }
}