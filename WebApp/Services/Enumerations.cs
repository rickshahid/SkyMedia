namespace AzureSkyMedia.Services
{
    internal enum MediaEntity
    {
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

    internal enum MediaProtection
    {
        AES,
        PlayReady,
        Widevine
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
        MotionStablization,
        VideoAnnotation,
        VideoSummarization,
        CharacterRecognition,
        ThumbnailGeneration,
        ContentModeration
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

    public enum TransferService
    {
        SigniantFlight,
        AsperaFasp
    }
}
