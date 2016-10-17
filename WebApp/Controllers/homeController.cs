using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.ServiceBroker;
using SkyMedia.WebApp.Models;

namespace SkyMedia.WebApp.Controllers
{
    public class homeController : Controller
    {
        private string[] MapProtectionTypes(string protectionTypes)
        {
            string[] types = new string[] { };
            if (!string.IsNullOrEmpty(protectionTypes))
            {
                types = protectionTypes.Split(Constants.MultiItemSeparator);
            }
            return types;
        }

        private string GetLanguageCode(string fileName)
        {
            string[] fileNameInfo = fileName.Split('.');
            return fileNameInfo[fileNameInfo.Length - 2];
        }
        
        private MediaTrack[] GetTextTracks(MediaClient mediaClient, IAsset asset, LocatorType locatorType)
        {
            List<MediaTrack> tracks = new List<MediaTrack>();
            string fileExtension = Constants.Media.AssetMetadata.WebVttFileExtension;
            string[] fileNames = assetController.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                MediaTrack track = new MediaTrack();
                track.Type = "subtitles";
                track.Source = mediaClient.GetLocatorUrl(asset, locatorType, fileName);
                track.Language = GetLanguageCode(fileName);
                track.Label = Selections.GetLanguageLabel(track.Language);
                tracks.Add(track);
            }
            return tracks.ToArray();
        }

        private MediaTrack[] MapTextTracks(string textTracks)
        {
            List<MediaTrack> tracks = new List<MediaTrack>();
            if (!string.IsNullOrEmpty(textTracks))
            {
                string[] textTracksInfo = textTracks.Split(Constants.MultiItemsSeparator);
                foreach (string textTrackInfo in textTracksInfo)
                {
                    MediaTrack track = new MediaTrack();
                    string[] textTrack = textTrackInfo.Split(Constants.MultiItemSeparator);
                    track.Type = textTrack[0];
                    track.Label = textTrack[1];
                    track.Language = textTrack[2];
                    track.Source = textTrack[3];
                    tracks.Add(track);
                }
            }
            return tracks.ToArray();
        }

        private void AddBaseStream(List<MediaStream> streams, string settingStreamName, string settingSourceUrl,
                                   string settingTextTrack, string settingProtectionTypes)
        {
            string streamName = AppSetting.GetValue(settingStreamName);
            string sourceUrl = AppSetting.GetValue(settingSourceUrl);
            string textTracks = AppSetting.GetValue(settingTextTrack);
            string protectionTypes = AppSetting.GetValue(settingProtectionTypes);
            if (!string.IsNullOrEmpty(streamName))
            {
                MediaStream stream = new MediaStream();
                stream.Name = streamName;
                stream.SourceUrl = sourceUrl;
                stream.TextTracks = MapTextTracks(textTracks);
                stream.ProtectionTypes = MapProtectionTypes(protectionTypes);
                streams.Add(stream);
            }
        }

        private List<MediaStream> GetBaseStreams()
        {
            List<MediaStream> streams = new List<MediaStream>();

            string settingKey1 = Constants.AppSettings.MediaStream1Name;
            string settingKey2 = Constants.AppSettings.MediaStream1SourceUrl;
            string settingKey3 = Constants.AppSettings.MediaStream1TextTracks;
            string settingKey4 = Constants.AppSettings.MediaStream1ProtectionTypes;
            AddBaseStream(streams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream2Name;
            settingKey2 = Constants.AppSettings.MediaStream2SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream2TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream2ProtectionTypes;
            AddBaseStream(streams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream3Name;
            settingKey2 = Constants.AppSettings.MediaStream3SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream3TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream3ProtectionTypes;
            AddBaseStream(streams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream4Name;
            settingKey2 = Constants.AppSettings.MediaStream4SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream4TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream4ProtectionTypes;
            AddBaseStream(streams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream5Name;
            settingKey2 = Constants.AppSettings.MediaStream5SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream5TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream5ProtectionTypes;
            AddBaseStream(streams, settingKey1, settingKey2, settingKey3, settingKey4);

            return streams;
        }

        private int CompareStreams(MediaStream leftSide, MediaStream rightSide)
        {
            int comparison = string.Compare(leftSide.Name, rightSide.Name);
            if (comparison == 0)
            {
                if (leftSide.ProtectionTypes.Length == 0)
                {
                    comparison = -1;
                }
                else if (rightSide.ProtectionTypes.Length == 0)
                {
                    comparison = 1;
                }
                else
                {
                    string leftType = leftSide.ProtectionTypes[0];
                    string rightType = rightSide.ProtectionTypes[0];
                    StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;
                    if (string.Equals(leftType, ProtectionType.AES.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, ProtectionType.AES.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                    else if (string.Equals(leftType, ProtectionType.PlayReady.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, ProtectionType.PlayReady.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                    else if (string.Equals(leftType, ProtectionType.Widevine.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, ProtectionType.Widevine.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                    else if (string.Equals(leftType, ProtectionType.FairPlay.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, ProtectionType.FairPlay.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                }
            }
            return comparison;
        }

        public JsonResult dispatch(string command, int parameterId, string parameterName, bool parameterFlag)
        {
            string authToken = AuthToken.GetValue(this.Request, this.Response);
            switch (command)
            {
                case "channelCreate":
                    accountController.CreateChannel(authToken, parameterName);
                    break;
                case "channelSignal":
                    accountController.StartAdvertisement(authToken, parameterName, parameterId);
                    break;
                case "accountClear":
                    accountController.DeleteEntities(authToken, parameterFlag);
                    break;
            }
            string[][] entityCounts = accountController.GetEntityCounts(authToken);
            return Json(entityCounts);
        }

        private string GetLiveSourceUrl()
        {
            string liveSourceUrl = null;
            string settingKey = Constants.AppSettings.MediaLiveAccount;
            string[] liveAccount = AppSetting.GetValue(settingKey, true);
            if (liveAccount.Length > 0)
            {
                settingKey = Constants.AppSettings.MediaLiveChannelName;
                string channelName = AppSetting.GetValue(settingKey);
                MediaClient mediaClient = new MediaClient(liveAccount);
                IChannel channel = mediaClient.GetEntityByName(EntityType.Channel, channelName, true) as IChannel;
                if (channel != null)
                {
                    IAsset asset = channel.Programs.First().Asset;
                    liveSourceUrl = mediaClient.GetLocatorUrl(asset, LocatorType.OnDemandOrigin, null);
                }
            }
            return liveSourceUrl;
        }

        public IActionResult index()
        {
            string settingKey = Constants.ConnectionStrings.AzureMedia;
            string[] mediaAccount = AppSetting.GetValue(settingKey, true);

            string queryString = this.Request.QueryString.Value.ToLower();
            if (this.Request.Host.Value.Contains("live") || queryString.Contains("live"))
            {
                settingKey = Constants.AppSettings.MediaLiveStartDateTime;
                string startDateTime = AppSetting.GetValue(settingKey);
                DateTime liveStart;
                if (DateTime.TryParse(startDateTime, out liveStart))
                {
                    ViewData["startDateTime"] = liveStart.ToString();
                }
                ViewData["liveSourceUrl"] = GetLiveSourceUrl();
                settingKey = Constants.AppSettings.StorageCdnUrl;
                string cdnUrl = AppSetting.GetValue(settingKey);
                ViewData["buckleUpUrl"] = string.Concat(cdnUrl, "/BuckleUp.jpg");
                ViewData["analyticsProcessors"] = Selections.GetMediaProcessors(true);
                return View("live");
            }
            string authToken = AuthToken.GetValue(this.Request, this.Response);

            MediaClient mediaClient = null;
            List<MediaStream> streams;

            if (mediaAccount.Length > 0)
            {
                mediaClient = new MediaClient(mediaAccount);
            }
            else if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    mediaClient = new MediaClient(authToken);
                }
                catch
                {
                    return RedirectToAction("profileedit", "account");
                }
            }

            if (mediaClient == null)
            {
                streams = GetBaseStreams();
            }
            else
            {
                streams = new List<MediaStream>();
                IAsset[] assets = mediaClient.GetEntities(EntityType.Asset) as IAsset[];
                foreach (IAsset asset in assets)
                {
                    if (!mediaClient.LiveAsset(asset))
                    {
                        LocatorType locatorType = LocatorType.OnDemandOrigin;
                        string locatorUrl = mediaClient.GetLocatorUrl(asset, locatorType, null);
                        if (!string.IsNullOrEmpty(locatorUrl))
                        {
                            MediaStream stream = new MediaStream();
                            stream.Name = asset.Name;
                            stream.SourceUrl = locatorUrl;
                            stream.TextTracks = GetTextTracks(mediaClient, asset, locatorType);
                            stream.ProtectionTypes = mediaClient.GetProtectionTypes(asset);
                            streams.Add(stream);
                        }
                    }
                }
                streams.Sort(CompareStreams);
            }

            ViewData["mediaStreams"] = streams.ToArray();
            ViewData["analyticsProcessors"] = Selections.GetMediaProcessors(true);

            ViewData["streamNumber"] = 1;
            ViewData["autoPlay"] = "false";
            if (queryString.Contains("stream"))
            {
                ViewData["streamNumber"] = this.Request.Query["stream"];
                ViewData["autoPlay"] = "true";
            }

            ViewData["languageCode"] = this.Request.Query["language"];
            return View();
        }
    }
}
