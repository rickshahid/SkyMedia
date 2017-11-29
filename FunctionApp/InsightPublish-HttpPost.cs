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
    public static class InsightPublishHttpPost
    {
        private static MediaInsightPublish EnqueuePublish(string accountId, string indexId)
        {
            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.Table.InsightPublish;
            string partitionKey = accountId;
            string rowKey = indexId;
            MediaInsightPublish insightPublish = tableClient.GetEntity<MediaInsightPublish>(tableName, partitionKey, rowKey);
            if (insightPublish != null)
            {
                string settingKey = Constant.AppSettingKey.MediaPublishInsightQueue;
                string queueName = AppSetting.GetValue(settingKey);
                QueueClient queueClient = new QueueClient();
                queueClient.AddMessage(queueName, insightPublish);
            }
            return insightPublish;
        }

        [FunctionName("InsightPublish-HttpPost")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage req, TraceWriter log)
        {
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            IEnumerable<KeyValuePair<string, string>> urlParameters = req.GetQueryNameValuePairs();
            string accountId = urlParameters.SingleOrDefault(q => string.Equals(q.Key, "account", stringComparison)).Value;
            string indexId = urlParameters.SingleOrDefault(q => string.Equals(q.Key, "id", stringComparison)).Value;
            log.Info($"Account Id: {accountId}");
            log.Info($"Index Id: {indexId}");
            if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(indexId))
            {
                MediaInsightPublish insightPublish = EnqueuePublish(accountId, indexId);
                log.Info($"Insight Publish: {JsonConvert.SerializeObject(insightPublish)}");
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}