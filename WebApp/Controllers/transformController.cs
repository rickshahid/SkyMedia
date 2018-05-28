using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class transformController : Controller
    {
        private bool HasUniqueName(string processorPreset, out JObject presetConfig)
        {
            bool uniqueName = true;
            presetConfig = JObject.Parse(processorPreset);
            string presetName = presetConfig["PresetName"].ToString();
            string mediaProcessor = presetConfig["MediaProcessor"].ToString();
            string appDirectory = Directory.GetCurrentDirectory();
            string presetsDirectory = string.Concat(appDirectory, Constant.Media.Models, Constant.Media.Presets, mediaProcessor);
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
                presetConfig["id"] = string.Concat(authUser.MediaAccount.Name, Constant.TextDelimiter.Identifier, mediaProcessor, Constant.TextDelimiter.Identifier, presetName);
            }
            return uniqueName;
        }

        private static int OrderByName(SelectListItem leftItem, SelectListItem rightItem)
        {
            return string.Compare(leftItem.Text, rightItem.Text);
        }

        internal static SelectListItem[] GetProcessorPresets(MediaProcessor mediaProcessor, string accountName, bool includeLadder)
        {
            List<SelectListItem> processorPresets = new List<SelectListItem>();
            Dictionary<string, string> presets = Processor.GetProcessorPresets(mediaProcessor, accountName);
            if (includeLadder)
            {
                SelectListItem processorPreset = new SelectListItem()
                {
                    Text = Constant.Media.ProcessorPreset.StreamingLadderPresetName,
                    Value = Constant.Media.ProcessorPreset.StreamingLadderPresetValue,
                    Selected = true
                };
                processorPresets.Add(processorPreset);
                processorPreset = new SelectListItem()
                {
                    Text = Constant.Media.ProcessorPreset.DownloadLadderPresetName,
                    Value = Constant.Media.ProcessorPreset.DownloadLadderPresetValue
                };
                processorPresets.Add(processorPreset);
            }
            foreach (KeyValuePair<string, string> preset in presets)
            {
                SelectListItem processorPreset = new SelectListItem()
                {
                    Text = preset.Value,
                    Value = preset.Key
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
            SelectListItem[] presets = GetProcessorPresets(mediaProcessor, authUser.MediaAccount.Name, false);
            return Json(presets);
        }

        public JsonResult config(string presetId)
        {
            JObject preset;
            string collectionId = Constant.Database.Collection.ProcessorPreset;
            string procedureId = Constant.Database.Procedure.ProcessorPreset;
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                preset = databaseClient.GetDocument(collectionId, procedureId, "id", presetId);
            }
            return Json(preset);
        }

        public JsonResult save(string processorPreset)
        {
            bool saved = false;
            if (HasUniqueName(processorPreset, out JObject presetConfig))
            {
                string collectionId = Constant.Database.Collection.ProcessorPreset;
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    databaseClient.UpsertDocument(collectionId, presetConfig);
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
                string collectionId = Constant.Database.Collection.ProcessorPreset;
                string documentId = presetConfig["id"].ToString();
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    databaseClient.DeleteDocument(collectionId, documentId);
                    deleted = true;
                }
            }
            return Json(deleted);
        }

        //public JsonResult start(string[] assetIds, MediaJob mediaJob)
        //{
        //    string directoryId = homeController.GetDirectoryId(this.Request);
        //    string authToken = homeController.GetAuthToken(this.Request, this.Response);
        //    MediaClient mediaClient = new MediaClient(authToken);
        //    MediaJobInput[] jobInputs = Workflow.GetJobInputs(mediaClient, assetIds);
        //    object jobOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs);
        //    return Json(jobOutput);
        //}

        public IActionResult index()
        {
            //string authToken = homeController.GetAuthToken(this.Request, this.Response);
            //homeController.SetViewData(authToken, this.ViewData);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            ViewData["mediaProcessor"] = homeController.GetMediaProcessors(authToken, true);
            ViewData["mediaProcessorPreset"] = new SelectListItem[] { };
            return View();
        }
    }
}