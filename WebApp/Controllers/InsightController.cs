using System.IO;
using System.Net;

using Microsoft.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
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
                JContainer insights = null;
                string authToken = HomeController.GetAuthToken(Request, Response);
                using (MediaClient mediaClient = new MediaClient(authToken))
                {
                    if (!string.IsNullOrEmpty(insightId))
                    {
                        if (mediaClient.IndexerInsightExists(insightId, out JObject insight))
                        {
                            insights = insight;
                        }
                    }
                    else
                    {
                        Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                        StorageBlobClient blobClient = new StorageBlobClient(mediaClient.MediaAccount, asset.StorageAccountName);
                        CloudBlockBlob fileBlob = blobClient.GetBlockBlob(asset.Container, null, fileName);
                        using (Stream fileStream = fileBlob.OpenReadAsync().Result)
                        {
                            StreamReader fileReader = new StreamReader(fileStream);
                            string fileData = fileReader.ReadToEnd().TrimStart();
                            if (fileData.StartsWith("["))
                            {
                                insights = JArray.Parse(fileData);
                            }
                            else
                            {
                                insights = JObject.Parse(fileData);
                            }
                        }
                    }
                }
                return Json(insights);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
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
                        if (mediaClient.IndexerInsightExists(insightId, out JObject insight))
                        {
                            insights.Add(insight);
                        }
                    }
                }
                return Json(insights);
            }
            catch (ValidationException ex)
            {
                Error error = new Error()
                {
                    Type = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
                return new JsonResult(error)
                {
                    StatusCode = (int)error.Type
                };
            }
            catch (ApiErrorException ex)
            {
                return new JsonResult(ex.Response.Content)
                {
                    StatusCode = (int)ex.Response.StatusCode
                };
            }
        }

        public IActionResult Index(string insightId)
        {
            JArray insights = new JArray();
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                if (!string.IsNullOrEmpty(insightId))
                {
                    if (mediaClient.IndexerInsightExists(insightId, out JObject insight))
                    {
                        insights.Add(insight);
                    }
                }
                else
                {
                    insights = mediaClient.IndexerGetInsights();
                }
            }
            ViewData["indexerInsights"] = insights;
            return View();
        }

        public IActionResult Projects()
        {
            return View();
        }

        public IActionResult ModelsPeople()
        {
            return View();
        }

        public IActionResult ModelsLanguage()
        {
            return View();
        }

        public IActionResult ModelsBrand()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}