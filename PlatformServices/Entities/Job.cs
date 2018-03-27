namespace AzureSkyMedia.PlatformServices
{
    public class MediaJob
    {
        public string Name { get; set; }

        public int Priority { get; set; }

        public MediaJobTask[] Tasks { get; set; }
    }
}