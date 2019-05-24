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

        public static MediaTrack[] GetMediaTracks(string sourceUrl, string textTracks)
        {
            List<MediaTrack> mediaTracks = new List<MediaTrack>();
            if (!string.IsNullOrEmpty(textTracks))
            {
                JArray sourceTextTracks = JArray.Parse(textTracks);
                foreach (JToken sourceTextTrack in sourceTextTracks)
                {
                    MediaTrack mediaTrack = sourceTextTrack.ToObject<MediaTrack>();
                    mediaTrack.SourceUrl = GetSourceUrl(sourceUrl, mediaTrack.SourceUrl);
                    mediaTracks.Add(mediaTrack);
                }
            }
            return mediaTracks.ToArray();
        }

        public static MediaTrack[] GetMediaTracks(MediaClient mediaClient, Asset asset)
        {
            List<MediaTrack> mediaTracks = new List<MediaTrack>();
            using (DatabaseClient databaseClient = new DatabaseClient(false))
            {
                string collectionId = Constant.Database.Collection.MediaAssets;
                MediaAssetLink assetLink = databaseClient.GetDocument<MediaAssetLink>(collectionId, asset.Name);
                if (assetLink != null)
                {
                    string trackType = null;
                    string trackLabel = null;
                    string trackSourceUrl = null;
                    if (assetLink.JobOutputs.ContainsKey(MediaTransformPreset.AudioIndexer))
                    {
                        trackType = Constant.Media.Track.AudioTranscript.CaptionsType;
                        trackLabel = Constant.Media.Track.AudioTranscript.CaptionsLabel;
                        string insightId = assetLink.JobOutputs[MediaTransformPreset.AudioIndexer];
                        trackSourceUrl = mediaClient.IndexerGetCaptionsUrl(insightId);
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
                        MediaTrack mediaTrack = new MediaTrack()
                        {
                            Type = trackType,
                            Label = trackLabel,
                            SourceUrl = trackSourceUrl
                        };
                        mediaTracks.Add(mediaTrack);
                    }
                }
            }
            return mediaTracks.ToArray();
        }
    }
}