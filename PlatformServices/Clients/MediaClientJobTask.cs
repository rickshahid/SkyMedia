﻿using System;
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

        private static JObject GetProcessorConfig(string documentId)
        {
            JObject processorConfig;
            using (DocumentClient documentClient = new DocumentClient())
            {
                processorConfig = documentClient.GetDocument(documentId);
            }
            return processorConfig;
        }

        private static MediaJobTask SetJobTask(MediaClient mediaClient, MediaJobTask jobTask, string assetName)
        {
            jobTask.Name = Processor.GetProcessorName(jobTask.MediaProcessor);
            if (string.IsNullOrEmpty(jobTask.OutputAssetName))
            {
                string outputAssetName = Path.GetFileNameWithoutExtension(assetName);
                jobTask.OutputAssetName = string.Concat(outputAssetName, " (", jobTask.Name, ")");
            }
            jobTask.OutputAssetEncryption = AssetCreationOptions.None;
            if (jobTask.ContentProtection != null)
            {
                jobTask.OutputAssetEncryption = AssetCreationOptions.StorageEncrypted;
            }
            return jobTask;
        }

        private static MediaJobTask[] SetJobTasks(MediaClient mediaClient, MediaJobTask jobTask, MediaJobInput[] jobInputs, bool multipleInputTask)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            if (multipleInputTask)
            {
                jobTask = SetJobTask(mediaClient, jobTask, jobInputs[0].AssetName);
                jobTask.InputAssetIds = GetAssetIds(jobInputs);
                jobTasks.Add(jobTask);
            }
            else
            {
                foreach (MediaJobInput jobInput in jobInputs)
                {
                    MediaJobTask newTask = jobTask.CreateCopy();
                    newTask = SetJobTask(mediaClient, newTask, jobInput.AssetName);
                    newTask.InputAssetIds = new string[] { jobInput.AssetId };
                    jobTasks.Add(newTask);
                }
            }
            return jobTasks.ToArray();
        }
    }
}