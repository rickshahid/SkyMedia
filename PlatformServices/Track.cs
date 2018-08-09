using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Track
    {
        public static TextTrack[] GetTextTracks(string tracks)
        {
            List<TextTrack> textTracks = new List<TextTrack>();
            if (!string.IsNullOrEmpty(tracks))
            {
                string[] tracksInfo = tracks.Split(Constant.TextDelimiter.Connection);
                foreach (string trackInfo in tracksInfo)
                {
                    string[] track = trackInfo.Split(Constant.TextDelimiter.Application);
                    TextTrack textTrack = new TextTrack()
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

        public static TextTrack[] GetTextTracks(MediaClient mediaClient, string assetName)
        {
            List<TextTrack> textTracks = new List<TextTrack>();
            Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
            MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, asset);
            foreach (CloudBlockBlob file in mediaAsset.Files)
            {
                string fileName = file.Name;
                if (fileName == Constant.Media.Analyzer.TranscriptFile)
                {
                    string locatorName = string.Concat(assetName, Constant.Media.Asset.NameDelimiter, Constant.Media.Analyzer.Transcript);
                    StreamingLocator locator = mediaClient.CreateLocator(locatorName, assetName, PredefinedStreamingPolicy.DownloadOnly, null);
                    TextTrack textTrack = new TextTrack()
                    {
                        Type = Constant.Media.Track.CaptionsType,
                        Label = Constant.Media.Track.CaptionsLabel,
                        SourceUrl = mediaClient.GetDownloadUrl(locator, fileName)
                    };
                    textTracks.Add(textTrack);
                }
            }
            return textTracks.ToArray();
        }
    }
}