using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-PublishOutput")]
        public static void Run([EventGridTrigger] EventGridEvent eventTrigger, ILogger logger)
        {
            logger.LogInformation(eventTrigger.Data.ToString());
        }
    }
}
