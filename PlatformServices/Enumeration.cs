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
        None,
        EncoderStandard,
        EncoderPremium,
        EncoderUltra,
        Indexer,
        FaceDetection,
        FaceRedaction,
        VideoAnnotation,
        VideoSummarization,
        CharacterRecognition,
        ContentModeration,
        MotionDetection,
        MotionHyperlapse,
        MotionStabilization
    }

    public enum TransferService
    {
        SigniantFlight,
        AsperaFasp
    }
}
