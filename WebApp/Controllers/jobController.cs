using System;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class jobController : Controller
    {
        public JsonResult template(string templateId)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            object jobTemplate = mediaClient.GetEntityById(MediaEntity.JobTemplate, templateId);
            return Json(jobTemplate);
        }

        [HttpPost]
        [Route("/publish")]
        public JobPublication Publish(string jobMessage, bool poisonQueue)
        {
            JobPublication jobPublication = null;
            try
            {
                if (string.IsNullOrEmpty(jobMessage))
                {
                    jobPublication = MediaClient.PublishJob(poisonQueue);
                }
                else
                {
                    MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(jobMessage);
                    if (jobNotification != null)
                    {
                        jobPublication = MediaClient.PublishJob(jobNotification, false);
                    }
                }
            }
            catch (Exception ex)
            {
                if (jobPublication == null)
                {
                    jobPublication = new JobPublication();
                }
                jobPublication.StatusMessage = ex.ToString();
            }
            return jobPublication;
        }

        [HttpPost]
        [Route("/purge")]
        public void Purge()
        {
            MediaClient.PurgePublish();
        }
    }
}
