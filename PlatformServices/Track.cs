using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Track
    {
        private static string GetSourceUrl(string streamingUrl, string fileName)
        {
            string manifestSuffix = string.Concat(Constant.Media.Stream.ManifestSuffix, Constant.Media.Stream.DefaultFormat);
            string manifestUrl = streamingUrl.Replace(manifestSuffix, string.Empty);
            string manifestFile = Path.GetFileName(manifestUrl);
            return manifestUrl.Replace(manifestFile, fileName);
        }

        public static TextTrack[] GetTextTracks(string sourceUrl, string sourceTracks)
        {
            List<TextTrack> textTracks = new List<TextTrack>();
            if (!string.IsNullOrEmpty(sourceTracks))
            {
                JArray sourceTextTracks = JArray.Parse(sourceTracks);
                foreach (JToken sourceTextTrack in sourceTextTracks)
                {
                    TextTrack textTrack = sourceTextTrack.ToObject<TextTrack>();
                    textTrack.SourceUrl = GetSourceUrl(sourceUrl, textTrack.SourceUrl);
                    textTracks.Add(textTrack);
                }
            }
            return textTracks.ToArray();
        }

        public static TextTrack[] GetTextTracks(MediaClient mediaClient, Asset asset)
        {
            List<TextTrack> textTracks = new List<TextTrack>();
            using (DatabaseClient databaseClient = new DatabaseClient(false))
            {
                string collectionId = Constant.Database.Collection.MediaAssets;
                MediaAssetLink assetLink = databaseClient.GetDocument<MediaAssetLink>(collectionId, asset.Name);
                if (assetLink != null)
                {
                    string trackType = null;
                    string trackLabel = null;
                    string trackSourceUrl = null;
                    if (assetLink.JobOutputs.ContainsKey(MediaTransformPreset.VideoIndexer))
                    {
                        trackType = Constant.Media.Track.AudioTranscript.CaptionsType;
                        trackLabel = Constant.Media.Track.AudioTranscript.CaptionsLabel;
                        string insightId = assetLink.JobOutputs[MediaTransformPreset.VideoIndexer];
                        trackSourceUrl = mediaClient.IndexerGetCaptionsUrl(insightId);
                    }
                    else if (assetLink.JobOutputs.ContainsKey(MediaTransformPreset.AudioIndexer))
                    {
                        trackType = Constant.Media.Track.AudioTranscript.CaptionsType;
                        trackLabel = Constant.Media.Track.AudioTranscript.CaptionsLabel;
                        string insightId = assetLink.JobOutputs[MediaTransformPreset.AudioIndexer];
                        trackSourceUrl = mediaClient.IndexerGetCaptionsUrl(insightId);
                    }
                    else if (assetLink.JobOutputs.ContainsKey(MediaTransformPreset.VideoAnalyzer))
                    {
                        trackType = Constant.Media.Track.AudioTranscript.SubtitlesType;
                        trackLabel = Constant.Media.Track.AudioTranscript.SubtitlesLabel;
                        string assetName = assetLink.JobOutputs[MediaTransformPreset.VideoAnalyzer];
                        string fileName = Constant.Media.Track.AudioTranscript.FileName;
                        trackSourceUrl = mediaClient.GetDownloadUrl(assetName, fileName);
                    }
                    else if (assetLink.JobOutputs.ContainsKey(MediaTransformPreset.AudioAnalyzer))
                    {
                        trackType = Constant.Media.Track.AudioTranscript.SubtitlesType;
                        trackLabel = Constant.Media.Track.AudioTranscript.SubtitlesLabel;
                        string assetName = assetLink.JobOutputs[MediaTransformPreset.AudioAnalyzer];
                        string fileName = Constant.Media.Track.AudioTranscript.FileName;
                        trackSourceUrl = mediaClient.GetDownloadUrl(assetName, fileName);
                    }
                    if (!string.IsNullOrEmpty(trackSourceUrl))
                    {
                        TextTrack textTrack = new TextTrack()
                        {
                            Type = trackType,
                            Label = trackLabel,
                            SourceUrl = trackSourceUrl
                        };
                        textTracks.Add(textTrack);
                    }
                }
            }
            return textTracks.ToArray();
        }
    }
}