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

        private MediaJobInput[] GetJobInputs(JObject clip)
        {
            List<MediaJobInput> jobInputs = new List<MediaJobInput>();
            JToken[] inputIds = clip["inputsIds"].ToArray();
            foreach (JToken inputId in inputIds)
            {
                MediaJobInput jobInput = new MediaJobInput()
                {
                    AssetId = inputId["id"].ToString(),
                    AssetFilter = string.Equals(inputId["type"], "filter")
                };
                jobInputs.Add(jobInput);
            }
            return jobInputs.ToArray();
        }

        public JsonResult clip(string clipData)
        {
            object clipOutput = null;
            JObject clip = JObject.Parse(clipData);
            MediaJobInput[] jobInputs = GetJobInputs(clip);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            if (jobInputs[0].AssetFilter)
            {
                string filterName = clip["name"].ToString();
                string assetId = jobInputs[0].AssetId;
                //clipOutput = MediaClient.CreateFilter()
            }
            else
            {
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
                string directoryId = homeController.GetDirectoryId(this.Request);
                mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs.ToArray());
                clipOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs.ToArray());
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