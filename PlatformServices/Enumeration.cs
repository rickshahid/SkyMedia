using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AzureSkyMedia.PlatformServices
{
    internal enum MediaEntity
    {
        MonitoringConfiguration,
        StorageAccount,
        ContentKey,
        ContentKeyAuthPolicy,
        ContentKeyAuthPolicyOption,
        Manifest,
        ManifestAsset,
        ManifestFile,
        Asset,
        File,
        Channel,
        Program,
        Processor,
        ProcessorUnit,
        Job,
        JobTemplate,
        NotificationEndpoint,
        AccessPolicy,
        DeliveryPolicy,
        StreamingEndpoint,
        StreamingFilter,
        Locator
    }

    internal enum MediaProcessorConfig
    {
        SpokenLanguage,
        SearchPartition,
        FaceDetectionMode,
        FaceDetectionTrackingMode,
        FaceDetectionAggregateEmotionWindow,
        FaceDetectionAggregateEmotionInterval,
        FaceRedactionMode,
        FaceRedactionBlurMode,
        CaptionFormatTtml,
        CaptionFormatWebVtt,
        SummarizationDurationSeconds,
        SummarizationFadeTransitions,
        MotionDetectionSensitivityLevel,
        MotionDetectionLightChange,
        MotionHyperlapseFrameStart,
        MotionHyperlapseFrameEnd,
        MotionHyperlapseSpeed,
        VideoDescription,
        VideoMetadata,
        VideoPublic,
        VideoOnly,
        AudioOnly
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaProcessor
    {
        EncoderStandard,
        EncoderPremium,
        VideoIndexer,
        VideoAnnotation,
        VideoSummarization,
        CharacterRecognition,
        SpeechAnalyzer,
        FaceDetection,
        FaceRedaction,
        MotionDetection,
        MotionHyperlapse,
        ContentModeration
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaProtocol
    {
        FMP4,
        RTMP,
        RTP
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaEncoding
    {
        None,
        Standard,
        Premium
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaProtection
    {
        AES,
        PlayReady,
        Widevine,
        FairPlay
    }
}