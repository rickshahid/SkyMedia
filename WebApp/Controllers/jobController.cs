using System;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class jobController : Controller
    {
        [HttpPost]
        [Route("/publish")]
        public JobPublication Publish(string jobMessage, bool poisonQueue)
        {
            JobPublication jobPublication = new JobPublication();
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
                jobPublication.ErrorMessage = ex.ToString();
            }
            return jobPublication;
        }
    }
}
