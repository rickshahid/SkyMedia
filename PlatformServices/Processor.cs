using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Processor
    {
        public static string SetProcessorName(string processorName)
        {
            return Regex.Replace(processorName, Constant.TextFormatter.SpacePattern, Constant.TextFormatter.SpaceReplacement);
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
                case MediaProcessor.EncoderUltra:
                    processorId = Constant.Media.ProcessorId.EncoderUltra;
                    break;
                case MediaProcessor.Indexer:
                    processorId = Constant.Media.ProcessorId.Indexer;
                    break;
                case MediaProcessor.VideoAnnotation:
                    processorId = Constant.Media.ProcessorId.VideoAnnotation;
                    break;
                case MediaProcessor.VideoSummarization:
                    processorId = Constant.Media.ProcessorId.VideoSummarization;
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
            string processorName = SetProcessorName(mediaProcessor.ToString());
            return processorName.Replace(Constant.TextDelimiter.Identifier, ' ');
        }

        public static MediaProcessor GetProcessorType(string processorId)
        {
            MediaProcessor mediaProcessor = MediaProcessor.None;
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
                case Constant.Media.ProcessorId.Indexer:
                    mediaProcessor = MediaProcessor.Indexer;
                    break;
                case Constant.Media.ProcessorId.FaceDetection:
                    mediaProcessor = MediaProcessor.FaceDetection;
                    break;
                case Constant.Media.ProcessorId.FaceRedaction:
                    mediaProcessor = MediaProcessor.FaceRedaction;
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
                case Constant.Media.ProcessorId.MotionDetection:
                    mediaProcessor = MediaProcessor.MotionDetection;
                    break;
                case Constant.Media.ProcessorId.MotionHyperlapse:
                    mediaProcessor = MediaProcessor.MotionHyperlapse;
                    break;
                case Constant.Media.ProcessorId.MotionStabilization:
                    mediaProcessor = MediaProcessor.MotionStabilization;
                    break;
            }
            return mediaProcessor;
        }

        private static string GetAnalyticsProcessorName(string fileName)
        {
            string[] fileNameInfo = fileName.Split('_');
            string processorName = fileNameInfo[fileNameInfo.Length - 1];
            processorName = processorName.Replace(Constant.Media.FileExtension.Json, string.Empty);
            return Processor.SetProcessorName(processorName);
        }

        public static MediaMetadata[] GetAnalyticsMetadata(IAsset asset)
        {
            List<MediaMetadata> analyticsMetadata = new List<MediaMetadata>();
            string fileExtension = Constant.Media.FileExtension.Json;
            string[] fileNames = MediaClient.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                string processorName = GetAnalyticsProcessorName(fileName);
                MediaMetadata mediaMetadata = new MediaMetadata();
                mediaMetadata.ProcessorName = processorName;
                mediaMetadata.FileName = fileName;
                analyticsMetadata.Add(mediaMetadata);
            }
            return analyticsMetadata.ToArray();
        }
    }
}
