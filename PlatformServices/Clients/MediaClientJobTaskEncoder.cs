using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static bool IsPremiumWorkflow(string fileName)
        {
            return fileName.EndsWith(Constant.Media.ProcessorConfig.EncoderPremiumWorkflowExtension, StringComparison.OrdinalIgnoreCase);
        }

        private static int OrderByWorkflow(MediaAssetInput leftSide, MediaAssetInput rightSide)
        {
            int comparison = 0;
            if (IsPremiumWorkflow(leftSide.PrimaryFile))
            {
                comparison = -1;
            }
            else if (IsPremiumWorkflow(rightSide.PrimaryFile))
            {
                comparison = 1;
            }
            return comparison;
        }

        private static MediaJobTask[] GetEncoderTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (jobTask.ProcessorType != MediaProcessor.EncoderStandard)
            {
                List<MediaAssetInput> assets = new List<MediaAssetInput>(inputAssets);
                assets.Sort(OrderByWorkflow);
                inputAssets = assets.ToArray();
            }
            else
            {
                if (string.Equals(jobTask.ProcessorConfig, Constant.Media.ProcessorConfig.EncoderStandardThumbnailsPreset, StringComparison.OrdinalIgnoreCase))
                {
                    string settingKey = Constant.AppSettingKey.MediaProcessorThumbnailGenerationDocumentId;
                    string documentId = AppSetting.GetValue(settingKey);
                    using (CosmosClient cosmosClient = new CosmosClient(false))
                    {
                        JObject processorConfig = cosmosClient.GetDocument(documentId);
                        jobTask.ProcessorConfig = processorConfig.ToString();
                    }
                }
                bool inputSubclipped = false;
                foreach (MediaAssetInput inputAsset in inputAssets)
                {
                    if (!string.IsNullOrEmpty(inputAsset.MarkInTime))
                    {
                        inputSubclipped = true;
                    }
                }
                if (inputSubclipped && !jobTask.ProcessorConfig.StartsWith("{"))
                {
                    using (CosmosClient cosmosClient = new CosmosClient(true))
                    {
                        string collectionId = Constant.Database.Collection.Encoding;
                        string procedureId = Constant.Database.Procedure.EncoderConfig;
                        JObject encoderConfig = cosmosClient.ExecuteProcedure(collectionId, procedureId, "name", jobTask.ProcessorConfig);
                        jobTask.ProcessorConfig = encoderConfig.ToString();
                    }
                }
            }
            JArray inputSources = new JArray();
            foreach (MediaAssetInput inputAsset in inputAssets)
            {
                if (!string.IsNullOrEmpty(inputAsset.MarkInTime))
                {
                    JObject inputSource = new JObject();
                    inputSource.Add("StartTime", inputAsset.MarkInTime);
                    inputSource.Add("Duration", inputAsset.MarkOutTime);
                    inputSources.Add(inputSource);
                }
            }
            if (inputSources.Count > 0)
            {
                JObject oldConfig = JObject.Parse(jobTask.ProcessorConfig);
                oldConfig.Remove("Sources");
                JObject newConfig = new JObject();
                newConfig.Add(oldConfig.First);
                newConfig.Add("Sources", inputSources);
                oldConfig.First.Remove();
                JEnumerable<JToken> children = oldConfig.Children();
                foreach (JToken child in children)
                {
                    newConfig.Add(child);
                }
                jobTask.ProcessorConfig = newConfig.ToString();
            }
            bool multipleInputTask = jobTask.ProcessorType != MediaProcessor.EncoderStandard;
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, multipleInputTask);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }
    }
}
