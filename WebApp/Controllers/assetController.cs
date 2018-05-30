using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        public JsonResult create(string storageAccount, string assetName, string description, string alternateId,
                                 string[] fileNames, bool multipleFileAsset, bool standardEncoderAsset)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = Workflow.CreateAssets(authToken, mediaClient, storageAccount, assetName, description, alternateId, multipleFileAsset, fileNames);

            List<Job> jobs = new List<Job>();
            if (standardEncoderAsset)
            {
                string transformName = Constant.Media.Transform.PresetAdaptiveStreaming;
                EncoderNamedPreset encoderPreset = EncoderNamedPreset.AdaptiveStreaming;
                Transform transform = mediaClient.CreateTransform(transformName, encoderPreset);

                foreach (Asset asset in assets)
                {
                    JobInputAsset inputAsset = new JobInputAsset(asset.Name);

                    string outputAssetName = string.Concat(asset.Name, Constant.Media.Job.EncoderOutputAssetSuffix);
                    Asset outputAsset = mediaClient.CreateAsset(storageAccount, outputAssetName, transform.Name, string.Empty);

                    Job job = mediaClient.CreateJob(transformName, null, inputAsset, outputAsset.Name);
                    jobs.Add(job);
                }
            }

            return jobs.Count > 0 ? Json(jobs.ToArray()) : Json(assets);
        }

        public JsonResult streams(string searchCriteria, int skipCount, int takeCount, string streamType)
        {
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaStream[] streams = Media.GetMediaStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
            return Json(streams);
        }

        public IActionResult edit()
        {
            return View();
        }

        public IActionResult sprite()
        {
            return View();
        }

        public IActionResult index()
        {
            return View();
        }
    }
}