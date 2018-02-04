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
        SummarizationDurationSeconds,
        SummarizationFadeTransitions,
        SummarizationIncludeAudio,
        FaceDetectionMode,
        FaceRedactionBlurMode,
        FaceEmotionAggregateWindow,
        FaceEmotionAggregateInterval,
        SpeechAnalyzerLanguageId,
        SpeechAnalyzerTimedTextFormatTtml,
        SpeechAnalyzerTimedTextFormatWebVtt,
        MotionDetectionSensitivityLevel,
        MotionDetectionLightChange
    }

    public enum MediaProcessor
    {
        EncoderStandard,
        EncoderPremium,
        VideoIndexer,
        VideoAnnotation,
        VideoSummarization,
        FaceDetection,
        SpeechAnalyzer,
        MotionDetection,
        ContentModeration,
        CharacterRecognition
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

    public enum MediaThumbnailFormat
    {
        JPG,
        PNG,
        BMP
    }
}