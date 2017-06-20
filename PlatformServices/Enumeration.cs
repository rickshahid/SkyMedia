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
        SpeechToText,
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

    public enum MediaInsight
    {
        Search,
        People,
        Keywords,
        Sentiments
    }

    public enum TransferService
    {
        SigniantFlight,
        AsperaFasp
    }
}
