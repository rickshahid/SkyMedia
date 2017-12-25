using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class presetController : Controller
    {
        internal static SelectListItem[] GetProcessorPresets(MediaProcessor mediaProcessor, string accountId, bool includeDefaults)
        {
            List<SelectListItem> processorPresets = new List<SelectListItem>();
            NameValueCollection presets = Processor.GetProcessorPresets(mediaProcessor, accountId);
            if (includeDefaults)
            {
                SelectListItem processorPreset = new SelectListItem()
                {
                    Text = "H.264 MBR Adaptive Streaming Ladder (Uninterleaved)",
                    Value = "Adaptive Streaming"
                };
                processorPresets.Add(processorPreset);
                processorPreset = new SelectListItem()
                {
                    Text = "H.264 MBR Adaptive Streaming Ladder (Interleaved)",
                    Value = "Content Adaptive Multiple Bitrate MP4"
                };
                processorPresets.Add(processorPreset);
            }
            foreach (string presetName in presets.Keys)
            {
                SelectListItem processorPreset = new SelectListItem()
                {
                    Text = presetName,
                    Value = presets[presetName]
                };
                processorPresets.Add(processorPreset);
            }
            return processorPresets.ToArray();
        }

        public JsonResult options(MediaProcessor mediaProcessor)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            User authUser = new User(authToken);
            SelectListItem[] presets = GetProcessorPresets(mediaProcessor, authUser.Id, false);
            return Json(presets);
        }

        public JsonResult config(string presetId)
        {
            JObject preset;
            string collectionId = Constant.Database.Collection.ProcessorConfig;
            string procedureId = Constant.Database.Procedure.ProcessorConfig;
            using (DocumentClient documentClient = new DocumentClient())
            {
                preset = documentClient.GetDocument(collectionId, procedureId, "id", presetId);
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