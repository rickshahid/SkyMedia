using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentProcessStorageBlob
    {
        [FunctionName("ContentProcess-StorageBlob")]
        public static void Run([BlobTrigger("ams/{name}")] Stream blob, string name, TraceWriter log)
        {
            log.Info($"Content Process: {name}");
            if (name.EndsWith(Constant.Media.FileExtension.ManifestAsset))
            {
                StreamReader streamReader = new StreamReader(blob);

                MediaAccount mediaAccount = new MediaAccount()
                {
                    DomainName = streamReader.ReadLine(),
                    EndpointUrl = streamReader.ReadLine(),
                    ClientId = streamReader.ReadLine(),
                    ClientKey = streamReader.ReadLine()
                };

                string processorConfig = string.Empty;
                BlobClient blobClient = new BlobClient();
                string containerName = Constant.Storage.Blob.Container.ContentProcess;
                List<string> fileNames = new List<string>();
                while (!streamReader.EndOfStream)
                {
                    string fileName = streamReader.ReadLine();
                    if (fileName.EndsWith(Constant.Media.FileExtension.Json))
                    {
                        CloudBlockBlob configFile = blobClient.GetBlob(containerName, null, fileName);
                        Stream configStream = configFile.OpenRead();
                        using (StreamReader configReader = new StreamReader(configStream))
                        {
                            processorConfig = configReader.ReadToEnd();
                        }
                    }
                    else
                    {
                        fileNames.Add(fileName);
                    }
                }

                MediaClient mediaClient = new MediaClient(mediaAccount);
                string assetName = Path.GetFileNameWithoutExtension(name);
                string assetId = mediaClient.CreateAsset(assetName, fileNames.ToArray());
                log.Info($"Asset Id: {assetId}");

                if (!string.IsNullOrEmpty(processorConfig))
                {
                    MediaJobTask jobTask = new MediaJobTask()
                    {
                        MediaProcessor = MediaProcessor.EncoderStandard,
                        ProcessorConfig = processorConfig
                    };

                    MediaJob mediaJob = new MediaJob()
                    {
                        Name = assetName,
                        Tasks = new MediaJobTask[] { jobTask }
                    };

                    MediaJobInput jobInput = new MediaJobInput()
                    {
                        AssetId = assetId
                    };
                    MediaJobInput[] jobInputs = new MediaJobInput[] { jobInput };

                    string jobId = Workflow.SubmitJob(mediaClient, mediaJob, jobInputs);
                    log.Info($"Job Id: {jobId}");
                }
            }
        }
    }
}