using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class InsightPublishHttpPost
    {
        [FunctionName("InsightPublish-HttpPost")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request, TraceWriter log)
        {
            string accountId = request.Query["account"];
            string indexId = request.Query["id"];
            log.Info($"Account Id: {accountId}");
            log.Info($"Index Id: {indexId}");
            if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(indexId))
            {
                MediaPublish insightPublish = EnqueuePublish(accountId, indexId);
                log.Info($"Insight Publish: {JsonConvert.SerializeObject(insightPublish)}");
            }
            return new OkResult();
        }

        private static MediaPublish EnqueuePublish(string accountId, string indexId)
        {
            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.Table.InsightPublish;
            string partitionKey = accountId;
            string rowKey = indexId;
            MediaPublish insightPublish = tableClient.GetEntity<MediaPublish>(tableName, partitionKey, rowKey);
            if (insightPublish != null)
            {
                string settingKey = Constant.AppSettingKey.MediaPublishInsightQueue;
                string queueName = AppSetting.GetValue(settingKey);
                QueueClient queueClient = new QueueClient();
                queueClient.AddMessage(queueName, insightPublish);
            }
            return insightPublish;
        }
    }
}