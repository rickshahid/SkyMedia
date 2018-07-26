using System.IO;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage.Blob;
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

        public static MediaTrack[] GetTextTracks(MediaClient mediaClient, StreamingLocator locator)
        {
            List<MediaTrack> textTracks = new List<MediaTrack>();
            Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, locator.AssetName);
            BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
            MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, asset);
            foreach (IListBlobItem file in mediaAsset.Files)
            {
                string fileName = Path.GetFileName(file.Uri.ToString());
                if (fileName == Constant.Media.Analyzer.TranscriptFile)
                {
                    string transcriptUrl = blobClient.GetDownloadUrl(asset.Container, fileName, false);
                    MediaTrack textTrack = new MediaTrack()
                    {
                        SourceUrl = transcriptUrl,
                        Type = Constant.Media.Track.Captions,
                        Label = Constant.Media.Track.CaptionsLabel
                    };
                    textTracks.Add(textTrack);
                }
            }
            return textTracks.ToArray();
        }
    }
}