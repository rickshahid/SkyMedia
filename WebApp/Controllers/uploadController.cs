using System.IO;
using System.Xml;
using System.Net.Http;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class UploadController : Controller
    {
        private void UploadIngestManifest(StorageBlobClient blobClient, MediaIngestManifest ingestManifest)
        {
            string ingestManifestJson = JsonConvert.SerializeObject(ingestManifest);
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string assetName = Path.GetFileNameWithoutExtension(ingestManifest.JobInputFileUrl);
            string fileName = string.Concat(Constant.Media.IngestManifest.TriggerPrefix, Constant.TextDelimiter.Manifest, assetName, Constant.Media.IngestManifest.FileExtension);
            CloudBlockBlob manifest = blobClient.GetBlockBlob(containerName, fileName);
            manifest.Properties.ContentType = Constant.Media.ContentType.IngestManifest;
            manifest.UploadTextAsync(ingestManifestJson).Wait();
        }

        private void UploadIngestManifests(MediaIngestManifest ingestManifest, string rssUrl, int videoCount)
        {
            XmlDocument rssDocument;
            StorageBlobClient blobClient = new StorageBlobClient();
            using (WebClient webClient = new WebClient(null))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, rssUrl);
                rssDocument = webClient.GetResponse<XmlDocument>(webRequest);
            }
            XmlElement channel = rssDocument.DocumentElement["channel"];
            XmlNodeList videos = channel.SelectNodes("item");
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(rssDocument.NameTable);
            namespaceManager.AddNamespace(Constant.Media.Channel9.NamespacePrefix, Constant.Media.Channel9.NamespaceUrl);
            for (int i = 0; i < videoCount; i++)
            {
                XmlNode video = videos[i];
                XmlNode videoContent = video.SelectSingleNode(Constant.Media.Channel9.XPathQuery, namespaceManager);
                string videoUrl = videoContent.Attributes["url"].Value;
                string videoDescription = video.SelectSingleNode("title").InnerText;
                videoUrl = videoUrl.Replace(Constant.Media.Channel9.UrlHttp, Constant.Media.Channel9.UrlHttps);
                videoUrl = videoUrl.Replace(Constant.Media.Channel9.Http, Constant.Media.Channel9.Https);
                ingestManifest.JobInputFileUrl = videoUrl;
                ingestManifest.JobOutputAssetDescriptions = new string[] { videoDescription };
                UploadIngestManifest(blobClient, ingestManifest);
            }
        }

        [HttpPost]
        [Route("/syndicate")]
        public void Syndicate(string rssUrl = "https://channel9.msdn.com/Shows/AI-Show/feed/mp4high", int videoCount = 3)
        {
            StorageBlobClient blobClient = new StorageBlobClient();
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string fileName = string.Concat(Constant.Media.IngestManifest.TriggerPrefix, Constant.Media.IngestManifest.FileExtension);
            CloudBlockBlob manifestFile = blobClient.GetBlockBlob(containerName, fileName);
            if (manifestFile.ExistsAsync().Result)
            {
                MediaIngestManifest ingestManifest;
                using (Stream manifestStream = manifestFile.OpenReadAsync().Result)
                {
                    StreamReader manifestReader = new StreamReader(manifestStream);
                    string manifestData = manifestReader.ReadToEnd();
                    ingestManifest = JsonConvert.DeserializeObject<MediaIngestManifest>(manifestData);
                }
                using (MediaClient mediaClient = new MediaClient(null, ingestManifest.MediaAccount))
                {
                    Account.DeleteEntities(mediaClient, false);
                }
                UploadIngestManifests(ingestManifest, rssUrl, videoCount);
            }
        }

        public JsonResult Block(string name, int chunk, int chunks, string storageAccount, string contentType)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            User userProfile = new User(authToken);
            StorageBlobClient blobClient = new StorageBlobClient(userProfile.MediaAccount, storageAccount);
            Stream blockStream = Request.Form.Files[0].OpenReadStream();
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            blobClient.UploadBlock(blockStream, containerName, name, chunk, chunks, contentType);
            return Json(chunk);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            Dictionary<string, string> storageAccounts = Account.GetStorageAccounts(authToken);
            ViewData["storageAccount"] = HomeController.GetListItems(storageAccounts);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["indexerEnabled"] = mediaClient.IndexerEnabled();
            }
            return View();
        }
    }
}