namespace AzureSkyMedia.PlatformServices
{
    public enum MediaProtection
    {
        AES,
        PlayReady,
        Widevine,
        FairPlay
    }

    public enum MediaEntity
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

    public enum MediaProcessorConfig
    {
        PublicVideo,
        SearchPartition,
        TranscriptLanguage,
        CaptionFormatWebVtt,
        CaptionFormatTtml,
        SummarizationDurationSeconds,
        FaceDetectionMode,
        FaceRedactionMode,
        MotionDetectionSensitivityLevel,
        MotionDetectionLightChange,
        MotionHyperlapseStartFrame,
        MotionHyperlapseFrameCount,
        MotionHyperlapseSpeed
    }

    public enum MediaEncoding
    {
        None,
        Standard,
        Premium
    }

    public enum MediaPrivacy
    {
        Private,
        Public
    }

    public enum TransferService
    {
        SigniantFlight,
        AsperaFasp
    }
}
