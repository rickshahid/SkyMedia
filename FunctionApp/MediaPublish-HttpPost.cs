using System;
using System.IO;
using System.Web.Http;

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
        [FunctionName("MediaPublish-HttpPost")]
        public static IActionResult Run([HttpTrigger("post")] HttpRequest request, ILogger logger)
        {
            object eventResponse = null;
            try
            {
                if (request.Query.ContainsKey("id"))
                {
                    logger.LogInformation("Request Query: {0}", request.QueryString);
                    string indexId = request.Query["id"].ToString();
                    string indexState = request.Query["state"].ToString();
                    ProcessPublish(indexId, indexState, logger);
                }
                else
                {
                    string requestBody;
                    using (StreamReader requestReader = new StreamReader(request.Body))
                    {
                        requestBody = requestReader.ReadToEnd();
                    }
                    logger.LogInformation("Request Body: {0}", requestBody);
                    JToken eventInfo = JArray.Parse(requestBody)[0];
                    if (requestBody.Contains("validationCode"))
                    {
                        string validationCode = eventInfo["data"]["validationCode"].ToString();
                        logger.LogInformation("Validation Code: {0}", validationCode);
                        eventResponse = new SubscriptionValidationResponse(validationCode);
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
                                    ProcessPublish(jobName, null, logger);
                                }
                                break;
                        }
                    }
                }
            }
            catch (HttpResponseException ex)
            {
                logger.LogError(ex, "HTTP Exception: {0}", ex.Response.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception: {0}", ex.ToString());
            }
            return new OkObjectResult(eventResponse);
        }

        private static void ProcessPublish(string documentId, string processState, ILogger logger)
        {
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.ContentPublish;
                MediaPublish mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, documentId);
                if (mediaPublish != null)
                {
                    logger.LogInformation("Media Publish: {0}", JsonConvert.SerializeObject(mediaPublish));
                    if (!string.IsNullOrEmpty(processState))
                    {
                        MediaClient.SendNotificationMessage(mediaPublish, null, documentId, processState);
                    }
                    else
                    {
                        string settingKey = Constant.AppSettingKey.MediaPublishQueue;
                        string queueName = AppSetting.GetValue(settingKey);
                        QueueClient queueClient = new QueueClient();
                        queueClient.AddMessage(queueName, mediaPublish);
                    }
                }
            }
        }
    }
}