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
        StreamingFilter,
        LiveEvent,
        LiveOutput
    }

    public enum MediaProcessor
    {
        EncoderStandard,
        EncoderPremium,
        VideoIndexer
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
        PNG,
        JPG,
        BMP
    }
}