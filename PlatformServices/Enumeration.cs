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
        ContentAwareEncoding,
        AdaptiveStreaming,
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
        InputFile,
        AssetFile,
        Asset
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
        Language,
        Person,
        Brand
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaClipType
    {
        Filter,
        Asset
    }
}