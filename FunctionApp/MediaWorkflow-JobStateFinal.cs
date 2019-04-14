using System;
using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaWorkflowJobStateFinal
    {
        [FunctionName("MediaWorkflow-JobStateFinal")]
        public static void Run([EventGridTrigger] EventGridEvent eventTrigger, ILogger logger)
        {
            try
            {
                logger.LogInformation(JsonConvert.SerializeObject(eventTrigger, Formatting.Indented));
                switch (eventTrigger.EventType)
                {
                    case "Microsoft.Media.JobFinished":
                        JObject eventData = JObject.FromObject(eventTrigger.Data);
                        JObject jobData = (JObject)eventData["correlationData"];
                        if (jobData.ContainsKey("mediaAccount"))
                        {
                            string mediaAccountJson = jobData["mediaAccount"].ToString();
                            MediaAccount mediaAccount = JsonConvert.DeserializeObject<MediaAccount>(mediaAccountJson);
                            string transformName = eventTrigger.Subject.Split("/")[1];
                            string jobName = Path.GetFileName(eventTrigger.Subject);
                            StreamingLocator streamingLocator = MediaClient.PublishJobOutput(mediaAccount, transformName, jobName);
                            if (streamingLocator != null)
                            {
                                logger.LogInformation(JsonConvert.SerializeObject(streamingLocator, Formatting.Indented));
                            }
                        }
                        break;

                    case "Microsoft.Media.JobCanceled":
                        break;

                    case "Microsoft.Media.JobErrored":
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
    }
}