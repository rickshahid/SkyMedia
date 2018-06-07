using System.Linq;
using System.Net;
using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static MediaJobTask[] GetAudioAnalyzerTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            //jobTask.MediaProcessor = MediaProcessor.AudioAnalyzer;
            JObject processorConfig = GetProcessorConfig(jobTask);
            JToken processorOptions = processorConfig["Features"][0]["Options"];
            JArray timedTextFormats = new JArray();
            //if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.AudioAnalyzerTimedTextFormatWebVtt.ToString()])
            //{
            //    timedTextFormats.Add("WebVTT");
            //}
            //if (jobTask.ProcessorConfigBoolean[MediaProcessorConfig.AudioAnalyzerTimedTextFormatTtml.ToString()])
            //{
            //    timedTextFormats.Add("TTML");
            //}
            //processorOptions["Formats"] = timedTextFormats;
            //processorOptions["Language"] = jobTask.ProcessorConfigString[MediaProcessorConfig.AudioAnalyzerLanguageId.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetVideoSummarizationTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            //jobTask.MediaProcessor = MediaProcessor.VideoSummarization;
            JObject processorConfig = GetProcessorConfig(jobTask);
            JToken processorOptions = processorConfig["Options"];
            //processorOptions["MaxMotionThumbnailDurationInSecs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.VideoSummarizationDurationSeconds.ToString()];
            //processorOptions["FadeInFadeOut"] = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.VideoSummarizationFadeTransitions.ToString()];
            //processorOptions["OutputAudio"] = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.VideoSummarizationIncludeAudio.ToString()];
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetFaceDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            //jobTask.MediaProcessor = MediaProcessor.FaceDetection;
            //string faceDetectionMode = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceDetectionMode.ToString()];
            //if (faceDetectionMode == "Redact")
            //{
            //    faceDetectionMode = "Combined";
            //    foreach (MediaJobInput jobInput in jobInputs)
            //    {
            //        IAsset asset = (IAsset)mediaClient.GetEntityById(MediaEntity.Asset, jobInput.AssetId);
            //        foreach (IAssetFile assetFile in asset.AssetFiles)
            //        {
            //            if (assetFile.Name.EndsWith(Constant.Media.FileExtension.Annotations))
            //            {
            //                faceDetectionMode = "Redact";
            //            }
            //        }
            //    }
            //}
            //JObject processorConfig = GetProcessorConfig(jobTask);
            //JToken processorOptions = processorConfig["Options"];
            //processorOptions["Mode"] = faceDetectionMode;
            //if (jobTask.ProcessorConfigString.ContainsKey(MediaProcessorConfig.FaceRedactionBlurMode.ToString()))
            //{
            //    processorOptions["BlurType"] = jobTask.ProcessorConfigString[MediaProcessorConfig.FaceRedactionBlurMode.ToString()];
            //}
            //if (jobTask.ProcessorConfigInteger.ContainsKey(MediaProcessorConfig.FaceEmotionAggregateWindow.ToString()))
            //{
            //    processorOptions["AggregateEmotionWindowMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceEmotionAggregateWindow.ToString()];
            //}
            //if (jobTask.ProcessorConfigInteger.ContainsKey(MediaProcessorConfig.FaceEmotionAggregateInterval.ToString()))
            //{
            //    processorOptions["AggregateEmotionIntervalMs"] = jobTask.ProcessorConfigInteger[MediaProcessorConfig.FaceEmotionAggregateInterval.ToString()];
            //}
            //jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }

        private static MediaJobTask[] GetMotionDetectionTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            //jobTask.MediaProcessor = MediaProcessor.MotionDetection;
            JObject processorConfig = GetProcessorConfig(jobTask);
            jobTask.ProcessorConfig = processorConfig.ToString();
            return GetJobTasks(mediaClient, jobTask, jobInputs, false);
        }
    }

    internal class VideoAnalyzer
    {
        private WebClient _indexer;
        //private string _serviceUrl;

        private VideoAnalyzer()
        {
            //string settingKey = Constant.AppSettingKey.MediaIndexerServiceUrl;
            //_serviceUrl = AppSetting.GetValue(settingKey);
        }

        public VideoAnalyzer(MediaAccount mediaAccount) : this()
        {
            //if (!string.IsNullOrEmpty(mediaAccount.IndexerKey))
            //{
            //    _indexer = new WebClient(mediaAccount.IndexerKey);
            //}
        }

        private string GetPrivacy(bool videoPublic)
        {
            return videoPublic ? "public" : "private";
        }

        //private string GetCallbackUrl()
        //{
        //    string settingKey = Constant.AppSettingKey.MediaPublishUrl;
        //    string callbackUrl = AppSetting.GetValue(settingKey);
        //    return WebUtility.UrlEncode(callbackUrl);
        //}

        //private string GetFileName(IAsset asset)
        //{
        //    string fileName = null;
        //    if (asset.AssetFiles.Count() > 1)
        //    {
        //        long fileSize = 0;
        //        //string[] assetFiles = MediaClient.GetAssetFiles(asset, Constant.Media.FileExtension.MP4);
        //        //foreach (string assetFile in asset.AssetFiles)
        //        //{
        //        //    if (string.IsNullOrEmpty(fileName) || fileSize < assetFile.ContentFileSize)
        //        //    {
        //        //        fileName = assetFile.Name;
        //        //        fileSize = assetFile.ContentFileSize;
        //        //    }
        //        //}
        //    }
        //    return fileName;
        //}

        //private void SetPublish(MediaAccount mediaAccount, string indexId)
        //{
        //    MediaPublish mediaPublish = new MediaPublish
        //    {
        //        Id = indexId,
        //        MediaAccount = mediaAccount
        //    };
        //    using - DatabaseClient databaseClient = new DatabaseClient();
        //    string collectionId = Constant.Database.Collection.OutputPublish;
        //    databaseClient.UpsertDocument(collectionId, mediaPublish);
        //}

        //private string StartAnalysis(IAsset asset, string locatorUrl, MediaInsightConfig insightConfig)
        //{
        //    string indexId = string.Empty;
        //    string requestUrl = string.Concat(_serviceUrl, "/Breakdowns");
        //    requestUrl = string.Concat(requestUrl, "?name=", WebUtility.UrlEncode(asset.Name));
        //    requestUrl = string.Concat(requestUrl, "&externalId=", WebUtility.UrlEncode(asset.Id));
        //    requestUrl = string.Concat(requestUrl, "&videoUrl=", WebUtility.UrlEncode(locatorUrl));
        //    requestUrl = string.Concat(requestUrl, "&callbackUrl=", GetCallbackUrl());
        //    requestUrl = string.Concat(requestUrl, "&privacy=", GetPrivacy(insightConfig.VideoPublic));
        //    if (!string.IsNullOrEmpty(insightConfig.VideoDescription))
        //    {
        //        requestUrl = string.Concat(requestUrl, "&description=", WebUtility.UrlEncode(insightConfig.VideoDescription));
        //    }
        //    if (!string.IsNullOrEmpty(insightConfig.VideoMetadata))
        //    {
        //        requestUrl = string.Concat(requestUrl, "&metadata=", WebUtility.UrlEncode(insightConfig.VideoMetadata));
        //    }
        //    if (!string.IsNullOrEmpty(insightConfig.LanguageId))
        //    {
        //        requestUrl = string.Concat(requestUrl, "&language=", insightConfig.LanguageId);
        //    }
        //    if (!string.IsNullOrEmpty(insightConfig.LinguisticModelId))
        //    {
        //        requestUrl = string.Concat(requestUrl, "&linguisticModelId=", insightConfig.LinguisticModelId);
        //    }
        //    if (!string.IsNullOrEmpty(insightConfig.SearchPartition))
        //    {
        //        requestUrl = string.Concat(requestUrl, "&partition=", insightConfig.SearchPartition);
        //    }
        //    if (insightConfig.AudioOnly)
        //    {
        //        requestUrl = string.Concat(requestUrl, "&indexingPreset=AudioOnly");
        //    }
        //    using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Post, requestUrl))
        //    {
        //        indexId = _indexer.GetResponse<string>(request);
        //    }
        //    return indexId;
        //}

        //public static string GetAssetId(JObject index)
        //{
        //    string assetId = string.Empty;
        //    if (index["breakdowns"] != null && index["breakdowns"].HasValues)
        //    {
        //        JToken externalId = index["breakdowns"][0]["externalId"];
        //        if (externalId != null)
        //        {
        //            assetId = externalId.ToString();
        //        }
        //    }
        //    return assetId;
        //}

        //public static string GetIndexId(IAsset asset)
        //{
        //    string indexId = string.Empty;
        //    string insightId = asset.AlternateId;
        //    if (!string.IsNullOrEmpty(insightId))
        //    {
        //        Using - DatabaseClient databaseClient = new DatabaseClient();
        //        string collectionId = Constant.Database.Collection.OutputInsight;
        //        MediaInsight mediaInsight = databaseClient.GetDocument<MediaInsight>(collectionId, insightId);
        //        if (mediaInsight != null)
        //        {
        //            foreach (MediaInsightSource insightSource in mediaInsight.Sources)
        //            {
        //                //if (insightSource.MediaProcessor == MediaProcessor.VideoAnalyzer)
        //                //{
        //                //    indexId = insightSource.OutputId;
        //                //}
        //            }
        //        }
        //    }
        //    return indexId;
        //}

        //public static string GetLanguageLabel(JObject index)
        //{
        //    string languageLabel = string.Empty;
        //    if (index["breakdowns"] != null && index["breakdowns"].HasValues)
        //    {
        //        JToken language = index["breakdowns"][0]["language"];
        //        if (language != null)
        //        {
        //            string languageId = language.ToString();
        //            languageLabel = Media.GetLanguageLabel(languageId, true);
        //        }
        //    }
        //    return languageLabel;
        //}

        //public JArray GetAccounts()
        //{
        //    JArray accounts = null;
        //    if (_indexer != null)
        //    {
        //        string requestUrl = string.Concat(_serviceUrl, "/Accounts");
        //        using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
        //        {
        //            accounts = _indexer.GetResponse<JArray>(request);
        //        }
        //    }
        //    return accounts;
        //}

        //public string GetInsightUrl(string indexId, bool allowEdit)
        //{
        //    string url = string.Empty;
        //    if (_indexer != null)
        //    {
        //        string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId, "/InsightsWidgetUrl");
        //        if (allowEdit)
        //        {
        //            requestUrl = string.Concat(requestUrl, "?allowEdit=true");
        //        }
        //        using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
        //        {
        //            url = _indexer.GetResponse<string>(request);
        //        }
        //    }
        //    return url;
        //}

        //public string GetWebVttUrl(string indexId, string languageId)
        //{
        //    string url = string.Empty;
        //    if (_indexer != null)
        //    {
        //        string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId, "/VttUrl");
        //        if (!string.IsNullOrEmpty(languageId))
        //        {
        //            requestUrl = string.Concat(requestUrl, "?", languageId);
        //        }
        //        using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
        //        {
        //            url = _indexer.GetResponse<string>(request);
        //        }
        //    }
        //    return url;
        //}

        //public JObject GetIndex(string indexId, string languageId, bool processingState)
        //{
        //    JObject index = null;
        //    if (_indexer != null)
        //    {
        //        string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId);
        //        if (!string.IsNullOrEmpty(languageId))
        //        {
        //            requestUrl = string.Concat(requestUrl, "?", languageId);
        //        }
        //        else if (processingState)
        //        {
        //            requestUrl = string.Concat(requestUrl, "/State");
        //        }
        //        using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
        //        {
        //            index = _indexer.GetResponse<JObject>(request);
        //        }
        //    }
        //    return index;
        //}

        //public void RestartAnalysis(MediaClient mediaClient, string indexId)
        //{
        //    if (_indexer != null)
        //    {
        //        string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/reindex/", indexId);
        //        requestUrl = string.Concat(requestUrl, "?callbackUrl=", GetCallbackUrl());
        //        using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Put, requestUrl))
        //        {
        //            _indexer.GetResponse<object>(request);
        //        }
        //        SetPublish(mediaClient.MediaAccount, indexId);
        //    }
        //}

        //public string StartAnalysis(MediaClient mediaClient, IAsset asset, MediaInsightConfig insightConfig)
        //{
        //    string indexId = string.Empty;
        //    if (_indexer != null)
        //    {
        //        string fileName = GetFileName(asset);
        //        string locatorUrl = mediaClient.GetLocatorUrl(LocatorType.Sas, asset, fileName, false);
        //        indexId = StartAnalysis(asset, locatorUrl, insightConfig);
        //        SetPublish(mediaClient.MediaAccount, indexId);
        //    }
        //    return indexId;
        //}

        //public void DeleteVideo(string indexId, bool deleteInsight)
        //{
        //    if (_indexer != null)
        //    {
        //        string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/", indexId);
        //        if (deleteInsight)
        //        {
        //            requestUrl = string.Concat(requestUrl, "?deleteInsights=true");
        //        }
        //        using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Delete, requestUrl))
        //        {
        //            _indexer.GetResponse<object>(request);
        //        }
        //    }
        //}

        //public JObject Search(MediaSearchCriteria searchCriteria)
        //{
        //    JObject results = null;
        //    if (_indexer != null)
        //    {
        //        if (!string.IsNullOrEmpty(searchCriteria.IndexId))
        //        {
        //            results = GetIndex(searchCriteria.IndexId, searchCriteria.LanguageId, false);
        //        }
        //        else
        //        {
        //            string requestUrl = string.Concat(_serviceUrl, "/Breakdowns/Search?privacy=", GetPrivacy(searchCriteria.VideoPublic));
        //            if (!string.IsNullOrEmpty(searchCriteria.IndexId))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&id=", searchCriteria.IndexId);
        //            }
        //            if (!string.IsNullOrEmpty(searchCriteria.AssetId))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&externalId=", WebUtility.UrlEncode(searchCriteria.AssetId));
        //            }
        //            if (!string.IsNullOrEmpty(searchCriteria.LanguageId))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&language=", searchCriteria.LanguageId);
        //            }
        //            if (!string.IsNullOrEmpty(searchCriteria.TextScope))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&textScope=", WebUtility.UrlEncode(searchCriteria.TextScope));
        //            }
        //            if (!string.IsNullOrEmpty(searchCriteria.TextQuery))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&query=", WebUtility.UrlEncode(searchCriteria.TextQuery));
        //            }
        //            if (!string.IsNullOrEmpty(searchCriteria.SearchPartition))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&partition=", searchCriteria.SearchPartition);
        //            }
        //            if (!string.IsNullOrEmpty(searchCriteria.Owner))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&owner=", searchCriteria.Owner);
        //            }
        //            if (!string.IsNullOrEmpty(searchCriteria.Face))
        //            {
        //                requestUrl = string.Concat(requestUrl, "&face=", searchCriteria.Face);
        //            }
        //            if (searchCriteria.PageSize > 0)
        //            {
        //                requestUrl = string.Concat(requestUrl, "&pageSize=", searchCriteria.PageSize.ToString());
        //            }
        //            if (searchCriteria.SkipCount > 0)
        //            {
        //                requestUrl = string.Concat(requestUrl, "&skip=", searchCriteria.SkipCount.ToString());
        //            }
        //            using (HttpRequestMessage request = _indexer.GetRequest(HttpMethod.Get, requestUrl))
        //            {
        //                results = _indexer.GetResponse<JObject>(request);
        //            }
        //        }
        //    }
        //    return results;
        //}
    }
}