using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using SkyMedia.ServiceBroker;

namespace SkyMedia.WebApp.Controllers
{
    public class analyticsController : Controller
    {
        private JObject GetFragment(string documentId, double timeSeconds)
        {
            DatabaseClient databaseClient = new DatabaseClient(true);
            string collectionId = Constants.Media.AssetMetadata.DocumentCollection;
            return databaseClient.ExecuteProcedure(collectionId, "getTimecodeFragment", documentId, timeSeconds);
        }

        public JsonResult metadata(string fileName, double timeSeconds)
        {
            string[] fileNameInfo = fileName.Split(Constants.NamedItemsSeparator);
            string documentId = fileNameInfo[0];
            JObject fragment = GetFragment(documentId, timeSeconds);
            return Json(fragment);
        }
    }
}
