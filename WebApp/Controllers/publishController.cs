using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

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
    }
}