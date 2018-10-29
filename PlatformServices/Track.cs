using System;
using System.Collections.Generic;

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

        public static TextTrack[] GetTextTracks(MediaClient mediaClient, Asset asset)
        {
            string captionsUrl = null;
            List<TextTrack> textTracks = new List<TextTrack>();
            MediaAsset mediaAsset = new MediaAsset(mediaClient, asset);
            foreach (MediaFile assetFile in mediaAsset.Files)
            {
                if (string.IsNullOrEmpty(captionsUrl))
                {
                    if (string.Equals(assetFile.Name, Constant.Media.Track.TranscriptFile, StringComparison.OrdinalIgnoreCase))
                    {
                        captionsUrl = mediaClient.GetDownloadUrl(asset, assetFile.Name);
                    }
                    else if (!string.IsNullOrEmpty(asset.AlternateId))
                    {
                        captionsUrl = mediaClient.IndexerGetCaptionsUrl(asset.AlternateId);
                    }
                }
            }
            if (!string.IsNullOrEmpty(captionsUrl))
            {
                TextTrack textTrack = new TextTrack()
                {
                    Type = Constant.Media.Track.CaptionsType,
                    Label = Constant.Media.Track.CaptionsLabel,
                    SourceUrl = captionsUrl
                };
                textTracks.Add(textTrack);
            }
            return textTracks.ToArray();
        }
    }
}