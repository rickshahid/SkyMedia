using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaJobTask
    {
        public MediaJobTask()
        {
            this.ProcessorConfigBoolean = new Dictionary<MediaProcessorConfig, bool>();
            this.ProcessorConfigInteger = new Dictionary<MediaProcessorConfig, int>();
            this.ProcessorConfigString = new Dictionary<MediaProcessorConfig, string>();
        }

        public MediaJobTask CreateCopy()
        {
            MediaJobTask newTask = (MediaJobTask)this.MemberwiseClone();
            foreach (KeyValuePair<MediaProcessorConfig, bool> keyValue in this.ProcessorConfigBoolean)
            {
                newTask.ProcessorConfigBoolean.Add(keyValue.Key, keyValue.Value);
            }
            foreach (KeyValuePair<MediaProcessorConfig, int> keyValue in this.ProcessorConfigInteger)
            {
                newTask.ProcessorConfigInteger.Add(keyValue.Key, keyValue.Value);
            }
            foreach (KeyValuePair<MediaProcessorConfig, string> keyValue in this.ProcessorConfigString)
            {
                newTask.ProcessorConfigString.Add(keyValue.Key, keyValue.Value);
            }
            return newTask;
        }

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

        public Dictionary<MediaProcessorConfig, bool> ProcessorConfigBoolean { get; set; }

        public Dictionary<MediaProcessorConfig, int> ProcessorConfigInteger { get; set; }

        public Dictionary<MediaProcessorConfig, string> ProcessorConfigString { get; set; }
    }
}
