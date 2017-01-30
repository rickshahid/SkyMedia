using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/processors")]
        public object GetMediaProcessors([FromBody] string accountName, [FromBody] string accountKey)
        {
            return Entities.GetMediaProcessors(accountName, accountKey);
        }

        [HttpPost]
        [Route("/publish")]
        public void PublishJob([FromBody] string jobNotification, [FromBody] bool webHook)
        {
            MediaClient.PublishJob(jobNotification, webHook);
        }

        [HttpPost]
        [Route("/message")]
        public void SendSMSText([FromBody] string messageText, [FromBody] string mobileNumber)
        {
            MessageClient.SendText(messageText, mobileNumber);
        }
    }
}
