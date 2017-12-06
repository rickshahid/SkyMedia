using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public string Name { get; set; }

        public ReservedUnitType NodeType { get; set; }

        public int Priority { get; set; }

        public MediaJobTask[] Tasks { get; set; }

        public string TemplateId { get; set; }

        public bool SaveWorkflow { get; set; }
    }
}