using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.Services
{
    internal struct MediaJobEventProperties
    {
        public string AccountName { get; set; }

        public string JobId { get; set; }

        public string JobName { get; set; }

        public JobState OldState { get; set; }

        public JobState NewState { get; set; }
    }
}
