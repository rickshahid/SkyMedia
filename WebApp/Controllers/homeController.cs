using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class homeController : Controller
    {
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
                    track.Language = Entities.GetLanguageCode(track.Source);
                    tracks.Add(track);
                }
            }
            return tracks.ToArray();
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
                mediaStream.AnalyticsProcessors = new NameValueCollection();
                mediaStreams.Add(mediaStream);
            }
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
            ViewData["liveSourceUrl"] = Entities.GetLiveSourceUrl(livePreview);
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
                    Entities.ClearAccount(mediaClient, parameterFlag);
                    break;
            }
            string[][] entityCounts = Entities.GetEntityCounts(mediaClient);
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
            else if (Entities.StreamingEnabled(mediaClient))
            {
                mediaStreams = Entities.GetMediaStreams(mediaClient);
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
