using System;

namespace AzureSkyMedia.Services
{
    internal class MediaJobEvent
    {
        public MediaJobEvent()
        {
            MediaJobEventProperties jobEventProperties = new MediaJobEventProperties();
            jobEventProperties.JobId = string.Empty;
            this.EventProperties = jobEventProperties;
        }

        public MediaJobEventProperties EventProperties { get; set; }

        public string MessageVersion { get; set; }

        public string EventType { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ETag { get; set; }

        //public string MessageId { get; set; }

        //public string PopReceipt { get; set; }
    }
}
