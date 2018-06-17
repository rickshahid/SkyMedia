using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public Job CreateJob(string transformName, string jobName, JobInput jobInput, string[] outputAssetNames)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                jobName = Guid.NewGuid().ToString();
            }
            List<JobOutputAsset> outputAssets = new List<JobOutputAsset>();
            foreach (string outputAssetName in outputAssetNames)
            {
                JobOutputAsset outputAsset = new JobOutputAsset(outputAssetName);
                outputAssets.Add(outputAsset);
            }
            Job job = new Job()
            {
                Input = jobInput,
                Outputs = outputAssets.ToArray()
            };
            Task<AzureOperationResponse<Job>> createTask = _media.Jobs.CreateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, jobName, job);
            AzureOperationResponse<Job> createResponse = createTask.Result;
            return createResponse.Body;
        }

        //private void SetProcessorUnits(IJob job, ReservedUnitType nodeType, bool newJob)
        //{
        //    int unitCount = job.Tasks.Count;
        //    IEncodingReservedUnit[] processorUnits = GetEntities(MediaEntity.ProcessorUnit) as IEncodingReservedUnit[];
        //    IEncodingReservedUnit processorUnit = processorUnits[0];
        //    processorUnit.ReservedUnitType = (ReservedUnitType)nodeType;
        //    if (newJob)
        //    {
        //        processorUnit.CurrentReservedUnits += unitCount;
        //    }
        //    else if (processorUnit.CurrentReservedUnits >= unitCount)
        //    {
        //        processorUnit.CurrentReservedUnits -= unitCount;
        //    }
        //    processorUnit.Update();
        //}

        //public static MediaJob GetJob(string authToken, MediaClient mediaClient, MediaJob mediaJob, MediaJobInput[] jobInputs)
        //{
        //    List<MediaJobTask> jobTasks = new List<MediaJobTask>();
        //    foreach (MediaJobTask jobTask in mediaJob.Tasks)
        //    {
        //        MediaJobTask[] tasks = null;
        //        switch (jobTask.MediaProcessor)
        //        {
        //            case MediaProcessor.EncoderStandard:
        //            case MediaProcessor.EncoderPremium:
        //                //tasks = GetEncoderTasks(mediaClient, jobTask, jobInputs);
        //                break;
        //            //case MediaProcessor.AudioAnalyzer:
        //            //    tasks = GetAudioAnalyzerTasks(mediaClient, jobTask, jobInputs);
        //            //    break;
        //            //case MediaProcessor.VideoAnalyzer:
        //            //    MediaJobTask encoderTask = GetEncoderTask(mediaJob);
        //            //    if (encoderTask == null)
        //            //    {
        //            //        VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
        //            //        foreach (MediaJobInput jobInput in jobInputs)
        //            //        {
        //            //            IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, jobInput.AssetId) as IAsset;
        //            //            string indexId = VideoAnalyzer.GetIndexId(asset);
        //            //            if (!string.IsNullOrEmpty(indexId))
        //            //            {
        //            //                videoAnalyzer.RestartAnalysis(mediaClient, indexId);
        //            //            }
        //            //            else
        //            //            {
        //            //                videoAnalyzer.StartAnalysis(mediaClient, asset, jobTask.InsightConfig);
        //            //            }
        //            //        }
        //            //    }
        //            //    else
        //            //    {
        //            //        encoderTask.InsightConfig = jobTask.InsightConfig;
        //            //    }
        //            //    break;
        //            //case MediaProcessor.VideoSummarization:
        //            //    tasks = GetVideoSummarizationTasks(mediaClient, jobTask, jobInputs);
        //            //    break;
        //            //case MediaProcessor.FaceDetection:
        //            //    tasks = GetFaceDetectionTasks(mediaClient, jobTask, jobInputs);
        //            //    break;
        //            //case MediaProcessor.MotionDetection:
        //            //    tasks = GetMotionDetectionTasks(mediaClient, jobTask, jobInputs);
        //            //    break;
        //        }
        //        if (tasks != null)
        //        {
        //            jobTasks.AddRange(tasks);
        //        }
        //    }
        //    mediaJob.Tasks = jobTasks.ToArray();
        //    if (string.IsNullOrEmpty(mediaJob.Name))
        //    {
        //        mediaJob.Name = jobInputs.Length > 1 ? Constant.Media.Job.MultipleInputAssets : jobInputs[0].AssetName;
        //    }
        //    return mediaJob;
        //}

        //public static MediaJobInput GetJobInput(MediaClient mediaClient, string assetId)
        //{
        //    IAsset asset = mediaClient.GetEntity(MediaEntity.Asset, assetId) as IAsset;
        //    return asset == null ? new MediaJobInput() : GetJobInput(asset);
        //}

        //public static MediaJobInput GetJobInput(IAsset asset)
        //{
        //    MediaJobInput jobInput = new MediaJobInput()
        //    {
        //        AssetId = asset.Id,
        //        AssetName = asset.Name,
        //        AssetType = asset.AssetType.ToString()
        //    };
        //    return jobInput;
        //}

        //public IJob CreateJob(MediaJob mediaJob, MediaJobInput[] jobInputs)
        //{
        //    IJob job = _media2.Jobs.Create(mediaJob.Name, mediaJob.Priority);
        //    //foreach (MediaJobTask jobTask in mediaJob.Tasks)
        //    //{
        //    //    string processorId = Processor.GetProcessorId(jobTask.MediaProcessor, jobTask.ProcessorConfig);
        //    //    IMediaProcessor processor = GetEntityById(MediaEntity.Processor, processorId) as IMediaProcessor;
        //    //    ITask currentTask = job.Tasks.AddNew(jobTask.Name, processor, jobTask.ProcessorConfig, jobTask.Options);
        //    //    if (jobTask.ParentIndex.HasValue)
        //    //    {
        //    //        ITask parentTask = job.Tasks[jobTask.ParentIndex.Value];
        //    //        currentTask.InputAssets.AddRange(parentTask.OutputAssets);
        //    //    }
        //    //    else
        //    //    {
        //    //        IAsset[] assets = GetAssets(jobTask.InputAssetIds);
        //    //        currentTask.InputAssets.AddRange(assets);
        //    //    }
        //    //    currentTask.OutputAssets.AddNew(jobTask.OutputAssetName, jobTask.OutputAssetEncryption, jobTask.OutputAssetFormat);
        //    //}
        //    //INotificationEndPoint notificationEndpoint = GetNotificationEndpoint();
        //    //job.JobNotificationSubscriptions.AddNew(NotificationJobState.FinalStatesOnly, notificationEndpoint);
        //    //SetProcessorUnits(job, ReservedUnitType.Premium, true);
        //    //job.Submit();
        //    return job;
        //}
    }
}