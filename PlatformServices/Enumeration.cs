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
        StreamingLocator,
        StreamingFilter
    }

    internal enum MediaProcessorConfig
    {
        AudioAnalyzerLanguageId,
        AudioAnalyzerTimedTextFormatTtml,
        AudioAnalyzerTimedTextFormatWebVtt,
        VideoSummarizationDurationSeconds,
        VideoSummarizationFadeTransitions,
        VideoSummarizationIncludeAudio,
        FaceDetectionMode,
        FaceRedactionBlurMode,
        FaceEmotionAggregateWindow,
        FaceEmotionAggregateInterval,
        MotionDetectionSensitivityLevel,
        MotionDetectionLightChange
    }

    public enum MediaProcessor
    {
        EncoderStandard,
        EncoderPremium,
        VideoIndexer,

        AudioAnalyzer,
        VideoAnalyzer,

        VideoSummarization,
        FaceDetection,
        MotionDetection
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaProtocol
    {
        FMP4,
        RTMP
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
        Png,
        Jpg,
        Bmp,
        Sprite
    }
}