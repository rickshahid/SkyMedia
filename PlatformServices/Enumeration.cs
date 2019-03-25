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
        FilterAccount,
        FilterAsset,
        LiveEvent,
        LiveEventOutput
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaTransformPreset
    {
        AdaptiveStreaming,
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
        InputAsset,
        OutputAsset,
        OutputAssets
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaImageFormat
    {
        JPG,
        PNG,
        BMP
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
    public enum MediaClipType
    {
        Asset,
        Filter
    }
}