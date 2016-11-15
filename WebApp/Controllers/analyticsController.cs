using System.Linq;

using Microsoft.AspNetCore.Mvc;

using SkyMedia.ServiceBroker;

using Newtonsoft.Json.Linq;

namespace SkyMedia.WebApp.Controllers
{
    public class analyticsController : Controller
    {
        private JToken GetFragment(string documentId, double timeSeconds)
        {
            DatabaseClient databaseClient = new DatabaseClient();
            string collectionId = Constants.Media.AssetMetadata.DocumentCollection;
            documentId = string.Concat(collectionId, Constants.MultiItemSeparator, documentId);
            JObject document = databaseClient.GetDocument(documentId);
            int timescale = int.Parse(document["timescale"].ToString());
            JToken fragments = document["fragments"];
            for (int i = 0; i < fragments.Count(); i++)
            {
                JToken fragment = fragments[i];
                int start = int.Parse(fragment["start"].ToString());
                int duration = int.Parse(fragment["duration"].ToString());
                double fragmentStart = start / timescale;
                double fragmentEnd = fragmentStart + (duration / timescale);
                if (timeSeconds >= fragmentStart && timeSeconds <= fragmentEnd)
                {
                    return fragment;
                }
            }
            return null;
        }

        public JsonResult metadata(MediaProcessor mediaProcessor, double timeSeconds)
        {
            JArray metadata = null;
            //switch (mediaProcessor)
            //{
            //    case MediaProcessor.FaceDetection:
            //        JToken fragment = GetFragment(documentId, timeSeconds);
            //        if (fragment != null)
            //        {
            //            metadata = MapFragment(fragment);
            //        }
            //        break;
            //}
            return Json(timeSeconds);
        }

        private JArray MapFragment(JToken fragment)
        {
            JArray metadata = new JArray();
            foreach (JToken child in fragment.Children())
            {
                string json = string.Concat("{ 'text': '" + child.ToString() + "' }");
                metadata.Add(JObject.Parse(json));
            }
            return metadata;
        }
    }
}
