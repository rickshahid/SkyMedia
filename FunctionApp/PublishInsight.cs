using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class PublishInsights
    {
        private static MediaInsightsPublish EnqueuePublish(string accountId, string indexId)
        {
            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.InsightPublish;
            string partitionKey = accountId;
            string rowKey = indexId;
            MediaInsightsPublish insightsPublish = entityClient.GetEntity<MediaInsightsPublish>(tableName, partitionKey, rowKey);
            if (insightsPublish != null)
            {
                string settingKey = Constant.AppSettingKey.MediaPublishInsightsQueue;
                string queueName = AppSetting.GetValue(settingKey);
                MessageClient messageClient = new MessageClient();
                messageClient.AddMessage(queueName, insightsPublish);
            }
            return insightsPublish;
        }

        [FunctionName("PublishInsights")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req, TraceWriter log)
        {
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            IEnumerable<KeyValuePair<string, string>> urlParameters = req.GetQueryNameValuePairs();
            string accountId = urlParameters.SingleOrDefault(q => string.Equals(q.Key, "account", stringComparison)).Value;
            string indexId = urlParameters.SingleOrDefault(q => string.Equals(q.Key, "id", stringComparison)).Value;
            log.Info($"Account Id: {accountId}");
            log.Info($"Index Id: {indexId}");
            if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(indexId))
            {
                MediaInsightsPublish insightsPublish = EnqueuePublish(accountId, indexId);
                log.Info($"Insights Publish: {JsonConvert.SerializeObject(insightsPublish)}");
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}