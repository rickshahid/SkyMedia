using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJobTask
    {
        public MediaJobTask CreateCopy()
        {
            return (MediaJobTask)this.MemberwiseClone();
        }

        public int? ParentIndex { get; set; }

        public string Name { get; set; }

        public string[] InputAssetIds { get; set; }

        public MediaProcessor ProcessorType { get; set; }

        public string ProcessorConfig { get; set; }

        public string ProcessorDocumentId { get; set; }

        public string OutputAssetName { get; set; }

        public AssetCreationOptions OutputAssetEncryption { get; set; }

        public AssetFormatOption OutputAssetFormat { get; set; }

        public TaskOptions Options { get; set; }

        public bool GenerateThumbnails { get; set; }

        public ContentProtection ContentProtection { get; set; }

        public string[] SpeechToTextLanguages { get; set; }

        public bool SpeechToTextCaptionWebVtt { get; set; }

        public bool SpeechToTextCaptionTtml { get; set; }

        public string FaceDetectionMode { get; set; }

        public string FaceRedactionMode { get; set; }

        public int SummarizationDurationSeconds { get; set; }

        public string MotionDetectionSensitivityLevel { get; set; }

        public bool MotionDetectionLightChange { get; set; }

        public int MotionHyperlapseStartFrame { get; set; }

        public int MotionHyperlapseFrameCount { get; set; }

        public int MotionHyperlapseSpeed { get; set; }
    }
}
