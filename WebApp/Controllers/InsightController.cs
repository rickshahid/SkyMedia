using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class InsightController : Controller
    {
        public JsonResult Data(string assetName, string fileName, string insightId)
        {
            try
            {
                JContainer insight;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    if (!string.IsNullOrEmpty(insightId))
                    {
                        insight = mediaClient.IndexerGetInsight(insightId);
                    }
                    else
                    {
                        Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                        StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
                        CloudBlockBlob fileBlob = blobClient.GetBlockBlob(asset.Container, fileName);
                        using (Stream fileStream = fileBlob.OpenReadAsync().Result)
                        {
                            StreamReader fileReader = new StreamReader(fileStream);
                            string fileData = fileReader.ReadToEnd().TrimStart();
                            if (fileData.StartsWith("["))
                            {
                                insight = JArray.Parse(fileData);
                            }
                            else
                            {
                                insight = JObject.Parse(fileData);
                            }
                        }
                    }
                }
                return Json(insight);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Reindex(string insightId)
        {
            try
            {
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    mediaClient.IndexerReindexVideo(insightId, Priority.Normal);
                }
                return Json(insightId);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Query(string searchQuery)
        {
            try
            {
                JObject searchResults;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    searchResults = mediaClient.IndexerSearch(searchQuery);
                }
                return Json(searchResults);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public JsonResult Refresh(string[] insightIds)
        {
            try
            {
                JArray insights = new JArray();
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    foreach (string insightId in insightIds)
                    {
                        JObject insight = mediaClient.IndexerGetInsight(insightId);
                        insights.Add(insight);
                    }
                }
                return Json(insights);
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["indexerInsights"] = mediaClient.IndexerGetInsights();
            }
            return View();
        }

        public IActionResult Item(string insightId)
        {
            JArray insights = new JArray();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                JObject insight = mediaClient.IndexerGetInsight(insightId);
                if (insight != null)
                {
                    insights.Add(insight);
                }
            }
            ViewData["indexerInsights"] = insights;
            return View();
        }
    }
}