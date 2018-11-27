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
        ThumbnailImages = 2,
        VideoAnalyzer = 4,
        AudioAnalyzer = 8,
        VideoIndexer = 16,
        AudioIndexer = 32
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaJobInputMode
    {
        UploadFile,
        AssetFile,
        Asset
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaJobOutputMode
    {
        SingleAsset,
        InputAsset,
        DistinctAssets
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaClipType
    {
        Asset,
        Filter
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