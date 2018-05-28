using System.Collections.Generic;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJobTask
    {
        public MediaJobTask DeepCopy()
        {
            MediaJobTask jobTask = (MediaJobTask)this.MemberwiseClone();
            jobTask.ProcessorConfigString = new Dictionary<string, string>(this.ProcessorConfigString);
            jobTask.ProcessorConfigBoolean = new Dictionary<string, bool>(this.ProcessorConfigBoolean);
            jobTask.ProcessorConfigInteger = new Dictionary<string, int>(this.ProcessorConfigInteger);
            return jobTask;
        }

        public MediaJobTask()
        {
            this.ProcessorConfigString = new Dictionary<string, string>();
            this.ProcessorConfigBoolean = new Dictionary<string, bool>();
            this.ProcessorConfigInteger = new Dictionary<string, int>();
        }

        public int? ParentIndex { get; set; }

        public string Name { get; set; }

        public string[] InputAssetIds { get; set; }

        public MediaProcessor MediaProcessor { get; set; }

        public string ProcessorConfigId { get; set; }

        public string ProcessorConfig { get; set; }

        public string OutputAssetName { get; set; }

        //public AssetFormatOption OutputAssetFormat { get; set; }

        //public AssetCreationOptions OutputAssetEncryption { get; set; }

        public Dictionary<string, string> ProcessorConfigString { get; set; }

        public Dictionary<string, bool> ProcessorConfigBoolean { get; set; }

        public Dictionary<string, int> ProcessorConfigInteger { get; set; }

        public ThumbnailGeneration ThumbnailGeneration { get; set; }

        public ContentProtection ContentProtection { get; set; }

        public MediaInsightConfig InsightConfig { get; set; }

        //public TaskOptions Options { get; set; }
    }
}