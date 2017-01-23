using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal struct AsperaRequest
    {
        [JsonProperty(PropertyName = "transfer_requests")]
        public TransferRequestItem[] TransferRequests { get; set; }
    }

    internal struct TransferRequestItem
    {
        [JsonProperty(PropertyName = "transfer_request")]
        public TransferRequest TransferRequest { get; set; }
    }

    internal struct TransferRequest
    {
        [JsonProperty(PropertyName = "source_root")]
        public string SourceRoot { get; set; }

        [JsonProperty(PropertyName = "destination_root")]
        public string DestinationRoot { get; set; }

        [JsonProperty(PropertyName = "rate_policy")]
        public string RatePolicy { get; set; }

        [JsonProperty(PropertyName = "target_rate_kbps")]
        public string TargetRateKbps { get; set; }

        [JsonProperty(PropertyName = "paths")]
        public TransferPath[] Paths { get; set; }
    }

    internal struct TransferPath
    {
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "destination")]
        public string Destination { get; set; }
    }
}
