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
        FaceRedactionMode,
        CaptionFormatTtml,
        CaptionFormatWebVtt,
        SummarizationDurationSeconds,
        SummarizationFadeTransitions,
        MotionDetectionSensitivityLevel,
        MotionDetectionLightChange,
        MotionHyperlapseStartFrame,
        MotionHyperlapseFrameCount,
        MotionHyperlapseSpeed,
        VideoDescription,
        VideoMetadata,
        VideoPublic,
        VideoOnly,
        AudioOnly
    }

    public enum MediaProcessor
    {
        EncoderStandard,
        EncoderPremium,
        EncoderUltra,
        VideoIndexer,
        VideoAnnotation,
        VideoSummarization,
        SpeechToText,
        FaceDetection,
        FaceRedaction,
        MotionDetection,
        MotionHyperlapse,
        MotionStabilization,
        CharacterRecognition,
        ContentModeration
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

    public enum TransferService
    {
        SigniantFlight,
        AsperaFasp
    }
}