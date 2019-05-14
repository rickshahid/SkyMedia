using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AzureSkyMedia.PlatformServices
{
    internal enum MediaEntity
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
        Container,
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaClipType
    {
        Filter,
        Asset
    }
}