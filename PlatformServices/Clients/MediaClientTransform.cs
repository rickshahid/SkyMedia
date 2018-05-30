using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public Transform CreateTransform(string transformName, EncoderNamedPreset encoderPreset)
        {
            BuiltInStandardEncoderPreset transformPreset = new BuiltInStandardEncoderPreset(encoderPreset);
            TransformOutput transformOutput = new TransformOutput(transformPreset);
            List<TransformOutput> transformOutputs = new List<TransformOutput>() { transformOutput };
            Task<AzureOperationResponse<Transform>> createTask = _media.Transforms.CreateOrUpdateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputs);
            AzureOperationResponse<Transform> createResponse = createTask.Result;
            return createResponse.Body;
        }

        //private static ITask[] GetJobTasks(IJob job, string[] processorIds)
        //{
        //    List<ITask> jobTasks = new List<ITask>();
        //    foreach (ITask jobTask in job.Tasks)
        //    {
        //        if (processorIds.Contains(jobTask.MediaProcessorId, StringComparer.OrdinalIgnoreCase))
        //        {
        //            jobTasks.Add(jobTask);
        //        }
        //    }
        //    return jobTasks.ToArray();
        //}

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
            string collectionId = Constant.Database.Collection.ProcessorPreset;
            string documentId = jobTask.ProcessorConfigId;
            if (string.IsNullOrEmpty(documentId))
            {
                documentId = string.Concat(jobTask.MediaProcessor.ToString(), Constant.Database.Document.DefaultIdSuffix);
            }
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                processorConfig = databaseClient.GetDocument(collectionId, documentId);
            }
            if (processorConfig != null)
            {
                processorConfig.Remove("PresetName");
                processorConfig.Remove("MediaProcessor");
                processorConfig.Remove("id");
                if (jobTask.ThumbnailGeneration != null)
                {
                    SetThumbnailGeneration(jobTask.ThumbnailGeneration, processorConfig);
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
            //newJobTask.OutputAssetEncryption = AssetCreationOptions.None;
            //if (newJobTask.ContentProtection != null)
            //{
            //    newJobTask.OutputAssetEncryption = AssetCreationOptions.StorageEncrypted;
            //}
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

        private static void SetThumbnailLayers(ThumbnailGeneration thumbnailGeneration, JToken thumbnailCodec)
        {
            JProperty pngLayers = (JProperty)thumbnailCodec["PngLayers"].Parent;
            pngLayers.Remove();
            JObject layer = new JObject();
            string format = thumbnailGeneration.Format.ToString();
            layer["Type"] = string.Concat(format, "Layer");
            layer["Width"] = thumbnailGeneration.Width;
            layer["Height"] = thumbnailGeneration.Height;
            if (thumbnailGeneration.Format == MediaImageFormat.JPG)
            {
                layer["Quality"] = Constant.Media.ProcessorPreset.ThumbnailJpgQuality;
            }
            string layers = string.Concat(format, "Layers");
            thumbnailCodec[layers] = new JArray(layer);
        }

        private static void SetThumbnailSprite(ThumbnailGeneration thumbnailGeneration, JToken spriteCodec, JArray codecs, JArray outputs)
        {
            if (thumbnailGeneration.Format != MediaImageFormat.JPG)
            {
                string format = thumbnailGeneration.Format.ToString();
                string layers = string.Concat(format, "Layers");
                JToken jpgLayers = spriteCodec[layers].DeepClone();
                JProperty oldLayers = (JProperty)spriteCodec[layers].Parent;
                oldLayers.Remove();
                spriteCodec["Type"] = "JpgImage";
                jpgLayers[0]["Type"] = "JpgLayer";
                jpgLayers[0]["Quality"] = Constant.Media.ProcessorPreset.ThumbnailJpgQuality;
                spriteCodec["JpgLayers"] = jpgLayers;
            }
            spriteCodec["SpriteColumn"] = thumbnailGeneration.Columns;
            if (codecs != null)
            {
                codecs.Add(spriteCodec);
                JObject outputFormat = new JObject();
                outputFormat["Type"] = "JpgFormat";
                JObject output = new JObject();
                output["FileName"] = "{Basename}_{Index}{Extension}";
                output["Format"] = outputFormat;
                outputs.Add(output);
            }
        }

        private static void SetThumbnailGeneration(ThumbnailGeneration thumbnailGeneration, JObject processorConfig)
        {
            JToken spriteCodec = null;
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
                        codec["Step"] = thumbnailGeneration.Single ? "1" : thumbnailGeneration.Step;
                        codec["Range"] = thumbnailGeneration.Single ? "1" : thumbnailGeneration.Range;
                    }
                    SetThumbnailLayers(thumbnailGeneration, codec);
                    spriteCodec = thumbnailGeneration.Sprite ? codec : codec.DeepClone();
                }
            }
            if (thumbnailGeneration.Sprite)
            {
                codecs = null;
            }
            JArray outputs = (JArray)processorConfig["Outputs"];
            if (thumbnailGeneration.Format != MediaImageFormat.PNG)
            {
                foreach (JToken output in outputs)
                {
                    if (output["Format"]["Type"].ToString().Contains("Png"))
                    {
                        output["Format"]["Type"] = string.Concat(format, "Format");
                    }
                }
            }
            if (thumbnailGeneration.Sprite)
            {
                SetThumbnailSprite(thumbnailGeneration, spriteCodec, codecs, outputs);
            }
        }
    }
}