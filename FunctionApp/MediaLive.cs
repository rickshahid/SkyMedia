using System;
using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid.Models;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.FunctionApp
{
//    public static class MediaLive
//    {
//        [FunctionName("MediaLive")]
//        public static IActionResult Run([HttpTrigger("post")] HttpRequest request, ILogger logger)
//        {
//            SubscriptionValidationResponse validationResponse = null;
//            try
//            {
//                StreamReader requestReader = new StreamReader(request.Body);
//                string requestBody = requestReader.ReadToEnd();
//                if (requestBody.Contains("validationCode"))
//                {
//                    JToken eventInfo = JArray.Parse(requestBody)[0];
//                    string validationCode = eventInfo["data"]["validationCode"].ToString();
//                    validationResponse = new SubscriptionValidationResponse(validationCode);
//                }
//                else
//                {
//                    logger.LogInformation(requestBody);
//                }
//            }
//            catch (Exception ex)
//            {
//                string logData = ex.ToString();
//                logger.LogError(logData);
//                throw;
//            }
//            return new OkObjectResult(validationResponse);
//        }
//    }
}