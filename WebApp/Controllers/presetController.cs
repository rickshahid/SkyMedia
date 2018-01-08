using System;
using System.IO;
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
        private bool HasUniqueName(string processorPreset, out JObject presetConfig)
        {
            bool uniqueName = true;
            presetConfig = JObject.Parse(processorPreset);
            string presetName = presetConfig["PresetName"].ToString();
            string mediaProcessor = presetConfig["MediaProcessor"].ToString();
            string appDirectory = Directory.GetCurrentDirectory();
            string presetsDirectory = string.Concat(appDirectory, Constant.Media.ProcessorConfig.DirectoryPresets, mediaProcessor);
            string[] presetFiles = Directory.GetFiles(presetsDirectory);
            foreach (string presetFile in presetFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(presetFile);
                if (string.Equals(fileName, presetName, StringComparison.OrdinalIgnoreCase))
                {
                    uniqueName = false;
                }
            }
            if (uniqueName)
            {
                string authToken = homeController.GetAuthToken(this.Request, this.Response);
                User authUser = new User(authToken);
                presetConfig["id"] = string.Concat(authUser.MediaAccount.Id, Constant.TextDelimiter.Identifier, mediaProcessor, Constant.TextDelimiter.Identifier, presetName);
            }
            return uniqueName;
        }

        private static int OrderByName(SelectListItem leftItem, SelectListItem rightItem)
        {
            return string.Compare(leftItem.Text, rightItem.Text);
        }

        internal static SelectListItem[] GetProcessorPresets(MediaProcessor mediaProcessor, string accountId, bool includeLadder)
        {
            List<SelectListItem> processorPresets = new List<SelectListItem>();
            NameValueCollection presets = Processor.GetProcessorPresets(mediaProcessor, accountId);
            if (includeLadder)
            {
                SelectListItem processorPreset = new SelectListItem()
                {
                    Text = Constant.Media.ProcessorConfig.StreamingLadderPresetName,
                    Value = Constant.Media.ProcessorConfig.StreamingLadderPresetValue,
                    Selected = true
                };
                processorPresets.Add(processorPreset);
                processorPreset = new SelectListItem()
                {
                    Text = Constant.Media.ProcessorConfig.DownloadLadderPresetName,
                    Value = Constant.Media.ProcessorConfig.DownloadLadderPresetValue
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
            processorPresets.Sort(OrderByName);
            return processorPresets.ToArray();
        }

        public JsonResult configs(MediaProcessor mediaProcessor)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            User authUser = new User(authToken);
            SelectListItem[] presets = GetProcessorPresets(mediaProcessor, authUser.MediaAccount.Id, false);
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

        public JsonResult save(string processorPreset)
        {
            bool saved = false;
            if (HasUniqueName(processorPreset, out JObject presetConfig))
            {
                string collectionId = Constant.Database.Collection.ProcessorConfig;
                using (DocumentClient documentClient = new DocumentClient())
                {
                    documentClient.UpsertDocument(collectionId, presetConfig);
                    saved = true;
                }
            }
            return Json(saved);
        }

        public JsonResult delete(string processorPreset)
        {
            bool deleted = false;
            if (HasUniqueName(processorPreset, out JObject presetConfig))
            {
                string collectionId = Constant.Database.Collection.ProcessorConfig;
                string documentId = presetConfig["id"].ToString();
                using (DocumentClient documentClient = new DocumentClient())
                {
                    documentClient.DeleteDocument(collectionId, documentId);
                    deleted = true;
                }
            }
            return Json(deleted);
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