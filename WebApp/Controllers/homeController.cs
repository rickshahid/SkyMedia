using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.WindowsAzure.MediaServices.Client;

using AzureSkyMedia.Services;
using AzureSkyMedia.WebApp.Models;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class homeController : Controller
    {
        private bool StreamingEnabled(MediaClient mediaClient)
        {
            bool streamingEnabled = false;
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.State == StreamingEndpointState.Running)
                {
                    streamingEnabled = true;
                }
            }
            return streamingEnabled;
        }

        private string[] MapProtectionTypes(string streamProtectionTypes)
        {
            string[] protectionTypes = new string[] { };
            if (!string.IsNullOrEmpty(streamProtectionTypes))
            {
                protectionTypes = streamProtectionTypes.Split(Constants.MultiItemSeparator);
            }
            return protectionTypes;
        }

        private string GetLanguageCode(string sourceUrl)
        {
            string[] sourceInfo = sourceUrl.Split('.');
            string fileName = sourceInfo[sourceInfo.Length - 2];
            return fileName.Substring(fileName.Length - 2);
        }

        private MediaTrack[] GetTextTracks(MediaClient mediaClient, IAsset asset, LocatorType locatorType)
        {
            List<MediaTrack> tracks = new List<MediaTrack>();
            string fileExtension = Constants.Media.AssetMetadata.VttExtension;
            string[] fileNames = MediaClient.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                MediaTrack track = new MediaTrack();
                track.Type = Constants.Media.TrackSubtitles;
                track.Source = mediaClient.GetLocatorUrl(asset, locatorType, fileName);
                track.Language = GetLanguageCode(track.Source);
                tracks.Add(track);
            }
            return tracks.ToArray();
        }

        private MediaTrack[] MapTextTracks(string textTracks)
        {
            List<MediaTrack> tracks = new List<MediaTrack>();
            if (!string.IsNullOrEmpty(textTracks))
            {
                string[] tracksInfo = textTracks.Split(Constants.MultiItemsSeparator);
                foreach (string trackInfo in tracksInfo)
                {
                    MediaTrack track = new MediaTrack();
                    string[] textTrack = trackInfo.Split(Constants.MultiItemSeparator);
                    track.Type = textTrack[0];
                    track.Source = textTrack[1];
                    track.Language = GetLanguageCode(track.Source);
                    tracks.Add(track);
                }
            }
            return tracks.ToArray();
        }

        private void AddBaseStream(List<MediaStream> mediaStreams, string settingStreamName, string settingSourceUrl,
                                   string settingTextTrack, string settingProtectionTypes)
        {
            string streamName = AppSetting.GetValue(settingStreamName);
            string sourceUrl = AppSetting.GetValue(settingSourceUrl);
            string textTracks = AppSetting.GetValue(settingTextTrack);
            string protectionTypes = AppSetting.GetValue(settingProtectionTypes);
            if (!string.IsNullOrEmpty(streamName))
            {
                MediaStream mediaStream = new MediaStream();
                mediaStream.Name = streamName;
                mediaStream.SourceUrl = sourceUrl;
                mediaStream.TextTracks = MapTextTracks(textTracks);
                mediaStream.ProtectionTypes = MapProtectionTypes(protectionTypes);
                mediaStream.AnalyticsProcessors = new SelectListItem[] { };
                mediaStreams.Add(mediaStream);
            }
        }

        private SelectListItem[] GetAnalyticsProcessors(IAsset asset)
        {
            List<SelectListItem> analyticsProcessors = new List<SelectListItem>();
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(Constants.Media.AssetMetadata.JsonExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] fileNameInfo = assetFile.Name.Split('_');
                    string processorName = fileNameInfo[fileNameInfo.Length - 1];
                    processorName = processorName.Replace(Constants.Media.AssetMetadata.JsonExtension, string.Empty);
                    processorName = processorName.Replace(Constants.NamedItemSeparator, ' ');

                    SelectListItem analyticsProcessor = new SelectListItem();
                    analyticsProcessor.Text = processorName;
                    analyticsProcessor.Value = assetFile.Name;
                    analyticsProcessors.Add(analyticsProcessor);
                }
            }
            return analyticsProcessors.ToArray();
        }

        private List<MediaStream> GetMediaStreams()
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();

            string settingKey1 = Constants.AppSettings.MediaStream1Name;
            string settingKey2 = Constants.AppSettings.MediaStream1SourceUrl;
            string settingKey3 = Constants.AppSettings.MediaStream1TextTracks;
            string settingKey4 = Constants.AppSettings.MediaStream1ProtectionTypes;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream2Name;
            settingKey2 = Constants.AppSettings.MediaStream2SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream2TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream2ProtectionTypes;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream3Name;
            settingKey2 = Constants.AppSettings.MediaStream3SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream3TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream3ProtectionTypes;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream4Name;
            settingKey2 = Constants.AppSettings.MediaStream4SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream4TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream4ProtectionTypes;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constants.AppSettings.MediaStream5Name;
            settingKey2 = Constants.AppSettings.MediaStream5SourceUrl;
            settingKey3 = Constants.AppSettings.MediaStream5TextTracks;
            settingKey4 = Constants.AppSettings.MediaStream5ProtectionTypes;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            return mediaStreams;
        }

        private List<MediaStream> GetMediaStreams(MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            foreach (ILocator locator in locators)
            {
                IAsset asset = locator.Asset;
                if (asset.IsStreamable && asset.AssetFiles.Count() > 1)
                {
                    string locatorUrl = mediaClient.GetLocatorUrl(asset, locator.Type, null);
                    if (!string.IsNullOrEmpty(locatorUrl))
                    {
                        MediaStream mediaStream = new MediaStream();
                        mediaStream.Name = asset.Name;
                        mediaStream.SourceUrl = locatorUrl;
                        mediaStream.TextTracks = GetTextTracks(mediaClient, asset, locator.Type);
                        mediaStream.ProtectionTypes = mediaClient.GetProtectionTypes(asset);
                        mediaStream.AnalyticsProcessors = GetAnalyticsProcessors(asset);
                        mediaStreams.Add(mediaStream);
                    }
                }
                if (mediaStreams.Count == 5)
                {
                    break;
                }
            }
            mediaStreams.Sort(CompareStreams);
            return mediaStreams;
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
                    if (string.Equals(leftType, MediaProtection.AES.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, MediaProtection.AES.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                    else if (string.Equals(leftType, MediaProtection.PlayReady.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, MediaProtection.PlayReady.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                    else if (string.Equals(leftType, MediaProtection.Widevine.ToString(), stringComparison))
                    {
                        comparison = -1;
                    }
                    else if (string.Equals(rightType, MediaProtection.Widevine.ToString(), stringComparison))
                    {
                        comparison = 1;
                    }
                }
            }
            return comparison;
        }

        private string GetLiveSourceUrl(bool livePreview)
        {
            string liveSourceUrl = string.Empty;
            string settingKey = Constants.AppSettings.MediaLiveAccount;
            string[] liveAccount = AppSetting.GetValue(settingKey, true);
            if (liveAccount.Length > 0)
            {
                settingKey = Constants.AppSettings.MediaLiveChannelName;
                string channelName = AppSetting.GetValue(settingKey);
                MediaClient mediaClient = new MediaClient(liveAccount[0], liveAccount[1]);
                IChannel channel = mediaClient.GetEntityByName(MediaEntity.Channel, channelName, true) as IChannel;
                if (channel != null && channel.State == ChannelState.Running)
                {
                    if (livePreview)
                    {
                        liveSourceUrl = channel.Preview.Endpoints.First().Url.ToString();
                    }
                    else
                    {
                        IProgram program = channel.Programs.First();
                        if (program.State == ProgramState.Running)
                        {
                            liveSourceUrl = mediaClient.GetLocatorUrl(program.Asset, LocatorType.OnDemandOrigin, null);
                        }
                    }
                }
            }
            return liveSourceUrl;
        }

        private IActionResult GetLiveView(string queryString)
        {
            string settingKey = Constants.AppSettings.MediaLiveStartDateTime;
            string startDateTime = AppSetting.GetValue(settingKey);
            DateTime liveStart;
            if (DateTime.TryParse(startDateTime, out liveStart))
            {
                ViewData["startDateTime"] = liveStart.ToString();
            }
            bool livePreview = this.Request.Host.Value.Contains("preview") || queryString.Contains("preview");
            ViewData["livePreview"] = livePreview;
            ViewData["liveSourceUrl"] = GetLiveSourceUrl(livePreview);
            settingKey = Constants.AppSettings.StorageCdnUrl;
            string cdnUrl = AppSetting.GetValue(settingKey);
            ViewData["liveCountdownUrl"] = string.Concat(cdnUrl, "/BuckleUp.jpg");
            return View("live");
        }

        public static string GetAuthToken(HttpRequest request, HttpResponse response)
        {
            string authToken = null;
            string cookieKey = Constants.HttpCookies.UserAuthToken;
            if (request.HasFormContentType)
            {
                authToken = request.Form[Constants.HttpForm.IdToken];
                if (!string.IsNullOrEmpty(authToken))
                {
                    response.Cookies.Append(cookieKey, authToken);
                }
            }
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = request.Cookies[cookieKey];
            }
            return authToken;
        }

        public static SelectListItem[] GetStorageAccounts(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            List<SelectListItem> storageAccounts = new List<SelectListItem>();
            IStorageAccount[] accounts = mediaClient.GetEntities(MediaEntity.StorageAccount) as IStorageAccount[];
            foreach (IStorageAccount account in accounts)
            {
                SelectListItem storageAccount = new SelectListItem();
                storageAccount.Text = string.Concat("Account: ", account.Name);
                storageAccount.Value = account.Name;
                storageAccount.Selected = account.IsDefault;
                string storageUsed = Storage.GetCapacityUsed(authToken, account.Name);
                if (storageUsed != null)
                {
                    storageAccount.Text = string.Concat(storageAccount.Text, ", Storage Used: ", storageUsed, ")");
                    storageAccounts.Add(storageAccount);
                }
            }
            return storageAccounts.ToArray();
        }

        public static SelectListItem[] GetMediaProcessors()
        {
            List<SelectListItem> mediaProcessors = new List<SelectListItem>();

            SelectListItem mediaProcessor = new SelectListItem();
            mediaProcessor.Text = string.Empty;
            mediaProcessor.Value = MediaProcessor.None.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Encoder Standard";
            mediaProcessor.Value = MediaProcessor.EncoderStandard.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Encoder Premium";
            mediaProcessor.Value = MediaProcessor.EncoderPremium.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Indexer v1";
            mediaProcessor.Value = MediaProcessor.IndexerV1.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Indexer v2";
            mediaProcessor.Value = MediaProcessor.IndexerV2.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Face Detection";
            mediaProcessor.Value = MediaProcessor.FaceDetection.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Face Redaction";
            mediaProcessor.Value = MediaProcessor.FaceRedaction.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Motion Detection";
            mediaProcessor.Value = MediaProcessor.MotionDetection.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Motion Hyperlapse";
            mediaProcessor.Value = MediaProcessor.MotionHyperlapse.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Video Annotation";
            mediaProcessor.Value = MediaProcessor.VideoSummarization.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Video Summarization";
            mediaProcessor.Value = MediaProcessor.VideoSummarization.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Character Recognition";
            mediaProcessor.Value = MediaProcessor.CharacterRecognition.ToString();
            mediaProcessors.Add(mediaProcessor);

            return mediaProcessors.ToArray();
        }

        public JsonResult command(string commandId, int parameterId, string parameterName, bool parameterFlag)
        {
            string authToken = GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            switch (commandId)
            {
                case "channelCreate":
                    mediaClient.CreateChannel(parameterName);
                    break;
                case "channelSignal":
                    mediaClient.SignalChannel(parameterName, parameterId);
                    break;
                case "accountClear":
                    accountController.ClearAccount(mediaClient, parameterFlag);
                    break;
            }
            string[][] entityCounts = accountController.GetEntityCounts(mediaClient);
            return Json(entityCounts);
        }

        public IActionResult index()
        {
            if (this.Request.HasFormContentType)
            {
                string requestError = this.Request.Form["error_description"];
                if (!string.IsNullOrEmpty(requestError) && requestError.Contains("AADB2C90118"))
                {
                    return RedirectToAction("passwordreset", "account");
                }
            }

            string queryString = this.Request.QueryString.Value.ToLower();
            if (this.Request.Host.Value.Contains("live") || queryString.Contains("live"))
            {
                return GetLiveView(queryString);
            }

            string authToken = GetAuthToken(this.Request, this.Response);

            MediaClient mediaClient = null;
            string accountMessage = string.Empty;
            List<MediaStream> mediaStreams = new List<MediaStream>();

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    mediaClient = new MediaClient(authToken);
                }
                catch
                {
                    return RedirectToAction("profileedit", "account");
                }
                SearchClient searchClient = new SearchClient(authToken);
            }

            if (mediaClient == null)
            {
                mediaStreams = GetMediaStreams();
            }
            else if (StreamingEnabled(mediaClient))
            {
                mediaStreams = GetMediaStreams(mediaClient);
            }
            else
            {
                accountMessage = "Your media account does not have an active streaming endpoint.";
            }
            ViewData["accountMessage"] = accountMessage;
            ViewData["mediaStreams"] = mediaStreams.ToArray();

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
