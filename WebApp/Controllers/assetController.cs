using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class assetController : Controller
    {
        public JsonResult create(string storageAccount, bool storageEncryption, string uploadAssetName, bool multipleFileAsset, string[] fileNames)
        {
            string directoryId = homeController.GetDirectoryId(this.Request);
            string authToken = homeController.GetAuthToken(this.Request, this.Response);
            MediaClient mediaClient = new MediaClient(authToken);
            Asset[] assets = Workflow.CreateAssets(authToken, mediaClient, storageAccount, storageEncryption, uploadAssetName, multipleFileAsset, fileNames);
            return Json(assets);
        }

        //private MediaJobInput[] GetJobInputs(JObject clip)
        //{
        //    List<MediaJobInput> jobInputs = new List<MediaJobInput>();
        //    JToken[] inputIds = clip["inputsIds"].ToArray();
        //    foreach (JToken inputId in inputIds)
        //    {
        //        MediaJobInput jobInput = new MediaJobInput()
        //        {
        //            AssetId = inputId["id"].ToString(),
        //            AssetType = inputId["type"].ToString()
        //        };
        //        jobInputs.Add(jobInput);
        //    }
        //    return jobInputs.ToArray();
        //}

        //public JsonResult streams(string searchCriteria, int skipCount, int takeCount, string streamType)
        //{
        //    string authToken = homeController.GetAuthToken(this.Request, this.Response);
        //    MediaStream[] streams = Media.GetMediaStreams(authToken, searchCriteria, skipCount, takeCount, streamType);
        //    return Json(streams);
        //}

        //public JsonResult clip(string clipData)
        //{
        //    object clipOutput = null;
        //    JObject clip = JObject.Parse(clipData);
        //    MediaJobInput[] jobInputs = GetJobInputs(clip);
        //    string authToken = homeController.GetAuthToken(this.Request, this.Response);
        //    MediaClient mediaClient = new MediaClient(authToken);
        //    if (string.Equals(jobInputs[0].AssetType, "filter", StringComparison.OrdinalIgnoreCase))
        //    {
        //        string filterName = clip["name"].ToString();
        //        string assetId = jobInputs[0].AssetId;
        //        clipOutput = MediaClient.CreateFilter()
        //    }
        //    else
        //    {
        //        MediaJobTask jobTask = new MediaJobTask()
        //        {
        //            MediaProcessor = MediaProcessor.EncoderStandard,
        //            ProcessorConfig = clip["output"]["job"].ToString()
        //        };
        //        MediaJob mediaJob = new MediaJob()
        //        {
        //            Name = clip["name"].ToString(),
        //            Tasks = new MediaJobTask[] { jobTask }
        //        };
        //        string directoryId = homeController.GetDirectoryId(this.Request);
        //        mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs.ToArray());
        //        //clipOutput = Workflow.SubmitJob(directoryId, authToken, mediaClient, mediaJob, jobInputs.ToArray());
        //    }
        //    return Json(clipOutput);
        //}

        //private AssetNode[] GetAssets(string authToken, string assetId, bool getFiles)
        //{
        //    List<AssetNode> assets = new List<AssetNode>();
        //    if (string.IsNullOrEmpty(assetId))
        //    {
        //        //foreach (IAsset asset in _media2.Assets)
        //        //{
        //        //    if (asset.ParentAssets.Count == 0)
        //        //    {
        //        //        AssetNode assetNode = new AssetNode(authToken, asset, getFiles);
        //        //        assets.Add(assetNode);
        //        //    }
        //        //}
        //    }
        //    else
        //    {
        //        //IAsset rootAsset = GetEntityById(MediaEntity.Asset, assetId) as IAsset;
        //        //if (rootAsset != null)
        //        //{
        //        //    foreach (IAssetFile assetFile in rootAsset.AssetFiles)
        //        //    {
        //        //        AssetNode fileNode = new AssetNode(authToken, assetFile);
        //        //        assets.Add(fileNode);
        //        //    }
        //        //}
        //        //foreach (IAsset asset in _media2.Assets)
        //        //{
        //        //    foreach (IAsset parentAsset in asset.ParentAssets)
        //        //    {
        //        //        if (string.Equals(parentAsset.Id, rootAsset.Id, StringComparison.OrdinalIgnoreCase))
        //        //        {
        //        //            AssetNode assetNode = new AssetNode(authToken, asset, getFiles);
        //        //            assets.Add(assetNode);
        //        //        }
        //        //    }
        //        //}
        //    }
        //    return assets.ToArray();
        //}

        //public JsonResult nodes(string assetId, bool getFiles)
        //{
        //    string authToken = homeController.GetAuthToken(this.Request, this.Response);
        //    MediaClient mediaClient = new MediaClient(authToken);
        //    AssetNode[] assets = GetAssets(authToken, assetId, getFiles);
        //    return Json(assets);
        //}

        public IActionResult clipper()
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