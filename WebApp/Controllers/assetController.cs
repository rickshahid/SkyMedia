using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class AssetController : Controller
    {
        private Asset[] CreateInputAssets(MediaClient mediaClient, string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames)
        {
            List<Asset> inputAssets = new List<Asset>();
            BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, storageAccount);
            foreach (string fileName in fileNames)
            {
                string sourceContainer = Constant.Storage.BlobContainer.FileUpload;
                Asset inputAsset = mediaClient.CreateAsset(blobClient, blobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, fileName);
                inputAssets.Add(inputAsset);
            }
            return inputAssets.ToArray();
        }

        public JsonResult Workflow(string storageAccount, string assetName, string assetDescription, string assetAlternateId, string[] fileNames,
                                   bool standardEncoderPreset, bool thumbnailSpritePreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset,
                                   string streamingPolicyName)
        {
            Asset[] inputAssets;
            List<Job> jobs = new List<Job>();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                Transform transform = mediaClient.CreateTransform(standardEncoderPreset, videoAnalyzerPreset, audioAnalyzerPreset);
                inputAssets = CreateInputAssets(mediaClient, storageAccount, assetName, assetDescription, assetAlternateId, fileNames);
                foreach (Asset inputAsset in inputAssets)
                {
                    Job job = null;
                    string indexId = null;
                    if (mediaClient.IndexerIsEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
                    {
                        BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, storageAccount);
                        MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, inputAsset);
                        string fileName = mediaAsset.Files[0].Name;
                        string videoUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                        bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                        indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, videoUrl, inputAsset.Name, inputAsset.Description, string.Empty, audioOnly);
                    }
                    if (transform != null)
                    {
                        job = mediaClient.CreateJob(transform.Name, null, null, Priority.Normal, inputAsset.Name, null, inputAsset.StorageAccountName, assetDescription, indexId, streamingPolicyName);
                    }
                    if (job != null)
                    {
                        if (!string.IsNullOrEmpty(indexId))
                        {
                            job.CorrelationData.Add("indexId", indexId);
                        }
                        jobs.Add(job);
                    }
                    else
                    {
                        inputAsset.AlternateId = indexId;
                    }
                }
            }
            return jobs.Count > 0 ? Json(jobs.ToArray()) : Json(inputAssets);
        }

        public void Create(int assetCount, string assetType, bool assetPublish)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                BlobClient sourceBlobClient = new BlobClient();
                BlobClient assetBlobClient = new BlobClient(mediaClient.MediaAccount, mediaClient.PrimaryStorageAccount);
                for (int i = 1; i <= assetCount; i++)
                {
                    int assetId = i % 2 == 0 ? 2 : i % 2;
                    List<Task> uploadTasks = new List<Task>();
                    string containerName = Constant.Storage.BlobContainer.MediaServices;
                    string directoryPath = string.Concat(assetType, "/", assetId.ToString());
                    string assetName = MediaAsset.GetAssetName(sourceBlobClient, containerName, directoryPath);
                    assetName = string.Concat(i.ToString(), Constant.Media.Asset.NameDelimiter, assetName);
                    Asset asset = mediaClient.CreateAsset(mediaClient.PrimaryStorageAccount, assetName);
                    MediaFile[] sourceFiles = MediaAsset.GetAssetFiles(sourceBlobClient, containerName, directoryPath);
                    foreach (MediaFile sourceFile in sourceFiles)
                    {
                        CloudBlockBlob sourceBlob = sourceBlobClient.GetBlockBlob(containerName, sourceFile.Name);
                        CloudBlockBlob assetBlob = assetBlobClient.GetBlockBlob(asset.Container, sourceFile.Name);
                        Stream sourceStream = sourceBlob.OpenReadAsync().Result;
                        Task uploadTask = assetBlob.UploadFromStreamAsync(sourceStream);
                        uploadTasks.Add(uploadTask);
                    }
                    if (assetPublish)
                    {
                        Task.WaitAll(uploadTasks.ToArray());
                        string streamingPolicyName = assetType == Constant.Media.Asset.SingleBitrate ? PredefinedStreamingPolicy.DownloadOnly : PredefinedStreamingPolicy.ClearStreamingOnly;
                        mediaClient.CreateLocator(asset.Name, asset.Name, streamingPolicyName, null);
                    }
                }
            }
        }

        public JsonResult Find(string assetName)
        {
            Asset asset = null;
            if (!string.IsNullOrEmpty(assetName))
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                }
            }
            return Json(asset);
        }

        public JsonResult Insight(string assetName, string fileName, string indexId)
        {
            JObject insight;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                if (!string.IsNullOrEmpty(indexId))
                {
                    insight = mediaClient.IndexerGetInsight(indexId);
                }
                else
                {
                    Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                    BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
                    CloudBlockBlob fileBlob = blobClient.GetBlockBlob(asset.Container, fileName);
                    using (Stream fileStream = fileBlob.OpenReadAsync().Result)
                    {
                        StreamReader fileReader = new StreamReader(fileStream);
                        string fileData = fileReader.ReadToEnd();
                        insight = JObject.Parse(fileData);
                    }
                }
            }
            return Json(insight);
        }

        public void Reindex(string indexId)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                mediaClient.IndexerReindexVideo(indexId);
            }
        }

        public IActionResult Sprite()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}