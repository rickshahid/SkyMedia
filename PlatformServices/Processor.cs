using System;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Processor
    {
        private static string[] GetProcessorIds()
        {
            List<string> processorIds = new List<string>();
            processorIds.Add(Constant.Media.ProcessorId.EncoderStandard);
            processorIds.Add(Constant.Media.ProcessorId.EncoderPremium);
            processorIds.Add(Constant.Media.ProcessorId.EncoderUltra);
            processorIds.Add(Constant.Media.ProcessorId.VideoIndexer);
            processorIds.Add(Constant.Media.ProcessorId.VideoAnnotation);
            processorIds.Add(Constant.Media.ProcessorId.VideoSummarization);
            processorIds.Add(Constant.Media.ProcessorId.SpeechToText);
            processorIds.Add(Constant.Media.ProcessorId.FaceDetection);
            processorIds.Add(Constant.Media.ProcessorId.FaceRedaction);
            processorIds.Add(Constant.Media.ProcessorId.MotionDetection);
            processorIds.Add(Constant.Media.ProcessorId.MotionHyperlapse);
            processorIds.Add(Constant.Media.ProcessorId.MotionStabilization);
            processorIds.Add(Constant.Media.ProcessorId.CharacterRecognition);
            processorIds.Add(Constant.Media.ProcessorId.ContentModeration);
            return processorIds.ToArray();
        }

        private static MediaProcessor[] GetMediaProcessors(string authToken)
        {
            CacheClient cacheClient = new CacheClient(authToken);
            string itemKey = Constant.Cache.ItemKey.MediaProcessors;
            MediaProcessor[] mediaProcessors = cacheClient.GetValues<MediaProcessor>(itemKey);
            if (mediaProcessors.Length == 0)
            {
                MediaClient mediaClient = new MediaClient(authToken);
                IMediaProcessor[] processors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
                mediaProcessors = GetMediaProcessors(processors);
                cacheClient.SetValue<MediaProcessor[]>(itemKey, mediaProcessors);
            }
            return mediaProcessors;
        }

        private static MediaProcessor[] GetMediaProcessors(IMediaProcessor[] processors)
        {
            List<MediaProcessor> mediaProcessors = new List<MediaProcessor>();
            string[] processorIds = GetProcessorIds();
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

        internal static string GetProcessorId(MediaProcessor mediaProcessor)
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
                case MediaProcessor.EncoderUltra:
                    processorId = Constant.Media.ProcessorId.EncoderUltra;
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
                case MediaProcessor.SpeechToText:
                    processorId = Constant.Media.ProcessorId.SpeechToText;
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
                case MediaProcessor.MotionStabilization:
                    processorId = Constant.Media.ProcessorId.MotionStabilization;
                    break;
                case MediaProcessor.CharacterRecognition:
                    processorId = Constant.Media.ProcessorId.CharacterRecognition;
                    break;
                case MediaProcessor.ContentModeration:
                    processorId = Constant.Media.ProcessorId.ContentModeration;
                    break;
            }
            return processorId;
        }

        public static string GetProcessorName(MediaProcessor mediaProcessor)
        {
            string processorName = mediaProcessor.ToString();
            return Regex.Replace(processorName, Constant.TextFormatter.SpacePattern, Constant.TextFormatter.SpaceReplacement);
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
                case Constant.Media.ProcessorId.EncoderUltra:
                    mediaProcessor = MediaProcessor.EncoderUltra;
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
                case Constant.Media.ProcessorId.SpeechToText:
                    mediaProcessor = MediaProcessor.SpeechToText;
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
                case Constant.Media.ProcessorId.MotionStabilization:
                    mediaProcessor = MediaProcessor.MotionStabilization;
                    break;
                case Constant.Media.ProcessorId.CharacterRecognition:
                    mediaProcessor = MediaProcessor.CharacterRecognition;
                    break;
                case Constant.Media.ProcessorId.ContentModeration:
                    mediaProcessor = MediaProcessor.ContentModeration;
                    break;
            }
            return mediaProcessor;
        }

        public static object GetMediaProcessors(string authToken, bool getEntities)
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
                MediaProcessor[] processors = GetMediaProcessors(authToken);
                foreach (MediaProcessor processor in processors)
                {
                    string processorName = GetProcessorName(processor);
                    processorNames.Add(processorName, processor.ToString());
                }
                mediaProcessors = processorNames;
            }
            return mediaProcessors;
        }
    }
}