using System;
using System.Linq;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-CreateJob")]
        public static Job CreateJob([ActivityTrigger] DurableActivityContext context)
        {
            ValueTuple<MediaWorkflowManifest, string, Asset> jobInput = context.GetInput<(MediaWorkflowManifest, string, Asset)>();
            MediaWorkflowManifest workflowManifest = jobInput.Item1;
            string inputFileUrl = jobInput.Item2;
            Asset inputAsset = jobInput.Item3;

            Job job = null;
            string insightId = null;
            MediaClient mediaClient = new MediaClient(workflowManifest);

            bool videoIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoIndexer);
            bool audioIndexer = workflowManifest.TransformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioIndexer);
            if (mediaClient.IndexerEnabled() && (videoIndexer || audioIndexer))
            {
                insightId = mediaClient.IndexerUploadVideo(inputFileUrl, inputAsset, workflowManifest.JobPriority, videoIndexer, audioIndexer);
            }

            Transform transform = mediaClient.GetTransform(workflowManifest.TransformPresets);
            if (transform != null)
            {
                MediaJobOutputInsight outputInsight = new MediaJobOutputInsight()
                {
                    Id = insightId,
                    VideoIndexer = videoIndexer,
                    AudioIndexer = audioIndexer
                };
                job = mediaClient.CreateJob(transform.Name, workflowManifest.JobName, null, workflowManifest.JobPriority, inputFileUrl, inputAsset, workflowManifest.OutputAssetStorage, workflowManifest.JobOutputPublish, outputInsight, false);
            }
            return job;
        }
    }
}