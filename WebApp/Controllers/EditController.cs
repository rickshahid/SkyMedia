using Microsoft.AspNetCore.Mvc;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class EditController : Controller
    {
        //private MediaJobInput[] GetJobInputs(JObject clip)
        //{
        //    List<MediaJobInput> jobInputs = new List<MediaJobInput>();
        //    JToken[] inputIds = clip["inputsIds"].ToArray();
        //    foreach (JToken inputId in inputIds)
        //    {
        //        MediaJobInput jobInput = new MediaJobInput()
        //        {
        //            AssetId = inputId["id"].ToString(),
        //            AssetType = inputId["type"].ToString()
        //        };
        //        jobInputs.Add(jobInput);
        //    }
        //    return jobInputs.ToArray();
        //}

        //public JsonResult Streams(string assetName, int skipCount, int takeCount, string streamType)
        //{
        //    try
        //    {
        //        string authToken = HomeController.GetAuthToken(Request, Response);
        //        MediaStream[] mediaStreams = Media.GetClipperStreams(authToken, assetName, skipCount, takeCount, streamType);
        //        return Json(mediaStreams);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        Error error = new Error()
        //        {
        //            Type = HttpStatusCode.BadRequest,
        //            Message = ex.Message
        //        };
        //        return new JsonResult(error)
        //        {
        //            StatusCode = (int)error.Type
        //        };
        //    }
        //    catch (ApiErrorException ex)
        //    {
        //        return new JsonResult(ex.Response.Content)
        //        {
        //            StatusCode = (int)ex.Response.StatusCode
        //        };
        //    }
        //}

        //public JsonResult Clip(string clipData)
        //{
        //    try
        //    {
        //        JObject clip = JObject.Parse(clipData);
        //        string clipName = clip["name"].ToString();
        //        string clipType = clip["output"]["type"].ToString();
        //        string authToken = HomeController.GetAuthToken(Request, Response);
        //        using (MediaClient mediaClient = new MediaClient(authToken))
        //        {
        //            switch (clipType)
        //            {
        //                case "filter":
        //                    string assetId = clip["inputsIds"][0]["id"].ToString();
        //                    JToken timeRange = clip["output"]["filter"]["PresentationTimeRange"];
        //                    long timescale = (long)timeRange["Timescale"];
        //                    long startTimestamp = (long)timeRange["StartTimestamp"];
        //                    long endTimestamp = (long)timeRange["EndTimestamp"];
        //                    mediaClient.CreateFilter(assetId, clipName, timescale, startTimestamp, endTimestamp);
        //                    break;
        //                case "rendered":
        //                    break;
        //            }
        //        }
        //        return Json(true);
        //    }
        //    catch (ApiErrorException ex)
        //    {
        //        return new JsonResult(ex.Response.Content)
        //        {
        //            StatusCode = (int)ex.Response.StatusCode
        //        };
        //    }
        //    MediaJobInput[] jobInputs = GetJobInputs(clip);
        //    if (jobInputs[0].AssetFilter)
        //    {
        //        string filterName = clip["name"].ToString();
        //        string assetId = jobInputs[0].AssetId;
        //        //clipOutput = MediaClient.CreateFilter()
        //    }
        //    else
        //    {
        //        MediaJobTask jobTask = new MediaJobTask()
        //        {
        //            MediaProcessor = MediaProcessor.EncoderStandard,
        //            ProcessorConfig = clip["output"]["job"].ToString()
        //        };
        //        MediaJob mediaJob = new MediaJob()
        //        {
        //            Name = clip["name"].ToString(),
        //            NodeType = MediaJobNodeType.Premium,
        //            Tasks = new MediaJobTask[] { jobTask }
        //        };
        //        string directoryId = homeController.GetDirectoryId(this.Request);
        //        mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs.ToArray());
        //        clipOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs.ToArray());
        //    }
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}