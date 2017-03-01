using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private static bool PremiumWorkflow(string fileName)
        {
            return fileName.EndsWith(Constants.Media.ProcessorConfig.EncoderPremiumWorkflowExtension, StringComparison.InvariantCulture);
        }

        private static int OrderByWorkflow(MediaAssetInput leftSide, MediaAssetInput rightSide)
        {
            int comparison = 0;
            if (PremiumWorkflow(leftSide.PrimaryFile))
            {
                comparison = -1;
            }
            else if (PremiumWorkflow(rightSide.PrimaryFile))
            {
                comparison = 1;
            }
            return comparison;
        }

        private static MediaJobTask[] GetEncoderTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaAssetInput[] inputAssets)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (jobTask.MediaProcessor != MediaProcessor.EncoderStandard)
            {
                List<MediaAssetInput> assets = new List<MediaAssetInput>(inputAssets);
                assets.Sort(OrderByWorkflow);
                inputAssets = assets.ToArray();
            }
            else
            {
                if (string.Equals(jobTask.ProcessorConfig, Constants.Media.ProcessorConfig.EncoderStandardThumbnailsPreset, StringComparison.InvariantCultureIgnoreCase))
                {
                    string settingKey = Constants.AppSettingKey.MediaProcessorThumbnailGenerationDocumentId;
                    string documentId = AppSetting.GetValue(settingKey);
                    using (DatabaseClient databaseClient = new DatabaseClient(false))
                    {
                        JObject processorConfig = databaseClient.GetDocument(documentId);
                        jobTask.ProcessorConfig = processorConfig.ToString();
                    }
                }
                bool inputSubclipped = false;
                foreach (MediaAssetInput inputAsset in inputAssets)
                {
                    if (!string.IsNullOrEmpty(inputAsset.MarkIn))
                    {
                        inputSubclipped = true;
                    }
                }
                if (inputSubclipped && !jobTask.ProcessorConfig.StartsWith("{"))
                {
                    using (DatabaseClient databaseClient = new DatabaseClient(true))
                    {
                        string collectionId = Constants.Database.DocumentCollection.Encoding;
                        string procedureId = "getEncoderConfig";
                        JObject encoderConfig = databaseClient.ExecuteProcedure(collectionId, procedureId, "name", jobTask.ProcessorConfig);
                        jobTask.ProcessorConfig = encoderConfig.ToString();
                    }
                }
            }
            JArray inputSources = new JArray();
            foreach (MediaAssetInput inputAsset in inputAssets)
            {
                if (!string.IsNullOrEmpty(inputAsset.MarkIn))
                {
                    JObject inputSource = new JObject();
                    inputSource.Add("StartTime", inputAsset.MarkIn);
                    inputSource.Add("Duration", inputAsset.ClipDuration);
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
            bool multipleInputTask = jobTask.MediaProcessor != MediaProcessor.EncoderStandard;
            MediaJobTask[] mappedJobTasks = MapJobTasks(mediaClient, jobTask, inputAssets, multipleInputTask);
            jobTasks.AddRange(mappedJobTasks);
            return jobTasks.ToArray();
        }
    }
}
