using System;

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

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaTransformPreset
    {
        AdaptiveStreaming = 1,
        ThumbnailSprite = 2,
        VideoAnalyzer = 4,
        AudioAnalyzer = 8,
        VideoIndexer = 16,
        AudioIndexer = 32
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaJobInputType
    {
        UploadFile,
        AssetFile,
        Asset
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaJobOutputAssetMode
    {
        DistinctAssets,
        InputAsset,
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
    public enum MediaImageFormat
    {
        JPG,
        PNG,
        BMP
    }
}