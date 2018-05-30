using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Track
    {
        public static MediaTrack[] GetTextTracks(string tracks)
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

        //private static MediaTrack[] GetTextTracks(MediaClient mediaClient, VideoAnalyzer videoAnalyzer, IAsset asset)
        //{
        //    List<MediaTrack> textTracks = new List<MediaTrack>();
        //    string indexId = VideoAnalyzer.GetIndexId(asset);
        //    if (!string.IsNullOrEmpty(indexId))
        //    {
        //        string webVttUrl = videoAnalyzer.GetWebVttUrl(indexId, null);
        //        JObject index = videoAnalyzer.GetIndex(indexId, null, false);
        //        string languageLabel = VideoAnalyzer.GetLanguageLabel(index);
        //        MediaTrack textTrack = new MediaTrack()
        //        {
        //            Type = Constant.Media.Stream.TextTrack.Captions,
        //            Label = languageLabel,
        //            SourceUrl = webVttUrl,
        //        };
        //        textTracks.Add(textTrack);
        //    }
        //    string[] webVttUrls = mediaClient.GetWebVttUrls(asset);
        //    for (int i = 0; i < webVttUrls.Length; i++)
        //    {
        //        string webVttUrl = webVttUrls[i];
        //        string languageLabel = GetLanguageLabel(webVttUrl);
        //        if (!string.IsNullOrEmpty(webVttUrl))
        //        {
        //            MediaTrack textTrack = new MediaTrack()
        //            {
        //                Type = Constant.Media.Stream.TextTrack.Captions,
        //                Label = languageLabel,
        //                SourceUrl = webVttUrl,
        //            };
        //            textTracks.Add(textTrack);
        //        }
        //    }
        //    return textTracks.ToArray();
        //}
    }
}