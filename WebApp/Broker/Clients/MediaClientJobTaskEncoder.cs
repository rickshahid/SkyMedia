using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private static bool PremiumWorkflow(string fileName)
        {
            return fileName.EndsWith(Constants.Media.Job.PremiumWorkflowSuffix, StringComparison.InvariantCulture);
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
            if (jobTask.MediaProcessor == MediaProcessor.EncoderPremium)
            {
                jobTask.Name = Selections.GetProcessorName(MediaProcessor.EncoderPremium);
                List<MediaAssetInput> assets = new List<MediaAssetInput>(inputAssets);
                assets.Sort(OrderByWorkflow);
                inputAssets = assets.ToArray();
            }
            else
            {
                jobTask.Name = Selections.GetProcessorName(MediaProcessor.EncoderStandard);
            }
            string processorConfig = jobTask.ProcessorConfig;
            if (processorConfig != null)
            {
                processorConfig = processorConfig.Trim();
                if (processorConfig.StartsWith("{"))
                {
                    JArray inputSources = new JArray();
                    foreach (MediaAssetInput inputAsset in inputAssets)
                    {
                        if (!string.IsNullOrEmpty(inputAsset.MarkIn))
                        {
                            JObject inputSource = new JObject();
                            inputSource.Add("StartTime", inputAsset.MarkIn);
                            inputSource.Add("Duration", inputAsset.MarkOut);
                            inputSources.Add(inputSource);
                        }
                    }
                    if (inputSources.Count > 0)
                    {
                        JObject oldConfig = JObject.Parse(processorConfig);
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
                        processorConfig = newConfig.ToString();
                    }
                }
            }
            jobTask = SetJobTask(mediaClient, jobTask, processorConfig, inputAssets);
            jobTasks.Add(jobTask);
            return jobTasks.ToArray();
        }
    }
}
