using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class presetController : Controller
    {
        public JsonResult names(string mediaProcessor)
        {
            string appDirectory = Directory.GetCurrentDirectory();
            string presetsDirectory = string.Concat(appDirectory, @"\Models\ProcessorPresets\", mediaProcessor);
            string[] presetNames = Directory.GetFiles(presetsDirectory);
            for (int i = 0; i < presetNames.Length; i++)
            {
                presetNames[i] = Path.GetFileNameWithoutExtension(presetNames[i]);
            }
            return Json(presetNames);
        }

        public JsonResult name(string mediaProcessor, string presetName)
        {
            JObject preset;
            string collectionId = Constant.Database.Collection.ProcessorConfig;
            string procedureId = Constant.Database.Procedure.ProcessorConfig;
            using (DocumentClient documentClient = new DocumentClient())
            {
                preset = documentClient.ExecuteProcedure(collectionId, procedureId, "ProcessorName", presetName);
            }
            return Json(preset);
        }

        public IActionResult index()
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["mediaProcessor"] = homeController.GetMediaProcessors(authToken, true);
            ViewData["mediaProcessorPreset"] = new SelectListItem[] { };
            return View();
        }
    }
}