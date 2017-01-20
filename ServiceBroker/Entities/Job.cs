namespace AzureSkyMedia.ServiceBroker
{
    public struct MediaJob
    {
        public string Name { get; set; }

        public int Priority { get; set; }

        public MediaJobTask[] Tasks { get; set; }
    }
}