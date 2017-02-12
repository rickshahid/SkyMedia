namespace AzureSkyMedia.PlatformServices
{
    internal enum MediaProtection
    {
        AES,
        PlayReady,
        Widevine
    }

    public enum MediaJobNotificationEvent
    {
        None = 0,
        JobStateChange = 1,
        NotificationEndPointRegistration = 2,
        NotificationEndPointUnregistration = 3,
        TaskStateChange = 4,
        TaskProgress = 5
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
        None,
        EncoderStandard,
        EncoderPremium,
        EncoderUltra,
        IndexerV1,
        IndexerV2,
        FaceDetection,
        FaceRedaction,
        MotionDetection,
        MotionHyperlapse,
        MotionStabilization,
        VideoAnnotation,
        VideoSummarization,
        ThumbnailGeneration,
        CharacterRecognition,
        ContentModeration
    }

    public enum TransferService
    {
        SigniantFlight,
        AsperaFasp
    }
}
