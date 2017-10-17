using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class publishController : Controller
    {
        [HttpPut]
        [Route("/publish/content")]
        public MediaPublish PublishContent(bool poisonQueue)
        {
            string settingKey = Constant.AppSettingKey.MediaPublishContentQueue;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue)
            {
                queueName = string.Concat(queueName, Constant.Storage.Queue.PoisonSuffix);
            }
            return MediaClient.PublishContent(queueName);
        }

        [HttpPut]
        [Route("/publish/insight")]
        public MediaPublish PublishInsight(bool poisonQueue)
        {
            string settingKey = Constant.AppSettingKey.MediaPublishInsightQueue;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue)
            {
                queueName = string.Concat(queueName, Constant.Storage.Queue.PoisonSuffix);
            }
            return MediaClient.PublishInsight(queueName);
        }

        [HttpDelete]
        [Route("/publish/purge")]
        public void PurgePublish()
        {
            TableClient tableClient = new TableClient();
            MediaClient.PurgePublishContent(tableClient);
            MediaClient.PurgePublishInsight(tableClient);
        }

        [HttpGet]
        [Route("/insight/get")]
        public JObject GetInsight(string indexId, string spokenLanguage, bool processingState)
        {
            JObject insight = null;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                IndexerClient indexerClient = new IndexerClient(authToken, null, null);
                insight = indexerClient.GetIndex(indexId, spokenLanguage, processingState);
            }
            return insight;
        }

        [HttpDelete]
        [Route("/insight/delete")]
        public void DeleteInsight(string indexId)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                IndexerClient indexerClient = new IndexerClient(authToken, null, null);
                indexerClient.DeleteVideo(indexId, true);
            }
        }
    }
}