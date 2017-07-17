using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class PublishJob
    {
        [FunctionName("PublishJob")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            string webMessage = await req.Content.ReadAsStringAsync();
            log.Info($"Web Message: {webMessage}");
            if (webMessage.StartsWith("{"))
            {
                MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(webMessage);
                if (jobNotification != null)
                {
                    MediaJobPublication jobPublication = MediaClient.PublishJob(jobNotification, true);
                    log.Info($"Job Publication: {jobPublication.StatusMessage}");
                }
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
