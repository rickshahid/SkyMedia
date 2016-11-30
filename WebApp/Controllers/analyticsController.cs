using System.Linq;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using SkyMedia.ServiceBroker;

namespace SkyMedia.WebApp.Controllers
{
    public class analyticsController : Controller
    {
        //private JToken GetFragment(JObject mediaMetadata, double timeSeconds)
        //{
        //    int timescale = int.Parse(mediaMetadata["timescale"].ToString());
        //    JToken fragments = mediaMetadata["fragments"];
        //    for (int i = 0; i < fragments.Count(); i++)
        //    {
        //        JToken fragment = fragments[i];
        //        int start = int.Parse(fragment["start"].ToString());
        //        int duration = int.Parse(fragment["duration"].ToString());
        //        double fragmentStart = start / timescale;
        //        double fragmentEnd = fragmentStart + (duration / timescale);
        //        if (timeSeconds >= fragmentStart && timeSeconds <= fragmentEnd)
        //        {
        //            return fragment;
        //        }
        //    }
        //    return null;
        //}

        public JsonResult metadata(string fileName, double timeSeconds)
        {
            string[] fileNameInfo = fileName.Split('_');
            string documentId = fileNameInfo[0];

            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constants.Media.AssetMetadata.DocumentCollection;
            documentId = string.Concat(collectionId, Constants.MultiItemSeparator, documentId);
            JObject jsonDoc = databaseClient.GetDocument(documentId);

            //JToken metadata = GetFragment(jsonDoc, timeSeconds);
            return Json(jsonDoc);
        }
    }
}
