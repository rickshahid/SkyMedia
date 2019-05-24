using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AzureSkyMedia.PlatformServices
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaEntity
    {
        Asset,
        Transform,
        TransformJob,
        ContentKeyPolicy,
        StreamingPolicy,
        StreamingEndpoint,
        StreamingLocator,
        StreamingFilterAccount,
        StreamingFilterAsset,
        LiveEvent,
        LiveEventOutput
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaTransformPreset
    {
        AdaptiveStreaming,
        ContentAwareEncoding,
        ThumbnailImages,
        ThumbnailSprite,
        VideoAnalyzer,
        AudioAnalyzer,
        FaceDetector,
        VideoIndexer,
        AudioIndexer
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaJobInputMode
    {
        Asset,
        File
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaContentProtection
    {
        AES,
        PlayReady,
        Widevine,
        FairPlay
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaInsightModel
    {
        People,
        Language,
        Brand
    }
}