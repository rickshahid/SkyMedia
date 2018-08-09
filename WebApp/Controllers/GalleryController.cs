using System.IO;
using System.Xml;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;

using AzureSkyMedia.PlatformServices;

using Newtonsoft.Json;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class GalleryController : Controller
    {
        private static void UploadIngestManifest(BlobClient blobClient, MediaIngestManifest ingestManifest)
        {
            string ingestManifestJson = JsonConvert.SerializeObject(ingestManifest);
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string assetName = Path.GetFileNameWithoutExtension(ingestManifest.JobInputFileUrl);
            string fileName = string.Concat(Constant.Media.IngestManifest.TriggerPrefix, Constant.TextDelimiter.File, assetName, Constant.Media.IngestManifest.FileExtension);
            CloudBlockBlob manifest = blobClient.GetBlockBlob(containerName, fileName);
            manifest.Properties.ContentType = Constant.Media.ContentType.IngestManifest;
            manifest.UploadTextAsync(ingestManifestJson).Wait();
        }

        private static void UploadIngestManifests(MediaIngestManifest ingestManifest, string rssUrl)
        {
            XmlDocument rssDocument;
            BlobClient blobClient = new BlobClient();
            using (WebClient webClient = new WebClient(null))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, rssUrl);
                rssDocument = webClient.GetResponse<XmlDocument>(webRequest);
            }
            XmlElement channel = rssDocument.DocumentElement["channel"];
            XmlNodeList videos = channel.SelectNodes("item");
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(rssDocument.NameTable);
            namespaceManager.AddNamespace(Constant.Media.Channel9.NamespacePrefix, Constant.Media.Channel9.NamespaceUrl);
            for (int i = 0; i < Constant.Media.Channel9.IngestVideoCount; i++)
            {
                XmlNode video = videos[i];
                XmlNode videoContent = video.SelectSingleNode(Constant.Media.Channel9.XPathQuery, namespaceManager);
                string videoUrl = videoContent.Attributes["url"].Value;
                string videoDescription = video.SelectSingleNode("title").InnerText;
                videoUrl = videoUrl.Replace(Constant.Media.Channel9.UrlHttp, Constant.Media.Channel9.UrlHttps);
                videoUrl = videoUrl.Replace(Constant.Media.Channel9.Http, Constant.Media.Channel9.Https);
                ingestManifest.JobInputFileUrl = videoUrl;
                ingestManifest.JobOutputAssetDescription = videoDescription;
                UploadIngestManifest(blobClient, ingestManifest);
            }
        }

        [HttpPost]
        [Route("/gallery/refresh")]
        public void Refresh(string rssUrl = "https://channel9.msdn.com/Shows/AI-Show/feed/mp4high", bool skipDelete = false)
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
                    Account.DeleteEntities(mediaClient, false);
                }
            }
            UploadIngestManifests(ingestManifest, rssUrl);
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