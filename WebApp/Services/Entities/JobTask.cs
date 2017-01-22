using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.Services
{
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
}
