using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaJobStorageBlob
    {
        [FunctionName("MediaJob-StorageBlob")]
        public static void Run([BlobTrigger("ams/{name}")] Stream blob, string name, TraceWriter log)
        {
        //    bool createLog = true;
        //    BlobClient blobClient = new BlobClient();
        //    try
        //    {
        //        log.Info($"Media File: {name}");
        //        if (name.EndsWith(Constant.Media.FileExtension.JobManifest))
        //        {
        //            ParseJobManifest(blobClient, blob, name, log, createLog);
        //        }
        //        else
        //        {
        //            using (DatabaseClient databaseClient = new DatabaseClient())
        //            {
        //                string collectionId = Constant.Database.Collection.InputWorkflow;
        //                MediaProcess[] mediaProcesses = databaseClient.GetDocuments<MediaProcess>(collectionId);
        //                foreach (MediaProcess mediaProcess in mediaProcesses)
        //                {
        //                    List<string> missingFiles = new List<string>();
        //                    foreach (string missingFile in mediaProcess.MissingFiles)
        //                    {
        //                        if (!string.Equals(missingFile, name, StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            missingFiles.Add(name);
        //                        }
        //                    }
        //                    if (missingFiles.Count == 0)
        //                    {
        //                        databaseClient.DeleteDocument(collectionId, mediaProcess.Id);
        //                        blob = GetReadStream(blobClient, mediaProcess.Id);
        //                        ParseJobManifest(blobClient, blob, mediaProcess.Id, log, createLog);
        //                    }
        //                    else if (missingFiles.Count != mediaProcess.MissingFiles.Length)
        //                    {
        //                        mediaProcess.MissingFiles = missingFiles.ToArray();
        //                        databaseClient.UpsertDocument(collectionId, mediaProcess);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string logData = ex.ToString();
        //        UpdateJobLog(blobClient, name, logData, createLog);
        //        log.Info(logData);
        //    }
        }

        //private static bool FileExists(BlobClient blobClient, string blobName)
        //{
        //    string containerName = Constant.Storage.Blob.Container.MediaProcess;
        //    CloudBlockBlob blob = blobClient.GetBlockBlob(containerName, null, blobName);
        //    return blob.ExistsAsync().Result;
        //}

        //private static Stream GetReadStream(BlobClient blobClient, string blobName)
        //{
        //    string containerName = Constant.Storage.Blob.Container.MediaProcess;
        //    CloudBlockBlob blob = blobClient.GetBlockBlob(containerName, null, blobName);
        //    return blob.OpenReadAsync().Result;
        //}

        //private static CloudBlobStream GetWriteStream(BlobClient blobClient, string blobName, bool createNew)
        //{
        //    string containerName = Constant.Storage.Blob.Container.MediaProcess;
        //    CloudAppendBlob blob = blobClient.GetAppendBlob(containerName, null, blobName);
        //    return blob.OpenWriteAsync(createNew).Result;
        //}

        //private static void ParseJobManifest(BlobClient blobClient, Stream blob, string blobName, TraceWriter log, bool createLog)
        //{
        //    string[] assetIds = ParseJobManifest(blobClient, blob, out MediaClient mediaClient, out string[] assetFiles, out string[] missingFiles, out IDictionary<MediaProcessor, string> taskConfig);
        //    if (missingFiles.Length > 0)
        //    {
        //        MediaProcess mediaProcess = new MediaProcess()
        //        {
        //            Id = blobName,
        //            MissingFiles = missingFiles
        //        };
        //        using (DatabaseClient databaseClient = new DatabaseClient())
        //        {
        //            string collectionId = Constant.Database.Collection.InputWorkflow;
        //            databaseClient.UpsertDocument(collectionId, mediaProcess);
        //        }
        //    }
        //    else
        //    {
        //        if (assetFiles.Length > 0)
        //        {
        //            //string assetName = assetFiles[0];
        //            //string assetId = mediaClient.CreateAsset(assetName, assetFiles);
        //            //createLog = UpdateJobLog(blobClient, blobName, assetId, createLog);
        //            //log.Info($"Asset Id: {assetId}");
        //            //assetIds = new string[] { assetId };
        //        }
        //        string jobName = Path.GetFileNameWithoutExtension(blobName);
        //        //MediaJob mediaJob = GetMediaJob(jobName, taskConfig);
        //        //MediaJobInput[] jobInputs = GetJobInputs(mediaClient, assetIds);
        //        //string jobId = Workflow.SubmitJob(mediaClient, mediaJob, jobInputs);
        //        //createLog = UpdateJobLog(blobClient, blobName, jobId, createLog);
        //        //log.Info($"Job Id: {jobId}");
        //        //foreach (KeyValuePair<MediaProcessor, string> processorConfig in taskConfig)
        //        //{
        //        //    createLog = UpdateJobLog(blobClient, blobName, processorConfig.Key.ToString(), createLog);
        //        //    log.Info($"Job Task: {processorConfig.Key}");
        //        //}
        //    }
        //}

        //private static string[] ParseJobManifest(BlobClient blobClient, Stream blob, out MediaClient mediaClient, out string[] assetFiles,
        //                                         out string[] missingFiles, out IDictionary<MediaProcessor, string> taskConfig)
        //{
        //    List<string> assetIds = new List<string>();
        //    List<string> assetFileNames = new List<string>();
        //    List<string> missingFileNames = new List<string>();
        //    StreamReader jobReader = new StreamReader(blob);
        //    MediaAccount mediaAccount = new MediaAccount()
        //    {
        //        Name = jobReader.ReadLine(),
        //        SubscriptionId = jobReader.ReadLine(),
        //        ResourceGroupName = jobReader.ReadLine(),
        //        DirectoryTenantId = jobReader.ReadLine(),
        //        ClientApplicationId = jobReader.ReadLine(),
        //        ClientApplicationKey = jobReader.ReadLine()
        //    };
        //    mediaClient = new MediaClient(null, mediaAccount);
        //    taskConfig = new Dictionary<MediaProcessor, string>();
        //    while (!jobReader.EndOfStream)
        //    {
        //        string jobLine = jobReader.ReadLine();
        //        if (jobLine.StartsWith(Constant.Media.IdPrefix.Asset))
        //        {
        //            assetIds.Add(jobLine);
        //        }
        //        else if (!FileExists(blobClient, jobLine))
        //        {
        //            missingFileNames.Add(jobLine);
        //        }
        //        else if (jobLine.EndsWith(Constant.Media.FileExtension.Json))
        //        {
        //            string[] processorConfig = Path.GetFileNameWithoutExtension(jobLine).Split(Constant.TextDelimiter.File);
        //            MediaProcessor mediaProcessor = (MediaProcessor)Enum.Parse(typeof(MediaProcessor), processorConfig[0]);
        //            Stream configStream = GetReadStream(blobClient, jobLine);
        //            using (StreamReader configReader = new StreamReader(configStream))
        //            {
        //                taskConfig.Add(mediaProcessor, configReader.ReadToEnd());
        //            }
        //        }
        //        else
        //        {
        //            assetFileNames.Add(jobLine);
        //        }
        //    }
        //    assetFiles = assetFileNames.ToArray();
        //    missingFiles = missingFileNames.ToArray();
        //    return assetIds.ToArray();
        //}

        //private static bool UpdateJobLog(BlobClient blobClient, string manifestName, string logData, bool createLog)
        //{
        //    string logName = manifestName.Replace(Constant.Media.FileExtension.JobManifest, Constant.Media.FileExtension.JobLog);
        //    CloudBlobStream logStream = GetWriteStream(blobClient, logName, createLog);
        //    using (StreamWriter logWriter = new StreamWriter(logStream))
        //    {
        //        logWriter.WriteLine(logData);
        //    }
        //    return false;
        //}

        //private static MediaJob GetMediaJob(string jobName, IDictionary<MediaProcessor, string> taskConfig)
        //{
        //    List<MediaJobTask> jobTasks = new List<MediaJobTask>();
        //    foreach (KeyValuePair<MediaProcessor, string> processorConfig in taskConfig)
        //    {
        //        MediaJobTask jobTask = new MediaJobTask()
        //        {
        //            MediaProcessor = processorConfig.Key,
        //            ProcessorConfig = processorConfig.Value
        //        };
        //        jobTasks.Add(jobTask);
        //    }
        //    MediaJob mediaJob = new MediaJob()
        //    {
        //        Name = jobName,
        //        Tasks = jobTasks.ToArray()
        //    };
        //    return mediaJob;
        //}

        //private static MediaJobInput[] GetJobInputs(MediaClient mediaClient, string[] assetIds)
        //{
        //    List<MediaJobInput> jobInputs = new List<MediaJobInput>();
        //    foreach (string assetId in assetIds)
        //    {
        //        MediaJobInput jobInput = MediaClient.GetJobInput(mediaClient, assetId);
        //        jobInputs.Add(jobInput);
        //    }
        //    return jobInputs.ToArray();
        //}
    }
}