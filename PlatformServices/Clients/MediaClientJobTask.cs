using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static ITask[] GetJobTasks(IJob job, string[] processorIds)
        {
            List<ITask> jobTasks = new List<ITask>();
            foreach (ITask jobTask in job.Tasks)
            {
                if (processorIds.Contains(jobTask.MediaProcessorId, StringComparer.OrdinalIgnoreCase))
                {
                    jobTasks.Add(jobTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static string[] GetAssetIds(MediaJobInput[] jobInputs)
        {
            List<string> assetIds = new List<string>();
            foreach (MediaJobInput jobInput in jobInputs)
            {
                assetIds.Add(jobInput.AssetId);
            }
            return assetIds.ToArray();
        }

        private static JObject GetProcessorConfig(MediaJobTask jobTask)
        {
            JObject processorConfig;
            string collectionId = Constant.Database.Collection.ProcessorConfig;
            string documentId = jobTask.ProcessorConfigId;
            if (string.IsNullOrEmpty(documentId))
            {
                documentId = string.Concat(jobTask.MediaProcessor.ToString(), Constant.Database.Document.DefaultIdSuffix);
            }
            using (DocumentClient documentClient = new DocumentClient())
            {
                processorConfig = documentClient.GetDocument(collectionId, documentId);
            }
            if (processorConfig != null)
            {
                processorConfig.Remove("PresetName");
                processorConfig.Remove("MediaProcessor");
                processorConfig.Remove("id");
                if (jobTask.ThumbnailGeneration != null)
                {
                    processorConfig = UpdateThumbnailGeneration(processorConfig, jobTask.ThumbnailGeneration);
                }
            }
            return processorConfig;
        }

        private static MediaJobTask GetJobTask(MediaClient mediaClient, MediaJobTask jobTask, string assetName)
        {
            MediaJobTask newJobTask = jobTask.DeepCopy();
            newJobTask.Name = Processor.GetProcessorName(newJobTask.MediaProcessor);
            if (string.IsNullOrEmpty(newJobTask.OutputAssetName))
            {
                string outputAssetName = Path.GetFileNameWithoutExtension(assetName);
                newJobTask.OutputAssetName = string.Concat(outputAssetName, " (", newJobTask.Name, ")");
            }
            newJobTask.OutputAssetEncryption = AssetCreationOptions.None;
            if (newJobTask.ContentProtection != null)
            {
                newJobTask.OutputAssetEncryption = AssetCreationOptions.StorageEncrypted;
            }
            return newJobTask;
        }

        private static MediaJobTask[] GetJobTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs, bool multipleInputTask)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (multipleInputTask)
            {
                jobTask = GetJobTask(mediaClient, jobTask, jobInputs[0].AssetName);
                jobTask.InputAssetIds = GetAssetIds(jobInputs);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (MediaJobInput jobInput in jobInputs)
                {
                    MediaJobTask jobInputTask = GetJobTask(mediaClient, jobTask, jobInput.AssetName);
                    jobInputTask.InputAssetIds = new string[] { jobInput.AssetId };
                    jobTasks.Add(jobInputTask);
                }
            }
            return jobTasks.ToArray();
        }

        private static JObject UpdateThumbnailGeneration(JObject processorConfig, ThumbnailGeneration thumbnailGeneration)
        {
            string format = thumbnailGeneration.Format.ToString();
            JToken[] codecs = processorConfig["Codecs"].ToArray();
            foreach (JToken codec in codecs)
            {
                if (codec["Type"].ToString().Contains("Image"))
                {
                    codec["Type"] = string.Concat(format, "Image");
                    if (thumbnailGeneration.Best)
                    {
                        codec["Start"] = "{Best}";
                    }
                    else
                    {
                        codec["Start"] = thumbnailGeneration.Start;
                        if (thumbnailGeneration.Single)
                        {
                            codec["Step"] = "1";
                            codec["Range"] = "1";
                        }
                        else
                        {
                            codec["Step"] = thumbnailGeneration.Step;
                            codec["Range"] = thumbnailGeneration.Range;
                        }
                    }
                    if (thumbnailGeneration.Columns.HasValue)
                    {
                        codec["SpriteColumn"] = thumbnailGeneration.Columns.Value;
                    }
                    JObject layer = new JObject();
                    layer["Type"] = string.Concat(format, "Layer");
                    layer["Width"] = thumbnailGeneration.Width;
                    layer["Height"] = thumbnailGeneration.Height;
                    if (format == "Jpg")
                    {
                        layer["Quality"] = 90;
                    }
                    JProperty pngLayers = codec["PngLayers"].Parent as JProperty;
                    pngLayers.Remove();
                    JArray layers = new JArray(layer);
                    codec[string.Concat(format, "Layers")] = layers;
                }
            }
            JToken[] outputs = processorConfig["Outputs"].ToArray();
            foreach (JToken output in outputs)
            {
                if (output["Format"]["Type"].ToString().Contains("Png"))
                {
                    output["Format"]["Type"] = string.Concat(format, "Format");
                }
            }
            return processorConfig;
        }
    }
}