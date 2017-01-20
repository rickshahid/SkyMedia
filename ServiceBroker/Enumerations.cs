namespace AzureSkyMedia.ServiceBroker
{
    public enum MediaEntity
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

    public enum MediaProtection
    {
        AES,
        PlayReady,
        Widevine
    }

    public enum TransferService
    {
        SigniantFlight,
        AsperaFasp
    }
}
