using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaProcessStorageBlob
    {
        [FunctionName("MediaProcess-StorageBlob")]
        public static void Run([BlobTrigger("ams/{name}")] Stream blob, string name, TraceWriter log)
        {
            bool createNewLog = true;
            try
            {
                log.Info($"Media File: {name}");
                if (name.EndsWith(Constant.Media.FileExtension.JobManifest))
                {
                    StreamReader jobReader = new StreamReader(blob);

                    MediaAccount mediaAccount = new MediaAccount()
                    {
                        DomainName = jobReader.ReadLine(),
                        EndpointUrl = jobReader.ReadLine(),
                        ClientId = jobReader.ReadLine(),
                        ClientKey = jobReader.ReadLine(),
                        IndexerKey = jobReader.ReadLine()
                    };
                    MediaClient mediaClient = new MediaClient(mediaAccount);

                    string[] assetIds = ParseJobManifest(jobReader, out string[] assetFiles, out IDictionary<MediaProcessor, string> taskConfig);
                    string assetName = Path.GetFileNameWithoutExtension(name);
                    if (assetIds.Length == 0)
                    {
                        string assetId = mediaClient.CreateAsset(assetName, assetFiles);
                        assetIds = new string[] { assetId };
                        log.Info($"Asset Id: {assetId}");
                        createNewLog = UpdateJobLog(name, assetId, createNewLog);
                    }

                    MediaJob mediaJob = GetMediaJob(assetName, taskConfig);
                    MediaJobInput[] jobInputs = GetJobInputs(mediaClient, assetIds);

                    string jobId = Workflow.SubmitJob(mediaClient, mediaJob, jobInputs);
                    log.Info($"Job Id: {jobId}");
                    createNewLog = UpdateJobLog(name, jobId, createNewLog);
                }
            }
            catch (Exception ex)
            {
                string logData = ex.ToString();
                UpdateJobLog(name, logData, createNewLog);
                log.Info(logData);
            }
        }

        private static Stream GetReadStream(string blobName)
        {
            BlobClient blobClient = new BlobClient();
            string containerName = Constant.Storage.Blob.Container.MediaProcess;
            CloudBlockBlob blob = blobClient.GetBlockBlob(containerName, null, blobName);
            return blob.OpenRead();
        }

        private static CloudBlobStream GetWriteStream(string blobName, bool createNew)
        {
            BlobClient blobClient = new BlobClient();
            string containerName = Constant.Storage.Blob.Container.MediaProcess;
            CloudAppendBlob blob = blobClient.GetAppendBlob(containerName, null, blobName);
            return blob.OpenWrite(createNew);
        }

        private static string[] ParseJobManifest(StreamReader jobReader, out string[] assetFiles, out IDictionary<MediaProcessor, string> taskConfig)
        {
            List<string> assetIds = new List<string>();
            List<string> fileNames = new List<string>();
            taskConfig = new Dictionary<MediaProcessor, string>();
            while (!jobReader.EndOfStream)
            {
                string jobLine = jobReader.ReadLine();
                if (jobLine.StartsWith(Constant.Media.AssetIdPrefix))
                {
                    assetIds.Add(jobLine);
                }
                else if (jobLine.EndsWith(Constant.Media.FileExtension.Json))
                {
                    string[] processorConfig = Path.GetFileNameWithoutExtension(jobLine).Split(Constant.TextDelimiter.File);
                    MediaProcessor mediaProcessor = (MediaProcessor)Enum.Parse(typeof(MediaProcessor), processorConfig[0]);
                    Stream configStream = GetReadStream(jobLine);
                    using (StreamReader configReader = new StreamReader(configStream))
                    {
                        taskConfig.Add(mediaProcessor, configReader.ReadToEnd());
                    }
                }
                else
                {
                    fileNames.Add(jobLine);
                }
            }
            assetFiles = fileNames.ToArray();
            return assetIds.ToArray();
        }

        private static bool UpdateJobLog(string manifestName, string logData, bool createNew)
        {
            string logName = manifestName.Replace(Constant.Media.FileExtension.JobManifest, Constant.Media.FileExtension.JobLog);
            CloudBlobStream logStream = GetWriteStream(logName, createNew);
            using (StreamWriter logWriter = new StreamWriter(logStream))
            {
                logWriter.WriteLine(logData);
            }
            return false;
        }

        private static MediaJob GetMediaJob(string jobName, IDictionary<MediaProcessor, string> taskConfig)
        {
            List<MediaJobTask> jobTasks = new List<MediaJobTask>();
            foreach (KeyValuePair<MediaProcessor, string> processorConfig in taskConfig)
            {
                MediaJobTask jobTask = new MediaJobTask()
                {
                    MediaProcessor = processorConfig.Key,
                    ProcessorConfig = processorConfig.Value
                };
                jobTasks.Add(jobTask);
            }
            MediaJob mediaJob = new MediaJob()
            {
                Name = jobName,
                Tasks = jobTasks.ToArray()
            };
            return mediaJob;
        }

        private static MediaJobInput[] GetJobInputs(MediaClient mediaClient, string[] assetIds)
        {
            List<MediaJobInput> jobInputs = new List<MediaJobInput>();
            foreach (string assetId in assetIds)
            {
                MediaJobInput jobInput = MediaClient.GetJobInput(mediaClient, assetId);
                jobInputs.Add(jobInput);
            }
            return jobInputs.ToArray();
        }
    }
}