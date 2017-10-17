using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static bool IsPremiumWorkflow(string fileName)
        {
            return fileName.EndsWith(Constant.Media.ProcessorConfig.EncoderPremiumWorkflowExtension, StringComparison.OrdinalIgnoreCase);
        }

        private static int OrderByWorkflow(MediaJobInput leftSide, MediaJobInput rightSide)
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

        private static MediaJobTask[] GetEncoderTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (jobTask.MediaProcessor != MediaProcessor.EncoderStandard)
            {
                List<MediaJobInput> inputList = new List<MediaJobInput>(jobInputs);
                inputList.Sort(OrderByWorkflow);
                jobInputs = inputList.ToArray();
            }
            else
            {
                if (string.Equals(jobTask.ProcessorConfig, Constant.Media.ProcessorConfig.EncoderStandardThumbnailsPreset, StringComparison.OrdinalIgnoreCase))
                {
                    string settingKey = Constant.AppSettingKey.MediaProcessorThumbnailGenerationDocumentId;
                    string documentId = AppSetting.GetValue(settingKey);
                    using (DocumentClient documentClient = new DocumentClient(false))
                    {
                        JObject processorConfig = documentClient.GetDocument(documentId);
                        jobTask.ProcessorConfig = processorConfig.ToString();
                    }
                }
                bool inputSubclipped = false;
                foreach (MediaJobInput jobInput in jobInputs)
                {
                    if (!string.IsNullOrEmpty(jobInput.MarkInTime))
                    {
                        inputSubclipped = true;
                    }
                }
                if (inputSubclipped && !jobTask.ProcessorConfig.StartsWith("{"))
                {
                    using (DocumentClient cosmosClient = new DocumentClient(true))
                    {
                        string collectionId = Constant.Database.Collection.ProcessorConfig;
                        string procedureId = Constant.Database.Procedure.ProcessorConfig;
                        JObject encoderConfig = cosmosClient.ExecuteProcedure(collectionId, procedureId, "name", jobTask.ProcessorConfig);
                        encoderConfig.Remove("name");
                        jobTask.ProcessorConfig = encoderConfig.ToString();
                    }
                }
            }
            JArray inputSources = new JArray();
            foreach (MediaJobInput jobInput in jobInputs)
            {
                if (!string.IsNullOrEmpty(jobInput.MarkInTime))
                {
                    JObject inputSource = new JObject
                    {
                        { "StartTime", jobInput.MarkInTime },
                        { "Duration", jobInput.MarkOutTime }
                    };
                    inputSources.Add(inputSource);
                }
            }
            if (inputSources.Count > 0)
            {
                JObject oldConfig = JObject.Parse(jobTask.ProcessorConfig);
                oldConfig.Remove("Sources");
                JObject newConfig = new JObject
                {
                    oldConfig.First,
                    { "Sources", inputSources }
                };
                oldConfig.First.Remove();
                JEnumerable<JToken> children = oldConfig.Children();
                foreach (JToken child in children)
                {
                    newConfig.Add(child);
                }
                jobTask.ProcessorConfig = newConfig.ToString();
            }
            bool multipleInputTask = jobTask.MediaProcessor != MediaProcessor.EncoderStandard;
            MediaJobTask[] tasks = SetJobTasks(mediaClient, jobTask, jobInputs, multipleInputTask);
            jobTasks.AddRange(tasks);
            return jobTasks.ToArray();
        }
    }
}