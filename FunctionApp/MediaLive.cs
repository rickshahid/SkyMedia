using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaLive
    {
        [FunctionName("MediaLive")]
        public static void Run([EventGridTrigger] EventGridEvent eventTrigger, ILogger logger)
        {
            logger.LogInformation(JsonConvert.SerializeObject(eventTrigger, Formatting.Indented));
        }
    }
}