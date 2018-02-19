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
                    SetThumbnailGeneration(processorConfig, jobTask.ThumbnailGeneration);
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

        private static void SetThumbnailLayers(JToken codec, ThumbnailGeneration thumbnailGeneration)
        {
            JProperty pngLayers = codec["PngLayers"].Parent as JProperty;
            pngLayers.Remove();
            JObject layer = new JObject();
            string format = thumbnailGeneration.Format.ToString();
            layer["Type"] = string.Concat(format, "Layer");
            layer["Width"] = thumbnailGeneration.Width;
            layer["Height"] = thumbnailGeneration.Height;
            if (format == "Jpg")
            {
                layer["Quality"] = 90;
            }
            string layers = string.Concat(format, "Layers");
            codec[layers] = new JArray(layer);
        }

        private static void SetThumbnailSprite(JArray codecs, JToken thumbnailsCodec, JToken spriteCodec, JArray outputs, ThumbnailGeneration thumbnailGeneration)
        {
            if (thumbnailsCodec == null)
            {
                spriteCodec["SpriteColumn"] = thumbnailGeneration.Columns;
                spriteCodec["JpgLayers"][0]["Quality"] = 90;
            }
            else
            {
                string format = thumbnailGeneration.Format.ToString();
                string spriteJson = spriteCodec.ToString();
                spriteJson = spriteJson.Replace(format, "Jpg");
                spriteCodec = JObject.Parse(spriteJson);
                spriteCodec["SpriteColumn"] = thumbnailGeneration.Columns;
                spriteCodec["JpgLayers"][0]["Quality"] = 90;
                codecs.Add(spriteCodec);
                JObject outputFormat = new JObject();
                outputFormat["Type"] = "JpgFormat";
                JObject output = new JObject();
                output["FileName"] = "{Basename}_{Index}{Extension}";
                output["Format"] = outputFormat;
                outputs.Add(output);
            }
        }

        private static void SetThumbnailGeneration(JObject processorConfig, ThumbnailGeneration thumbnailGeneration)
        {
            JToken thumbnailsCodec = null;
            JToken spriteCodec = null;
            bool spriteOnly = false;
            if (thumbnailGeneration.Format == MediaThumbnailFormat.Sprite)
            {
                thumbnailGeneration.Format = MediaThumbnailFormat.Jpg;
                spriteOnly = true;
            }
            string format = thumbnailGeneration.Format.ToString();
            JArray codecs = (JArray)processorConfig["Codecs"];
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
                    SetThumbnailLayers(codec, thumbnailGeneration);
                    if (spriteOnly)
                    {
                        spriteCodec = codec;
                    }
                    else
                    {
                        thumbnailsCodec = codec;
                        spriteCodec = codec.DeepClone();
                    }
                }
            }
            JArray outputs = (JArray)processorConfig["Outputs"];
            foreach (JToken output in outputs)
            {
                if (output["Format"]["Type"].ToString().Contains("Png"))
                {
                    output["Format"]["Type"] = string.Concat(format, "Format");
                }
            }
            if (thumbnailGeneration.Sprite)
            {
                SetThumbnailSprite(codecs, thumbnailsCodec, spriteCodec, outputs, thumbnailGeneration);
            }
        }
    }
}