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
        VideoIndexer,
        AudioIndexer
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaJobInputMode
    {
        InputFile,
        AssetFile,
        Asset
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaJobOutputMode
    {
        MultipleAssets,
        SingleAsset
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaProtection
    {
        AES,
        PlayReady,
        Widevine,
        FairPlay
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaInsightModel
    {
        Brand,
        Person,
        Language
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaClipType
    {
        Filter,
        Asset
    }
}