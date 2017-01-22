using Microsoft.AspNetCore.Mvc.Rendering;

namespace AzureSkyMedia.WebApp.Models
{
    public struct MediaStream
    {
        public string Name { get; set; }

        public string SourceUrl { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        public string[] ProtectionTypes { get; set; }

        public SelectListItem[] AnalyticsProcessors { get; set; }
    }
}
