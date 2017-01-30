using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public string Name { get; set; }

        public ReservedUnitType Scale { get; set; }

        public int Priority { get; set; }

        public NotificationEndPointType Notification { get; set; }

        public MediaJobTask[] Tasks { get; set; }
    }
}
