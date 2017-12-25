using System;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Processor
    {
        private static string[] GetProcessorIds(bool presetsView)
        {
            List<string> processorIds = new List<string>();
            if (presetsView)
            {
                processorIds.Add(Constant.Media.ProcessorId.EncoderStandard);
                processorIds.Add(Constant.Media.ProcessorId.VideoAnnotation);
                processorIds.Add(Constant.Media.ProcessorId.CharacterRecognition);
                processorIds.Add(Constant.Media.ProcessorId.ContentModeration);
                processorIds.Add(Constant.Media.ProcessorId.SpeechAnalyzer);
                processorIds.Add(Constant.Media.ProcessorId.FaceDetection);
                processorIds.Add(Constant.Media.ProcessorId.FaceRedaction);
                processorIds.Add(Constant.Media.ProcessorId.MotionDetection);
                processorIds.Add(Constant.Media.ProcessorId.MotionHyperlapse);
            }
            else
            {
                processorIds.Add(Constant.Media.ProcessorId.EncoderStandard);
                processorIds.Add(Constant.Media.ProcessorId.EncoderPremium);
                processorIds.Add(Constant.Media.ProcessorId.VideoIndexer);
                processorIds.Add(Constant.Media.ProcessorId.VideoAnnotation);
                processorIds.Add(Constant.Media.ProcessorId.VideoSummarization);
                processorIds.Add(Constant.Media.ProcessorId.CharacterRecognition);
                processorIds.Add(Constant.Media.ProcessorId.ContentModeration);
                processorIds.Add(Constant.Media.ProcessorId.SpeechAnalyzer);
                processorIds.Add(Constant.Media.ProcessorId.FaceDetection);
                processorIds.Add(Constant.Media.ProcessorId.FaceRedaction);
                processorIds.Add(Constant.Media.ProcessorId.MotionDetection);
                processorIds.Add(Constant.Media.ProcessorId.MotionHyperlapse);
            }
            return processorIds.ToArray();
        }

        private static MediaProcessor[] GetMediaProcessors(string authToken, bool presetsView)
        {
            MediaProcessor[] mediaProcessors = null;
            try
            {
                CacheClient cacheClient = new CacheClient(authToken);
                string itemKey = Constant.Cache.ItemKey.MediaProcessors;
                mediaProcessors = cacheClient.GetValues<MediaProcessor>(itemKey);
                if (mediaProcessors.Length == 0)
                {
                    MediaClient mediaClient = new MediaClient(authToken);
                    IMediaProcessor[] processors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
                    mediaProcessors = GetMediaProcessors(processors, presetsView);
                    cacheClient.SetValue<MediaProcessor[]>(itemKey, mediaProcessors);
                }
            }
            finally
            {
                if (mediaProcessors == null)
                {
                    MediaClient mediaClient = new MediaClient(authToken);
                    IMediaProcessor[] processors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
                    mediaProcessors = GetMediaProcessors(processors, presetsView);
                }
            }
            return mediaProcessors;
        }

        private static MediaProcessor[] GetMediaProcessors(IMediaProcessor[] processors, bool presetsView)
        {
            List<MediaProcessor> mediaProcessors = new List<MediaProcessor>();
            string[] processorIds = GetProcessorIds(presetsView);
            foreach (string processorId in processorIds)
            {
                if (!processorId.StartsWith(Constant.Media.ProcessorId.Prefix, StringComparison.OrdinalIgnoreCase))
                {
                    MediaProcessor mediaProcessor = (MediaProcessor)Enum.Parse(typeof(MediaProcessor), processorId);
                    mediaProcessors.Add(mediaProcessor);
                }
                else
                {
                    foreach (IMediaProcessor processor in processors)
                    {
                        if (string.Equals(processorId, processor.Id, StringComparison.OrdinalIgnoreCase))
                        {
                            MediaProcessor? mediaProcessor = GetMediaProcessor(processor.Id);
                            if (mediaProcessor.HasValue)
                            {
                                mediaProcessors.Add(mediaProcessor.Value);
                            }
                        }
                    }
                }
            }
            return mediaProcessors.ToArray();
        }

        public static MediaProcessor? GetMediaProcessor(string processorId)
        {
            MediaProcessor? mediaProcessor = null;
            switch (processorId)
            {
                case Constant.Media.ProcessorId.EncoderStandard:
                    mediaProcessor = MediaProcessor.EncoderStandard;
                    break;
                case Constant.Media.ProcessorId.EncoderPremium:
                    mediaProcessor = MediaProcessor.EncoderPremium;
                    break;
                case Constant.Media.ProcessorId.VideoIndexer:
                    mediaProcessor = MediaProcessor.VideoIndexer;
                    break;
                case Constant.Media.ProcessorId.VideoAnnotation:
                    mediaProcessor = MediaProcessor.VideoAnnotation;
                    break;
                case Constant.Media.ProcessorId.VideoSummarization:
                    mediaProcessor = MediaProcessor.VideoSummarization;
                    break;
                case Constant.Media.ProcessorId.CharacterRecognition:
                    mediaProcessor = MediaProcessor.CharacterRecognition;
                    break;
                case Constant.Media.ProcessorId.ContentModeration:
                    mediaProcessor = MediaProcessor.ContentModeration;
                    break;
                case Constant.Media.ProcessorId.SpeechAnalyzer:
                    mediaProcessor = MediaProcessor.SpeechAnalyzer;
                    break;
                case Constant.Media.ProcessorId.FaceDetection:
                    mediaProcessor = MediaProcessor.FaceDetection;
                    break;
                case Constant.Media.ProcessorId.FaceRedaction:
                    mediaProcessor = MediaProcessor.FaceRedaction;
                    break;
                case Constant.Media.ProcessorId.MotionDetection:
                    mediaProcessor = MediaProcessor.MotionDetection;
                    break;
                case Constant.Media.ProcessorId.MotionHyperlapse:
                    mediaProcessor = MediaProcessor.MotionHyperlapse;
                    break;
            }
            return mediaProcessor;
        }

        public static object GetMediaProcessors(string authToken, bool presetsView, bool getEntities)
        {
            object mediaProcessors;
            if (getEntities)
            {
                MediaClient mediaClient = new MediaClient(authToken);
                mediaProcessors = mediaClient.GetEntities(MediaEntity.Processor);
            }
            else
            {
                NameValueCollection processorNames = new NameValueCollection();
                MediaProcessor[] processors = GetMediaProcessors(authToken, presetsView);
                foreach (MediaProcessor processor in processors)
                {
                    string processorName = GetProcessorName(processor);
                    processorNames.Add(processorName, processor.ToString());
                }
                mediaProcessors = processorNames;
            }
            return mediaProcessors;
        }

        public static string GetProcessorId(MediaProcessor mediaProcessor)
        {
            string processorId = null;
            switch (mediaProcessor)
            {
                case MediaProcessor.EncoderStandard:
                    processorId = Constant.Media.ProcessorId.EncoderStandard;
                    break;
                case MediaProcessor.EncoderPremium:
                    processorId = Constant.Media.ProcessorId.EncoderPremium;
                    break;
                case MediaProcessor.VideoIndexer:
                    processorId = Constant.Media.ProcessorId.VideoIndexer;
                    break;
                case MediaProcessor.VideoAnnotation:
                    processorId = Constant.Media.ProcessorId.VideoAnnotation;
                    break;
                case MediaProcessor.VideoSummarization:
                    processorId = Constant.Media.ProcessorId.VideoSummarization;
                    break;
                case MediaProcessor.CharacterRecognition:
                    processorId = Constant.Media.ProcessorId.CharacterRecognition;
                    break;
                case MediaProcessor.ContentModeration:
                    processorId = Constant.Media.ProcessorId.ContentModeration;
                    break;
                case MediaProcessor.SpeechAnalyzer:
                    processorId = Constant.Media.ProcessorId.SpeechAnalyzer;
                    break;
                case MediaProcessor.FaceDetection:
                    processorId = Constant.Media.ProcessorId.FaceDetection;
                    break;
                case MediaProcessor.FaceRedaction:
                    processorId = Constant.Media.ProcessorId.FaceRedaction;
                    break;
                case MediaProcessor.MotionDetection:
                    processorId = Constant.Media.ProcessorId.MotionDetection;
                    break;
                case MediaProcessor.MotionHyperlapse:
                    processorId = Constant.Media.ProcessorId.MotionHyperlapse;
                    break;
            }
            return processorId;
        }

        public static string GetProcessorName(MediaProcessor mediaProcessor)
        {
            string processorName = mediaProcessor.ToString();
            return Regex.Replace(processorName, Constant.TextFormatter.SpacePattern, Constant.TextFormatter.SpaceReplacement);
        }

        public static NameValueCollection GetProcessorPresets(MediaProcessor mediaProcessor, string accountId)
        {
            NameValueCollection processorPresets = new NameValueCollection();
            DocumentClient documentClient = new DocumentClient();
            string collectionId = Constant.Database.Collection.ProcessorConfig;
            string processorName = GetProcessorName(mediaProcessor);
            JObject[] presets = documentClient.GetDocuments(collectionId);
            foreach (JObject preset in presets)
            {
                string presetProcessor = preset["MediaProcessor"].ToString();
                if (string.Equals(presetProcessor, processorName, StringComparison.OrdinalIgnoreCase) && (preset["accountId"] == null ||
                    string.Equals(accountId, preset["accountId"].ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    string presetName = preset["PresetName"].ToString();
                    string presetId = preset["id"].ToString();
                    processorPresets.Add(presetName, presetId);
                }
            }
            return processorPresets;
        }
    }
}