using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class analyticsController : Controller
    {
        public JsonResult metadata(string documentId, double timeSeconds)
        {
            JObject metadataFragment;
            string collectionId = Constant.Database.Collection.Metadata;
            string procedureId = Constant.Database.Procedure.MetadataFragment;
            using (DatabaseClient databaseClient = new DatabaseClient(true))
            {
                metadataFragment = databaseClient.ExecuteProcedure(collectionId, procedureId, documentId, timeSeconds);
            }
            return Json(metadataFragment);
        }
    }
}
