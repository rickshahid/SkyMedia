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
            BlobClient blobClient = new BlobClient();
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string fileName = string.Concat(Constant.Media.IngestManifest.GalleryPrefix, Constant.Media.IngestManifest.TriggerPrefix, Constant.Media.IngestManifest.FileExtension);
            CloudBlockBlob manifestFile = blobClient.GetBlockBlob(containerName, fileName);
            Stream manifestStream = manifestFile.OpenReadAsync().Result;
            StreamReader manifestReader = new StreamReader(manifestStream);
            string ingestManifestData = manifestReader.ReadToEnd();
            MediaIngestManifest ingestManifest = JsonConvert.DeserializeObject<MediaIngestManifest>(ingestManifestData);
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