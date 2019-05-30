using Microsoft.Azure.WebJobs;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-BlobUrl")]
        public static string GetBlobUrl([ActivityTrigger] string fileName)
        {
            return _blobClient.GetDownloadUrl(_containerName, fileName);
        }
    }
}