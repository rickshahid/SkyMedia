﻿using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaProtection
    {
        public MediaContentProtection Type { get; set; }

        public string AuthenticationToken { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertificateUrl { get; set; }
    }

    public class ContentProtection
    {
        public bool PersistentLicense { get; set; }
    }

    public class PolicyOverrides
    {
        public bool CanPlay { get; set; }

        public bool CanPersist { get; set; }

        public bool CanRenew { get; set; }

        public int RentalDurationSeconds { get; set; }

        public int PlaybackDurationSeconds { get; set; }

        public int LicenseDurationSeconds { get; set; }
    }

    public class ContentKeySpec
    {
        public string TrackType { get; set; }

        public int SecurityLevel { get; set; }

        public OutputProtection RequiredOutputProtection { get; set; }
    }

    public class OutputProtection
    {
        public string HDCP { get; set; }
    }

    public class WidevineTemplate
    {
        public string AllowedTrackTypes { get; set; }

        public ContentKeySpec[] ContentKeySpecs { get; set; }

        public PolicyOverrides PolicyOverrides { get; set; }
    }
}