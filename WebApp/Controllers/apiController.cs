using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/processors")]
        public object GetMediaProcessors(string accountName, string accountKey)
        {
            return Entities.GetMediaProcessors(accountName, accountKey);
        }

        [HttpPost]
        [Route("/message")]
        public void SendSMSText(string messageText, string mobileNumber)
        {
            MessageClient.SendText(messageText, mobileNumber);
        }
    }
}
