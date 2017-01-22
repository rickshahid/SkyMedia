using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.MediaServices.Client;

using AzureSkyMedia.Services;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/processors")]
        public IMediaProcessor[] GetMediaProcessors(string accountName, string accountKey)
        {
            MediaClient mediaClient = new MediaClient(accountName, accountKey);
            return mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
        }

        [HttpPost]
        [Route("/message")]
        public void SendSMSText(string messageText, string mobileNumber)
        {
            MessageClient.SendText(messageText, mobileNumber);
        }
    }
}
