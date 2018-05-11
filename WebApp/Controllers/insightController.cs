using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class insightController : Controller
    {
        public static string GetProcessorName(MediaProcessor mediaProcessor)
        {
            return Processor.GetProcessorName(mediaProcessor);
        }

        public JsonResult metadata(MediaProcessor mediaProcessor, string documentId, double timeSeconds)
        {
            JObject metadata;
            string collectionId = Constant.Database.Collection.OutputInsight;
            string procedureId = Constant.Database.Procedure.TimecodeFragment;
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                if (mediaProcessor == MediaProcessor.VideoAnalyzer)
                {
                    metadata = databaseClient.GetDocument(collectionId, documentId);
                }
                else
                {
                    metadata = databaseClient.GetDocument(collectionId, procedureId, documentId, timeSeconds);
                }
            }
            return Json(metadata);
        }

        [HttpGet]
        [Route("/insight/accounts")]
        public JArray GetAccounts()
        {
            JArray accounts = null;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
                accounts = videoAnalyzer.GetAccounts();
            }
            return accounts;
        }

        [HttpGet]
        [Route("/insight/get")]
        public JObject GetInsight(string indexId, string languageId, bool processingState)
        {
            JObject index = null;
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
                index = videoAnalyzer.GetIndex(indexId, languageId, processingState);
            }
            return index;
        }

        [HttpDelete]
        [Route("/insight/delete")]
        public void DeleteVideo(string indexId, bool deleteInsight)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            if (!string.IsNullOrEmpty(authToken))
            {
                MediaClient mediaClient = new MediaClient(authToken);
                VideoAnalyzer videoAnalyzer = new VideoAnalyzer(mediaClient.MediaAccount);
                videoAnalyzer.DeleteVideo(indexId, deleteInsight);

                DatabaseClient databaseClient = new DatabaseClient();
                string collectionId = Constant.Database.Collection.OutputInsight;
                databaseClient.DeleteDocument(collectionId, indexId);
            }
        }
    }
}