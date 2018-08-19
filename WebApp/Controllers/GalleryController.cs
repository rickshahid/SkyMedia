using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class GalleryController : Controller
    {
        [HttpPost]
        [Route("/gallery/refresh")]
        public void RefreshContent(string rssUrl = "https://channel9.msdn.com/Shows/AI-Show/feed/mp4high", bool skipDelete = false)
        {
            MediaIngestManifest ingestManifest;
            BlobClient blobClient = new BlobClient();
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string fileName = string.Concat(Constant.Media.IngestManifest.GalleryPrefix, Constant.Media.IngestManifest.TriggerPrefix, Constant.Media.IngestManifest.FileExtension);
            CloudBlockBlob manifestFile = blobClient.GetBlockBlob(containerName, fileName);
            using (Stream manifestStream = manifestFile.OpenReadAsync().Result)
            {
                StreamReader manifestReader = new StreamReader(manifestStream);
                string manifestData = manifestReader.ReadToEnd();
                ingestManifest = JsonConvert.DeserializeObject<MediaIngestManifest>(manifestData);
            }
            if (!skipDelete)
            {
                using (MediaClient mediaClient = new MediaClient(null, ingestManifest.MediaAccount))
                {
                    Account.DeleteEntities(mediaClient, true);
                }
            }
            Media.UploadIngestManifests(ingestManifest, rssUrl);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["mediaStreams"] = Media.GetAccountStreams(authToken, mediaClient);
            }
            return View();
        }
    }
}