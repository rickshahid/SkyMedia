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
        private void UploadManifest(StorageBlobClient blobClient, MediaWorkflowManifest workflowManifest)
        {
            //string ingestManifestJson = JsonConvert.SerializeObject(workflowManifest);
            //string containerName = Constant.Storage.BlobContainer.MediaServices;
            //string assetName = Path.GetFileNameWithoutExtension(workflowManifest.JobInputFileUrl);
            //string fileName = string.Concat(Constant.Media.IngestManifest.TriggerPrefix, Constant.TextDelimiter.Manifest, assetName, Constant.Media.IngestManifest.FileExtension);
            //CloudBlockBlob manifest = blobClient.GetBlockBlob(containerName, null, fileName);
            //manifest.Properties.ContentType = Constant.Media.ContentType.IngestManifest;
            //manifest.UploadTextAsync(ingestManifestJson).Wait();
        }

        private void UploadManifests(MediaWorkflowManifest workflowManifest, string rssUrl, int videoCount)
        {
            //XmlDocument rssDocument;
            //StorageBlobClient blobClient = new StorageBlobClient();
            //using (WebClient webClient = new WebClient())
            //{
            //    HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, rssUrl);
            //    rssDocument = webClient.GetResponse<XmlDocument>(webRequest);
            //}
            //XmlElement channel = rssDocument.DocumentElement["channel"];
            //XmlNodeList videos = channel.SelectNodes("item");
            //XmlNamespaceManager namespaceManager = new XmlNamespaceManager(rssDocument.NameTable);
            //namespaceManager.AddNamespace(Constant.Media.Channel9.NamespacePrefix, Constant.Media.Channel9.NamespaceUrl);
            //for (int i = 0; i < videoCount; i++)
            //{
            //    XmlNode video = videos[i];
            //    XmlNode videoContent = video.SelectSingleNode(Constant.Media.Channel9.XPathQuery, namespaceManager);
            //    string videoUrl = videoContent.Attributes["url"].Value;
            //    string videoDescription = video.SelectSingleNode("title").InnerText;
            //    videoUrl = videoUrl.Replace(Constant.Media.Channel9.UrlHttp, Constant.Media.Channel9.UrlHttps);
            //    videoUrl = videoUrl.Replace(Constant.Media.Channel9.Http, Constant.Media.Channel9.Https);
            //    workflowManifest.JobInputFileUrl = videoUrl;
            //    workflowManifest.JobOutputAssetDescriptions = new string[] { videoDescription };
            //    UploadManifest(blobClient, workflowManifest);
            //}
        }

        public void Syndicate(string rssUrl = "https://channel9.msdn.com/Shows/AI-Show/feed/mp4high", int videoCount = Constant.Media.Channel9.DefaultIngestVideoCount)
        {
            //StorageBlobClient blobClient = new StorageBlobClient();
            //string containerName = Constant.Storage.BlobContainer.MediaServices;
            //string fileName = string.Concat(Constant.Media.IngestManifest.TriggerPrefix, Constant.Media.IngestManifest.FileExtension);
            //CloudBlockBlob manifestFile = blobClient.GetBlockBlob(containerName, null, fileName);
            //if (manifestFile.ExistsAsync().Result)
            //{
            //    MediaWorkflowManifest workflowManifest;
            //    using (Stream manifestStream = manifestFile.OpenReadAsync().Result)
            //    {
            //        StreamReader manifestReader = new StreamReader(manifestStream);
            //        string manifestData = manifestReader.ReadToEnd();
            //        workflowManifest = JsonConvert.DeserializeObject<MediaWorkflowManifest>(manifestData);
            //    }
            //    using (MediaClient mediaClient = new MediaClient(workflowManifest.MediaAccounts[0], null))
            //    {
            //        Account.DeleteEntities(mediaClient, false);
            //    }
            //    UploadManifests(workflowManifest, rssUrl, videoCount);
            //}
        }

        public JsonResult Block(string name, int chunk, int chunks, string storageAccount, string contentType)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            User currentUser = new User(authToken);
            StorageBlobClient blobClient = new StorageBlobClient(currentUser.MediaAccountPrimary, storageAccount);
            Stream blockStream = Request.Form.Files[0].OpenReadStream();
            string containerName = Constant.Storage.Blob.WorkflowContainerName;
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