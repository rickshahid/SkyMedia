using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class StreamFile
    {
        private static MediaTrack[] GetTextTracks(MediaClient mediaClient, IAsset asset, LocatorType locatorType)
        {
            List<MediaTrack> textTracks = new List<MediaTrack>();
            string fileExtension = Constants.Media.FileExtension.WebVtt;
            string[] fileNames = MediaClient.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                MediaTrack textTrack = new MediaTrack();
                textTrack.Type = Constants.Media.Stream.TextTrackSubtitles;
                textTrack.Source = mediaClient.GetLocatorUrl(asset, locatorType, fileName);
                textTrack.Language = GetLanguageCode(textTrack.Source);
                textTracks.Add(textTrack);
            }
            return textTracks.ToArray();
        }

        private static string GetAnalyticsProcessorName(string fileName)
        {
            string[] fileNameInfo = fileName.Split('_');
            string processorName = fileNameInfo[fileNameInfo.Length - 1];
            processorName = processorName.Replace(Constants.Media.FileExtension.Json, string.Empty);
            return processorName.Replace(Constants.NamedItemSeparator, ' ');
        }

        private static MediaMetadata[] GetAnalyticsProcessors(IAsset asset)
        {
            List<MediaMetadata> analyticsProcessors = new List<MediaMetadata>();
            string fileExtension = Constants.Media.FileExtension.Json;
            string[] fileNames = MediaClient.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                string processorName = GetAnalyticsProcessorName(fileName);
                MediaMetadata mediaMetadata = new MediaMetadata();
                mediaMetadata.ProcessorName = processorName;
                mediaMetadata.MetadataFile = fileName;
                analyticsProcessors.Add(mediaMetadata);
            }
            return analyticsProcessors.ToArray();
        }

        private static int OrderLocators(ILocator leftSide, ILocator rightSide)
        {
            return DateTime.Compare(leftSide.Asset.Created, rightSide.Asset.Created);
        }

        public static string GetLanguageCode(string sourceUrl)
        {
            string[] sourceInfo = sourceUrl.Split('.');
            string fileName = sourceInfo[sourceInfo.Length - 2];
            return fileName.Substring(fileName.Length - 2);
        }

        public static MediaStream[] GetMediaStreams(MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string settingKey = Constants.AppSettingKey.MediaLocatorMaxStreamCount;
            int maxStreamCount = int.Parse(AppSetting.GetValue(settingKey));
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            Array.Sort<ILocator>(locators, OrderLocators);
            foreach (ILocator locator in locators)
            {
                IAsset asset = locator.Asset;
                if (asset.IsStreamable && asset.AssetFiles.Count() > 1)
                {
                    string locatorUrl = mediaClient.GetLocatorUrl(asset, locator.Type, null);
                    if (!string.IsNullOrEmpty(locatorUrl))
                    {
                        MediaTrack[] textTracks = GetTextTracks(mediaClient, asset, locator.Type);
                        string[] protectionTypes = mediaClient.GetProtectionTypes(asset);
                        MediaMetadata[] analyticsProcessors = GetAnalyticsProcessors(asset);

                        MediaStream mediaStream = new MediaStream();
                        mediaStream.Name = asset.Name;
                        mediaStream.SourceUrl = locatorUrl;
                        mediaStream.TextTracks = textTracks;
                        mediaStream.ProtectionTypes = protectionTypes;
                        mediaStream.AnalyticsProcessors = analyticsProcessors;
                        mediaStreams.Add(mediaStream);

                        foreach (IStreamingAssetFilter filter in asset.AssetFilters)
                        {
                            mediaStream = new MediaStream();
                            mediaStream.Name = string.Concat(asset.Name, " (Filtered)");
                            mediaStream.SourceUrl = string.Concat(locatorUrl, "(filter=", filter.Name, ")");
                            mediaStream.TextTracks = new MediaTrack[] { };
                            mediaStream.ProtectionTypes = new string[] { };
                            mediaStream.AnalyticsProcessors = new MediaMetadata[] { };
                            mediaStreams.Add(mediaStream);
                        }
                    }

                }
                if (mediaStreams.Count == maxStreamCount)
                {
                    break;
                }
            }
            return mediaStreams.ToArray();
        }
    }
}
