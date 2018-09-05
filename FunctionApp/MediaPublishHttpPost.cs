using System;
using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPublishHttpPost
    {
        [FunctionName("MediaPublishHttpPost")]
        public static IActionResult Run([HttpTrigger("post")] HttpRequest request, ILogger logger)
        {
            SubscriptionValidationResponse validationResponse = null;
            if (request.Query.ContainsKey("id"))
            {
                logger.LogInformation("Request Query: {0}", request.QueryString);
                string indexId = request.Query["id"].ToString();
                string indexState = request.Query["state"].ToString();
                PublishJobOutput(indexId, indexState, logger);
            }
            else
            {
                StreamReader requestReader = new StreamReader(request.Body);
                string requestBody = requestReader.ReadToEnd();
                logger.LogInformation("Request Body: {0}", requestBody);
                JToken eventInfo = JArray.Parse(requestBody)[0];
                if (requestBody.Contains("validationCode"))
                {
                    string validationCode = eventInfo["data"]["validationCode"].ToString();
                    logger.LogInformation("Validation Code: {0}", validationCode);
                    validationResponse = new SubscriptionValidationResponse(validationCode);
                }
                else
                {
                    switch (eventInfo["eventType"].ToString())
                    {
                        case "Microsoft.Media.JobStateChange":
                            string eventSubject = eventInfo["subject"].ToString();
                            string eventState = eventInfo["data"]["state"].ToString();
                            if (string.Equals(eventState, "Finished", StringComparison.OrdinalIgnoreCase))
                            {
                                string jobName = Path.GetFileName(eventSubject);
                                logger.LogInformation("Job Name: {0}", jobName);
                                PublishJobOutput(jobName, null, logger);
                            }
                            break;
                    }
                }
            }
            return new OkObjectResult(validationResponse);
        }

        private static void PublishJobOutput(string documentId, string processState, ILogger logger)
        {
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.MediaPublish;
                MediaPublish mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, documentId);
                if (mediaPublish != null)
                {
                    mediaPublish.ProcessState = processState;
                    mediaPublish = MediaClient.PublishJobOutput(mediaPublish);
                    logger.LogInformation("Media Publish: {0}", JsonConvert.SerializeObject(mediaPublish));
                }
            }
        }
    }
}