using System;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static bool IsPremiumWorkflow(MediaJobInput jobInput)
        {
            return jobInput.PrimaryFile.EndsWith(Constant.Media.FileExtension.Workflow, StringComparison.OrdinalIgnoreCase);
        }

        private static int OrderByWorkflow(MediaJobInput leftItem, MediaJobInput rightIten)
        {
            int comparison = 0;
            if (IsPremiumWorkflow(leftItem))
            {
                comparison = -1;
            }
            else if (IsPremiumWorkflow(rightIten))
            {
                comparison = 1;
            }
            return comparison;
        }

        private static MediaJobTask[] GetEncoderTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs)
        {
            if (jobTask.MediaProcessor == MediaProcessor.EncoderPremium)
            {
                Array.Sort(jobInputs, OrderByWorkflow);
            }
            JObject processorConfig = GetProcessorConfig(jobTask);
            jobTask.ProcessorConfig = processorConfig == null ? jobTask.ProcessorConfigId : processorConfig.ToString();
            bool multipleInputTask = jobTask.MediaProcessor != MediaProcessor.EncoderStandard;
            return GetJobTasks(mediaClient, jobTask, jobInputs, multipleInputTask);
        }

        private static ITask[] GetEncoderTasks(IJob job)
        {
            string processorId1 = Constant.Media.ProcessorId.EncoderStandard;
            string processorId2 = Constant.Media.ProcessorId.EncoderPremium;
            string[] processorIds = new string[] { processorId1, processorId2 };
            return GetJobTasks(job, processorIds);
        }
    }
}