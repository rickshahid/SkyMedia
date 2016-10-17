using System.Runtime.Serialization;

namespace SkyMedia.WebApp.Models
{
    [DataContract]
    public class AsperaRequest
    {
        [DataMember(Name = "transfer_requests")]
        public TransferRequestItem[] TransferRequests { get; set; }
    }

    [DataContract]
    public class TransferRequestItem
    {
        [DataMember(Name = "transfer_request")]
        public TransferRequest TransferRequest { get; set; }
    }

    [DataContract]
    public class TransferRequest
    {
        [DataMember(Name = "source_root")]
        public string SourceRoot { get; set; }

        [DataMember(Name = "destination_root")]
        public string DestinationRoot { get; set; }

        [DataMember(Name = "cipher")]
        public string Cipher { get; set; }

        [DataMember(Name = "rate_policy")]
        public string RatePolicy { get; set; }

        [DataMember(Name = "target_rate_kbps")]
        public string TargetRateKbps { get; set; }

        [DataMember(Name = "paths")]
        public TransferPath[] Paths { get; set; }
    }

    [DataContract]
    public class TransferPath
    {
        public TransferPath() { }

        public TransferPath(string source, string destination) {
            Source = source;
            Destination = destination;
        }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "destination")]
        public string Destination { get; set; }
    }
}
