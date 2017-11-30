using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentProcessStorageBlob
    {
        [FunctionName("ContentProcess-StorageBlob")]
        public static void Run([BlobTrigger("ams/{name}")] Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string containerName = Constant.Storage.Blob.Container.AzureMediaServices;
            string accountFileName = Constant.Media.AccountFileName;

            BlobClient blobClient = new BlobClient();
            CloudBlockBlob accountBlob = blobClient.GetBlob(containerName, null, accountFileName);
            if (accountBlob.Exists())
            {
            }
        }
    }
}