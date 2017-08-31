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
        [Route("/publish/insights")]
        public MediaPublish PublishInsights(bool poisonQueue)
        {
            string settingKey = Constant.AppSettingKey.MediaPublishInsightsQueue;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue)
            {
                queueName = string.Concat(queueName, Constant.Storage.Queue.PoisonSuffix);
            }
            return MediaClient.PublishInsights(queueName);
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