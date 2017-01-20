using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.MediaServices.Client;

using AzureSkyMedia.ServiceBroker;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/processors")]
        public IMediaProcessor[] GetMediaProcessors(string accountName, string accountKey)
        {
            string[] accountCredentials = new string[] { accountName, accountKey };
            MediaClient mediaClient = new MediaClient(accountCredentials);
            return mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
        }
    }
}
