using Microsoft.AspNetCore.Mvc;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class ComposeController : Controller
    {
        public JsonResult Streams(string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            MediaStream[] streams = Media.GetClipperStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
            return Json(streams);
        }

        public JsonResult Clip(string clipData)
        {
            object clipOutput = null;
            //JObject clip = JObject.Parse(clipData);
            //MediaJobInput[] jobInputs = GetJobInputs(clip);
            //string authToken = homeController.GetAuthToken(this.Request, this.Response);
            //MediaClient mediaClient = new MediaClient(authToken);
            //if (jobInputs[0].AssetFilter)
            //{
            //    string filterName = clip["name"].ToString();
            //    string assetId = jobInputs[0].AssetId;
            //    //clipOutput = MediaClient.CreateFilter()
            //}
            //else
            //{
            //    MediaJobTask jobTask = new MediaJobTask()
            //    {
            //        MediaProcessor = MediaProcessor.EncoderStandard,
            //        ProcessorConfig = clip["output"]["job"].ToString()
            //    };
            //    MediaJob mediaJob = new MediaJob()
            //    {
            //        Name = clip["name"].ToString(),
            //        NodeType = MediaJobNodeType.Premium,
            //        Tasks = new MediaJobTask[] { jobTask }
            //    };
            //    string directoryId = homeController.GetDirectoryId(this.Request);
            //    mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs.ToArray());
            //    clipOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs.ToArray());
            //}
            return Json(clipOutput);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}