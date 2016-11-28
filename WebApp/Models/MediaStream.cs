using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json;
    
namespace SkyMedia.WebApp.Models
{
    public struct MediaTrack
    {
        [JsonProperty(PropertyName = "kind")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "src")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "srclang")]
        public string Language { get; set; }
    }

    public struct MediaStream
    {
        public string Name { get; set; }

        public string SourceUrl { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        public string[] ProtectionTypes { get; set; }

        public SelectListItem[] AnalyticsProcessors { get; set; }
    }
}
