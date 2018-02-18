using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        [HttpPut]
        [Route("/asset/publish")]
        public MediaPublished publish(bool? insightQueue, bool? poisonQueue)
        {
            if (!insightQueue.HasValue) insightQueue = false;
            if (!poisonQueue.HasValue) poisonQueue = false;
            string settingKey = insightQueue.Value ? Constant.AppSettingKey.MediaPublishInsightQueue : Constant.AppSettingKey.MediaPublishContentQueue;
            string queueName = AppSetting.GetValue(settingKey);
            if (poisonQueue.Value)
            {
                queueName = string.Concat(queueName, Constant.Storage.Queue.PoisonSuffix);
            }
            return insightQueue.Value ? MediaClient.PublishInsight(queueName) : MediaClient.PublishContent(queueName);
        }

        public JsonResult assets(string assetId, bool getFiles)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = mediaClient.GetAssets(authToken, assetId, getFiles);
            return Json(assets);
        }

        public JsonResult streams(string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaStream[] streams = Media.GetMediaStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
            return Json(streams);
        }

        public JsonResult clip(string clipData)
        {
            object clipOutput = null;
            string directoryId = homeController.GetDirectoryId(this.Request);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            JObject clip = JObject.Parse(clipData);
            switch (clip["type"].ToString())
            {
                case "asset":
                    List<MediaJobInput> jobInputs = new List<MediaJobInput>();
                    JToken[] inputIds = clip["inputsIds"].ToArray();
                    foreach (JToken inputId in inputIds)
                    {
                        MediaJobInput jobInput = new MediaJobInput()
                        {
                            AssetId = inputId["id"].ToString()
                        };
                        jobInputs.Add(jobInput);
                    }
                    MediaJobTask jobTask = new MediaJobTask()
                    {
                        MediaProcessor = MediaProcessor.EncoderStandard,
                        ProcessorConfig = clip["output"]["job"].ToString()
                    };
                    MediaJob mediaJob = new MediaJob()
                    {
                        Name = clip["name"].ToString(),
                        NodeType = MediaJobNodeType.Premium,
                        Tasks = new MediaJobTask[] { jobTask }
                    };
                    mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs.ToArray());
                    clipOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs.ToArray());
                    break;

                case "filter":
                    //clipOutput = MediaClient.CreateFilter();
                    break;
            }
            return Json(clipOutput);
        }

        public IActionResult clipper()
        {
            return View();
        }

        public IActionResult index()
        {
            return View();
        }
    }
}