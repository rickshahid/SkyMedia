using System;

using Newtonsoft.Json.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static bool IsPremiumWorkflow(MediaJobInput jobInput)
        {
            return jobInput.PrimaryFile.EndsWith(Constant.Media.ProcessorConfig.EncoderPremiumWorkflowExtension, StringComparison.OrdinalIgnoreCase);
        }

        private static int OrderByWorkflow(MediaJobInput leftSide, MediaJobInput rightSide)
        {
            int comparison = 0;
            if (IsPremiumWorkflow(leftSide))
            {
                comparison = -1;
            }
            else if (IsPremiumWorkflow(rightSide))
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
            else if (string.Equals(jobTask.ProcessorConfig, Constant.Media.ProcessorConfig.EncoderStandardThumbnailsPreset, StringComparison.OrdinalIgnoreCase))
            {
                string collectionId = Constant.Database.Collection.ProcessorConfig;
                string documentId = Processor.GetProcessorName(MediaProcessor.EncoderStandard);
                documentId = string.Concat(documentId, Constant.Database.Document.ThumbnailsIdSuffix);
                using (DocumentClient documentClient = new DocumentClient())
                {
                    JObject processorConfig = documentClient.GetDocument(collectionId, documentId);
                    jobTask.ProcessorConfig = processorConfig.ToString();
                }
            }
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