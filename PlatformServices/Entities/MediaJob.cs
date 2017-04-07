using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public bool Save { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public string TemplateId { get; set; }

        public MediaJobTask[] Tasks { get; set; }

        public ReservedUnitType NodeType { get; set; }
    }
}
