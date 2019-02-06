using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AzureSkyMedia.PlatformServices
{
    public class Error
    {
        [JsonProperty(PropertyName = "ErrorType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public HttpStatusCode Type { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }
    }
}