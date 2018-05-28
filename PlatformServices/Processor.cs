using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Processor
    {
        //private static string[] GetProcessorIds(bool presetsView)
        //{
        //    List<string> processorIds = new List<string>();
        //    processorIds.Add(Constant.Media.ProcessorId.EncoderStandard);
        //    if (!presetsView)
        //    {
        //        processorIds.Add(Constant.Media.ProcessorId.EncoderPremium);
        //    }
        //    processorIds.Add(Constant.Media.ProcessorId.AudioAnalyzer);
        //    if (!presetsView)
        //    {
        //        processorIds.Add(Constant.Media.ProcessorId.VideoAnalyzer);
        //    }
        //    processorIds.Add(Constant.Media.ProcessorId.VideoSummarization);
        //    processorIds.Add(Constant.Media.ProcessorId.FaceDetection);
        //    processorIds.Add(Constant.Media.ProcessorId.MotionDetection);
        //    return processorIds.ToArray();
        //}

        //private static MediaProcessor[] GetMediaProcessors(IMediaProcessor[] processors, bool presetsView)
        //{
        //    List<MediaProcessor> mediaProcessors = new List<MediaProcessor>();
        //    string[] processorIds = GetProcessorIds(presetsView);
        //    foreach (string processorId in processorIds)
        //    {
        //        if (!processorId.StartsWith(Constant.Media.IdPrefix.Processor, StringComparison.OrdinalIgnoreCase))
        //        {
        //            MediaProcessor mediaProcessor = (MediaProcessor)Enum.Parse(typeof(MediaProcessor), processorId);
        //            mediaProcessors.Add(mediaProcessor);
        //        }
        //        else
        //        {
        //            foreach (IMediaProcessor processor in processors)
        //            {
        //                if (string.Equals(processorId, processor.Id, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    MediaProcessor? mediaProcessor = GetMediaProcessor(processor.Id);
        //                    if (mediaProcessor.HasValue)
        //                    {
        //                        mediaProcessors.Add(mediaProcessor.Value);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return mediaProcessors.ToArray();
        //}

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
                //case Constant.Media.ProcessorId.AudioAnalyzer:
                //    mediaProcessor = MediaProcessor.AudioAnalyzer;
                //    break;
                //case Constant.Media.ProcessorId.VideoAnalyzer:
                //    mediaProcessor = MediaProcessor.VideoAnalyzer;
                //    break;
                //case Constant.Media.ProcessorId.VideoSummarization:
                //    mediaProcessor = MediaProcessor.VideoSummarization;
                //    break;
                //case Constant.Media.ProcessorId.FaceDetection:
                //    mediaProcessor = MediaProcessor.FaceDetection;
                //    break;
                //case Constant.Media.ProcessorId.FaceRedaction:
                //    mediaProcessor = MediaProcessor.FaceDetection;
                //    break;
                //case Constant.Media.ProcessorId.MotionDetection:
                //    mediaProcessor = MediaProcessor.MotionDetection;
                //    break;
            }
            return mediaProcessor;
        }

        public static Dictionary<string, string> GetMediaProcessors(string authToken, bool presetsView, bool getEntities)
        {
            Dictionary<string, string> mediaProcessors = new Dictionary<string, string>();

            mediaProcessors.Add("EncoderStandard", "Encoder Standard");
            mediaProcessors.Add("EncoderPremium", "Encoder Premium");
            mediaProcessors.Add("VideoIndexer", "Video Indexer");

            return mediaProcessors;

            //MediaClient mediaClient = new MediaClient(authToken);
            //object mediaProcessors = mediaClient.GetEntities(MediaEntity.Processor);
            //if (!getEntities)
            //{
            //    StringDictionary processorNames = new StringDictionary();
            //    MediaProcessor[] processors = GetMediaProcessors((IMediaProcessor[])mediaProcessors, presetsView);
            //    foreach (MediaProcessor processor in processors)
            //    {
            //        string processorName = GetProcessorName(processor);
            //        processorNames.Add(processorName, processor.ToString());
            //    }
            //    mediaProcessors = processorNames;
            //}
            //return mediaProcessors;
        }

        //public static string GetProcessorId(MediaProcessor mediaProcessor, string processorConfig)
        //{
        //    string processorId = null;
        //    switch (mediaProcessor)
        //    {
        //        case MediaProcessor.EncoderStandard:
        //            processorId = Constant.Media.ProcessorId.EncoderStandard;
        //            break;
        //        case MediaProcessor.EncoderPremium:
        //            processorId = Constant.Media.ProcessorId.EncoderPremium;
        //            break;
        //        case MediaProcessor.AudioAnalyzer:
        //            processorId = Constant.Media.ProcessorId.AudioAnalyzer;
        //            break;
        //        case MediaProcessor.VideoAnalyzer:
        //            processorId = Constant.Media.ProcessorId.VideoAnalyzer;
        //            break;
        //        case MediaProcessor.VideoSummarization:
        //            processorId = Constant.Media.ProcessorId.VideoSummarization;
        //            break;
        //        case MediaProcessor.FaceDetection:
        //            processorId = Constant.Media.ProcessorId.FaceRedaction;
        //            if (!string.IsNullOrEmpty(processorConfig))
        //            {
        //                JToken processorOptions = JObject.Parse(processorConfig)["Options"];
        //                string processorMode = processorOptions["Mode"].ToString();
        //                if (processorMode == "PerFaceEmotion" || processorMode == "AggregateEmotion")
        //                {
        //                    processorId = Constant.Media.ProcessorId.FaceDetection;
        //                }
        //            }
        //            break;
        //        case MediaProcessor.MotionDetection:
        //            processorId = Constant.Media.ProcessorId.MotionDetection;
        //            break;
        //    }
        //    return processorId;
        //}

        public static string GetProcessorName(MediaProcessor mediaProcessor)
        {
            string processorName = mediaProcessor.ToString();
            return Regex.Replace(processorName, Constant.TextFormatter.SpacePattern, Constant.TextFormatter.SpaceReplacement);
        }

        public static Dictionary<string, string> GetProcessorPresets(MediaProcessor mediaProcessor, string accountName)
        {
            Dictionary<string, string> processorPresets = new Dictionary<string, string>();
            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constant.Database.Collection.ProcessorPreset;
            string accountPresetId = string.Concat(accountName, Constant.TextDelimiter.Identifier, mediaProcessor.ToString());
            JObject[] presets = databaseClient.GetDocuments(collectionId);
            foreach (JObject preset in presets)
            {
                string presetId = preset["id"].ToString();
                string presetProcessor = preset["MediaProcessor"].ToString();
                string[] presetIdInfo = presetId.Split(Constant.TextDelimiter.Identifier);
                bool customPreset = presetIdInfo.Length == 3 ? true : false;
                if ((!customPreset && presetProcessor.StartsWith(mediaProcessor.ToString(), StringComparison.OrdinalIgnoreCase)) ||
                    (customPreset && presetId.StartsWith(accountPresetId, StringComparison.Ordinal)))
                {
                    string presetName = preset["PresetName"].ToString();
                    processorPresets.Add(presetName, presetId);
                }
            }
            return processorPresets;
        }
    }
}