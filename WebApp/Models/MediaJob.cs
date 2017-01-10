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

        public AssetFormatOption OutputAssetFormat { get; set; }

        public TaskOptions Options { get; set; }

        public ContentProtection ContentProtection { get; set; }

        public string[] IndexerSpokenLanguages { get; set; }

        public bool IndexerCaptionWebVtt { get; set; }

        public bool IndexerCaptionTtml { get; set; }

        public string FaceDetectionMode { get; set; }

        public string FaceRedactionMode { get; set; }

        public string MotionSensitivityLevel { get; set; }

        public bool MotionDetectLightChange { get; set; }

        public int HyperlapseStartFrame { get; set; }

        public int HyperlapseFrameCount { get; set; }

        public int HyperlapseSpeed { get; set; }

        public int SummaryDurationSeconds { get; set; }
    }

    public class MediaJobEvent
    {
        public MediaJobEvent()
        {
            MediaJobEventProperties jobEventProperties = new MediaJobEventProperties();
            jobEventProperties.JobId = string.Empty;
            this.Properties = jobEventProperties;
        }

        public string MessageVersion { get; set; }

        public string EventType { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ETag { get; set; }

        public MediaJobEventProperties Properties { get; set; }
    }

    public struct MediaJobEventProperties
    {
        public string AccountName { get; set; }

        public string JobId { get; set; }

        public string JobName { get; set; }

        public JobState OldState { get; set; }

        public JobState NewState { get; set; }
    }
}
