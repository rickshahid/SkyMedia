using System.Collections.Specialized;

namespace AzureSkyMedia.PlatformServices
{
    public struct MediaStream
    {
        public string Name { get; set; }

        public string SourceUrl { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        public string[] ProtectionTypes { get; set; }

        public NameValueCollection AnalyticsProcessors { get; set; }
    }
}
