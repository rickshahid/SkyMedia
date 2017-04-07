using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class analyticsController : Controller
    {
        private JObject GetMetadataFragment(string documentId, double timeSeconds)
        {
            JObject metadataFragment;
            using (DatabaseClient databaseClient = new DatabaseClient(true))
            {
                string collectionId = Constant.Database.Collection.Metadata;
                string procedureId = Constant.Database.Procedure.MetadataFragment;
                metadataFragment = databaseClient.ExecuteProcedure(collectionId, procedureId, documentId, timeSeconds);
            }
            return metadataFragment;
        }

        public JsonResult metadata(string fileName, double timeSeconds)
        {
            string documentId = fileName.Split(Constant.TextDelimiter.Identifier)[0];
            JObject metadataFragment = GetMetadataFragment(documentId, timeSeconds);
            return Json(metadataFragment);
        }
    }
}
