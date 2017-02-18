namespace AzureSkyMedia.PlatformServices
{
    public static class Processors
    {
        public static string GetMediaProcessorId(MediaProcessor mediaProcessor)
        {
            string processorId = null;
            switch (mediaProcessor)
            {
                case MediaProcessor.EncoderStandard:
                    processorId = Constants.Media.ProcessorId.EncoderStandard;
                    break;
                case MediaProcessor.EncoderPremium:
                    processorId = Constants.Media.ProcessorId.EncoderPremium;
                    break;
                case MediaProcessor.EncoderUltra:
                    processorId = Constants.Media.ProcessorId.EncoderUltra;
                    break;
                case MediaProcessor.IndexerV1:
                    processorId = Constants.Media.ProcessorId.IndexerV1;
                    break;
                case MediaProcessor.IndexerV2:
                    processorId = Constants.Media.ProcessorId.IndexerV2;
                    break;
                case MediaProcessor.FaceDetection:
                    processorId = Constants.Media.ProcessorId.FaceDetection;
                    break;
                case MediaProcessor.FaceRedaction:
                    processorId = Constants.Media.ProcessorId.FaceRedaction;
                    break;
                case MediaProcessor.MotionDetection:
                    processorId = Constants.Media.ProcessorId.MotionDetection;
                    break;
                case MediaProcessor.MotionHyperlapse:
                    processorId = Constants.Media.ProcessorId.MotionHyperlapse;
                    break;
                case MediaProcessor.MotionStabilization:
                    processorId = Constants.Media.ProcessorId.MotionStabilization;
                    break;
                case MediaProcessor.VideoAnnotation:
                    processorId = Constants.Media.ProcessorId.VideoAnnotation;
                    break;
                case MediaProcessor.VideoSummarization:
                    processorId = Constants.Media.ProcessorId.VideoSummarization;
                    break;
                case MediaProcessor.ThumbnailGeneration:
                    processorId = Constants.Media.ProcessorId.ThumbnailGeneration;
                    break;
                case MediaProcessor.CharacterRecognition:
                    processorId = Constants.Media.ProcessorId.CharacterRecognition;
                    break;
                case MediaProcessor.ContentModeration:
                    processorId = Constants.Media.ProcessorId.ContentModeration;
                    break;
            }
            return processorId;
        }

        public static MediaProcessor GetMediaProcessorType(string processorId)
        {
            MediaProcessor mediaProcessor = MediaProcessor.None;
            switch (processorId)
            {
                case Constants.Media.ProcessorId.EncoderStandard:
                    mediaProcessor = MediaProcessor.EncoderStandard;
                    break;
                case Constants.Media.ProcessorId.EncoderPremium:
                    mediaProcessor = MediaProcessor.EncoderPremium;
                    break;
                case Constants.Media.ProcessorId.EncoderUltra:
                    mediaProcessor = MediaProcessor.EncoderUltra;
                    break;
                case Constants.Media.ProcessorId.IndexerV1:
                    mediaProcessor = MediaProcessor.IndexerV1;
                    break;
                case Constants.Media.ProcessorId.IndexerV2:
                    mediaProcessor = MediaProcessor.IndexerV2;
                    break;
                case Constants.Media.ProcessorId.FaceDetection:
                    mediaProcessor = MediaProcessor.FaceDetection;
                    break;
                case Constants.Media.ProcessorId.FaceRedaction:
                    mediaProcessor = MediaProcessor.FaceRedaction;
                    break;
                case Constants.Media.ProcessorId.MotionDetection:
                    mediaProcessor = MediaProcessor.MotionDetection;
                    break;
                case Constants.Media.ProcessorId.MotionHyperlapse:
                    mediaProcessor = MediaProcessor.MotionHyperlapse;
                    break;
                case Constants.Media.ProcessorId.MotionStabilization:
                    mediaProcessor = MediaProcessor.MotionStabilization;
                    break;
                case Constants.Media.ProcessorId.VideoAnnotation:
                    mediaProcessor = MediaProcessor.VideoAnnotation;
                    break;
                case Constants.Media.ProcessorId.VideoSummarization:
                    mediaProcessor = MediaProcessor.VideoSummarization;
                    break;
                case Constants.Media.ProcessorId.ThumbnailGeneration:
                    mediaProcessor = MediaProcessor.ThumbnailGeneration;
                    break;
                case Constants.Media.ProcessorId.CharacterRecognition:
                    mediaProcessor = MediaProcessor.CharacterRecognition;
                    break;
                case Constants.Media.ProcessorId.ContentModeration:
                    mediaProcessor = MediaProcessor.ContentModeration;
                    break;
            }
            return mediaProcessor;
        }
    }
}
