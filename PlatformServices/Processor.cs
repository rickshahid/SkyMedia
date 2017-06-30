using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Processor
    {
        private static MediaProcessor?[] GetProcessorTypes(string authToken)
        {
            CacheClient cacheClient = new CacheClient(authToken);
            string itemKey = Constant.Cache.ItemKey.MediaProcessors;
            List<MediaProcessor?> processorTypes = cacheClient.GetValues<MediaProcessor?>(itemKey);
            if (processorTypes.Count == 0)
            {
                MediaClient mediaClient = new MediaClient(authToken);
                IMediaProcessor[] mediaProcessors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
                foreach (IMediaProcessor mediaProcessor in mediaProcessors)
                {
                    MediaProcessor? processorType = GetProcessorType(mediaProcessor.Id);
                    if (processorType.HasValue)
                    {
                        processorTypes.Add(processorType);
                    }
                }
                cacheClient.SetValue<List<MediaProcessor?>>(itemKey, processorTypes);
            }
            return processorTypes.ToArray();
        }

        internal static MediaProcessor? GetProcessorType(string processorId)
        {
            MediaProcessor? processorType = null;
            switch (processorId)
            {
                case Constant.Media.ProcessorId.EncoderStandard:
                    processorType = MediaProcessor.EncoderStandard;
                    break;
                case Constant.Media.ProcessorId.EncoderPremium:
                    processorType = MediaProcessor.EncoderPremium;
                    break;
                case Constant.Media.ProcessorId.EncoderUltra:
                    processorType = MediaProcessor.EncoderUltra;
                    break;
                case Constant.Media.ProcessorId.SpeechToText:
                    processorType = MediaProcessor.SpeechToText;
                    break;
                case Constant.Media.ProcessorId.FaceDetection:
                    processorType = MediaProcessor.FaceDetection;
                    break;
                case Constant.Media.ProcessorId.FaceRedaction:
                    processorType = MediaProcessor.FaceRedaction;
                    break;
                case Constant.Media.ProcessorId.VideoAnnotation:
                    processorType = MediaProcessor.VideoAnnotation;
                    break;
                case Constant.Media.ProcessorId.VideoSummarization:
                    processorType = MediaProcessor.VideoSummarization;
                    break;
                case Constant.Media.ProcessorId.CharacterRecognition:
                    processorType = MediaProcessor.CharacterRecognition;
                    break;
                case Constant.Media.ProcessorId.ContentModeration:
                    processorType = MediaProcessor.ContentModeration;
                    break;
                case Constant.Media.ProcessorId.MotionDetection:
                    processorType = MediaProcessor.MotionDetection;
                    break;
                case Constant.Media.ProcessorId.MotionHyperlapse:
                    processorType = MediaProcessor.MotionHyperlapse;
                    break;
                case Constant.Media.ProcessorId.MotionStabilization:
                    processorType = MediaProcessor.MotionStabilization;
                    break;
            }
            return processorType;
        }

        internal static string GetProcessorName(MediaProcessor processorType)
        {
            string processorName = processorType.ToString();
            return Regex.Replace(processorName, Constant.TextFormatter.SpacePattern, Constant.TextFormatter.SpaceReplacement);
        }

        internal static string GetProcessorId(MediaProcessor processorType)
        {
            string processorId = null;
            switch (processorType)
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
                case MediaProcessor.SpeechToText:
                    processorId = Constant.Media.ProcessorId.SpeechToText;
                    break;
                case MediaProcessor.FaceDetection:
                    processorId = Constant.Media.ProcessorId.FaceDetection;
                    break;
                case MediaProcessor.FaceRedaction:
                    processorId = Constant.Media.ProcessorId.FaceRedaction;
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
                case MediaProcessor.MotionDetection:
                    processorId = Constant.Media.ProcessorId.MotionDetection;
                    break;
                case MediaProcessor.MotionHyperlapse:
                    processorId = Constant.Media.ProcessorId.MotionHyperlapse;
                    break;
                case MediaProcessor.MotionStabilization:
                    processorId = Constant.Media.ProcessorId.MotionStabilization;
                    break;
            }
            return processorId;
        }

        internal static string[] GetProcessorIds()
        {
            List<string> processorIds = new List<string>();
            processorIds.Add(Constant.Media.ProcessorId.EncoderStandard);
            processorIds.Add(Constant.Media.ProcessorId.EncoderPremium);
            processorIds.Add(Constant.Media.ProcessorId.EncoderUltra);
            processorIds.Add(Constant.Media.ProcessorId.SpeechToText);
            processorIds.Add(Constant.Media.ProcessorId.FaceDetection);
            processorIds.Add(Constant.Media.ProcessorId.FaceRedaction);
            processorIds.Add(Constant.Media.ProcessorId.VideoAnnotation);
            processorIds.Add(Constant.Media.ProcessorId.VideoSummarization);
            processorIds.Add(Constant.Media.ProcessorId.CharacterRecognition);
            processorIds.Add(Constant.Media.ProcessorId.ContentModeration);
            processorIds.Add(Constant.Media.ProcessorId.MotionDetection);
            processorIds.Add(Constant.Media.ProcessorId.MotionHyperlapse);
            processorIds.Add(Constant.Media.ProcessorId.MotionStabilization);
            return processorIds.ToArray();
        }

        public static object GetMediaProcessors(string authToken, bool inventoryView)
        {
            object mediaProcessors;
            if (inventoryView)
            {
                MediaClient mediaClient = new MediaClient(authToken);
                mediaProcessors = mediaClient.GetEntities(MediaEntity.Processor);
            }
            else
            {
                NameValueCollection processors = new NameValueCollection();
                MediaProcessor?[] processorTypes = GetProcessorTypes(authToken);
                foreach (MediaProcessor? processorType in processorTypes)
                {
                    if (processorType.HasValue)
                    {
                        string processorName = GetProcessorName(processorType.Value);
                        processors.Add(processorName, processorType.ToString());
                    }
                }
                mediaProcessors = processors;
            }
            return mediaProcessors;
        }
    }
}
