using System;
using System.IO;
using System.Web.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.EventGrid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaPublishHttpPost
    {
        [FunctionName("MediaPublish-HttpPost")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request, TraceWriter log)
        {
            object eventResponse = null;
            try
            {
                if (request.Query.ContainsKey("id"))
                {
                    log.Info($"Request Query: {request.QueryString}");
                    string indexId = request.Query["id"].ToString();
                    string indexState = request.Query["state"].ToString();
                    ProcessPublish(indexId, indexState, log);
                }
                else
                {
                    string requestBody;
                    using (StreamReader requestReader = new StreamReader(request.Body))
                    {
                        requestBody = requestReader.ReadToEnd();
                    }
                    log.Info($"Request Body: {requestBody}");
                    JToken eventInfo = JArray.Parse(requestBody)[0];
                    if (requestBody.Contains("validationCode"))
                    {
                        string validationCode = eventInfo["data"]["validationCode"].ToString();
                        log.Info($"Validation Code: {validationCode}");
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
                                    log.Info($"Job Name: {jobName}");
                                    ProcessPublish(jobName, null, log);
                                }
                                break;
                        }
                    }
                }
            }
            catch (HttpResponseException ex)
            {
                log.Info($"HTTP Exception: {ex.ToString()}");
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.ToString()}");
            }
            return new OkObjectResult(eventResponse);
        }

        private static void ProcessPublish(string documentId, string processState, TraceWriter log)
        {
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.ContentPublish;
                MediaPublish mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, documentId);
                if (mediaPublish != null)
                {
                    log.Info($"Media Publish: {JsonConvert.SerializeObject(mediaPublish)}");
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