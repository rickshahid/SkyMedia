using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class SearchController : Controller
    {
        //public JsonResult Metadata(MediaProcessor mediaProcessor, string documentId, double timeSeconds)
        //{
        //    JObject metadata;
        //    string collectionId = Constant.Database.Collection.OutputInsight;
        //    string procedureId = Constant.Database.Procedure.TimecodeFragment;
        //    using (DatabaseClient databaseClient = new DatabaseClient())
        //    {
        //        if (mediaProcessor == MediaProcessor.VideoAnalyzer)
        //        {
        //            metadata = databaseClient.GetDocument(collectionId, documentId);
        //        }
        //        else
        //        {
        //            metadata = databaseClient.GetDocument(collectionId, procedureId, documentId, timeSeconds);
        //        }
        //    }
        //    return Json(metadata);
        //}

        //[HttpGet]
        //[Route("/insight/accounts")]
        //public JArray GetAccounts()
        //{
        //    JArray accounts = null;
        //    string authToken = HomeController.GetAuthToken(Request, Response);
        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        using MediaClient mediaClient = new MediaClient(authToken);
        //        VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
        //        accounts = videoAnalyzer.GetAccounts();
        //    }
        //    return accounts;
        //}

        //[HttpGet]
        //[Route("/insight/get")]
        //public JObject GetInsight(string indexId, string languageId, bool processingState)
        //{
        //    JObject index = null;
        //    string authToken = HomeController.GetAuthToken(Request, Response);
        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        using MediaClient mediaClient = new MediaClient(authToken);
        //        VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
        //        index = videoAnalyzer.GetIndex(indexId, languageId, processingState);
        //    }
        //    return index;
        //}

        //[HttpDelete]
        //[Route("/insight/delete")]
        //public void DeleteVideo(string indexId, bool deleteInsight)
        //{
        //    string authToken = HomeController.GetAuthToken(Request, Response);
        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        using MediaClient mediaClient = new MediaClient(authToken);
        //        VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
        //        videoAnalyzer.DeleteVideo(indexId, deleteInsight);

        //        using (DatabaseClient databaseClient = new DatabaseClient())
        //        {
        //            string collectionId = Constant.Database.Collection.OutputInsight;
        //            databaseClient.DeleteDocument(collectionId, indexId);
        //        }
        //    }
        //}

        public IActionResult Index()
        {
            ViewData["indexerLanguages"] = HomeController.GetSpokenLanguages(true, true);
            return View();
        }
    }
}