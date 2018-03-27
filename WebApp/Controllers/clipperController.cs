using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class clipperController : Controller
    {
        private MediaJobInput[] GetJobInputs(JObject clip)
        {
            List<MediaJobInput> jobInputs = new List<MediaJobInput>();
            JToken[] inputIds = clip["inputsIds"].ToArray();
            foreach (JToken inputId in inputIds)
            {
                MediaJobInput jobInput = new MediaJobInput()
                {
                    AssetId = inputId["id"].ToString(),
                    AssetType = inputId["type"].ToString()
                };
                jobInputs.Add(jobInput);
            }
            return jobInputs.ToArray();
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
            JObject clip = JObject.Parse(clipData);
            MediaJobInput[] jobInputs = GetJobInputs(clip);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            if (string.Equals(jobInputs[0].AssetType, "filter", StringComparison.OrdinalIgnoreCase))
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
                    Tasks = new MediaJobTask[] { jobTask }
                };
                string directoryId = homeController.GetDirectoryId(this.Request);
                mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs.ToArray());
                clipOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs.ToArray());
            }
            return Json(clipOutput);
        }

        public IActionResult index()
        {
            return View();
        }
    }
}