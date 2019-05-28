using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Storage.Blob;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-BlobList")]
        public static string[] ListBlobContainer([ActivityTrigger] DurableActivityContext context)
        {
            List<string> blobNames = new List<string>();
            CloudBlockBlob[] blobs = _blobClient.ListBlobContainer(_containerName, null);
            foreach (CloudBlockBlob blob in blobs)
            {
                if (!blob.Name.Equals(Constant.Storage.Blob.WorkflowManifestFile, _stringComparison))
                {
                    blobNames.Add(blob.Name);
                }
            }
            return blobNames.ToArray();
        }
    }
}