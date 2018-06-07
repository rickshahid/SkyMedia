using System;
using System.IO;

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
                string eventMessage = new StreamReader(request.Body).ReadToEnd();
                log.Info($"Event Message: {eventMessage}");
                JToken eventInfo = JArray.Parse(eventMessage)[0];
                if (eventMessage.Contains("validationCode"))
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
                            switch (eventInfo["data"]["state"].ToString())
                            {
                                case "Finished":
                                    MediaPublish mediaPublish = EnqueuePublish(eventInfo, log);
                                    log.Info($"Media Publish: {JsonConvert.SerializeObject(mediaPublish)}");
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.ToString()}");
            }
            return new OkObjectResult(eventResponse);
        }

        private static MediaPublish EnqueuePublish(JToken eventInfo, TraceWriter log)
        {
            MediaPublish mediaPublish;
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string eventSubject = eventInfo["subject"].ToString();
                string collectionId = Constant.Database.Collection.OutputPublish;
                string jobName = Path.GetFileName(eventSubject);
                log.Info($"Job Name: {jobName}");
                mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, jobName);
                if (mediaPublish != null)
                {
                    string settingKey = Constant.AppSettingKey.MediaPublishQueue;
                    string queueName = AppSetting.GetValue(settingKey);
                    QueueClient queueClient = new QueueClient();
                    queueClient.AddMessage(queueName, mediaPublish);
                }
            }
            return mediaPublish;
        }
    }
}