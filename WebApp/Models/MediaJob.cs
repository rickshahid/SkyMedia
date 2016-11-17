using System;

using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.ServiceBroker;

namespace SkyMedia.WebApp.Models
{
    internal struct MediaJob
    {
        public string Name { get; set; }

        public int Priority { get; set; }

        public MediaJobTask[] Tasks { get; set; }
    }

    public class MediaJobTask
    {
        public int? ParentIndex { get; set; }

        public string Name { get; set; }

        public string[] InputAssetIds { get; set; }

        public MediaProcessor MediaProcessor { get; set; }

        public string ProcessorConfig { get; set; }

        public string ProcessorDocumentId { get; set; }

        public string OutputAssetName { get; set; }

        public AssetCreationOptions OutputAssetEncryption { get; set; }

        public TaskOptions Options { get; set; }

        public string[] SpokenLanguages { get; set; }

        public bool CaptionFormatWebVtt { get; set; }

        public bool CaptionFormatTtml { get; set; }

        public string SensitivityLevel { get; set; }

        public bool DetectLightChange { get; set; }

        public int StartFrame { get; set; }

        public int FrameCount { get; set; }

        public int SpeedMultiplier { get; set; }

        public int DurationSeconds { get; set; }
    }

    internal struct MediaJobEventProperties
    {
        public string AccountName { get; set; }

        public string JobId { get; set; }

        public string JobName { get; set; }

        public JobState OldState { get; set; }

        public JobState NewState { get; set; }
    }

    internal class MediaJobEvent
    {
        public string MessageVersion { get; set; }

        public string EventType { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ETag { get; set; }

        public MediaJobEventProperties Properties { get; set; }
    }
}
