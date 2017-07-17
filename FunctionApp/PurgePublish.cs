using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class PurgePublish
    {
        [FunctionName("PurgePublish")]
        public static void Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            MediaClient.PurgePublish();
        }
    }
}
