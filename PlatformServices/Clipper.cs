using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Clipper
    {
        private static IAsset GetSourceAsset(MediaClient mediaClient, string sourceUrl)
        {
            string locatorId = sourceUrl.Split('/')[3];
            locatorId = string.Concat(Constant.Media.Stream.LocatorIdPrefix, locatorId);
            ILocator locator = mediaClient.GetEntityById(MediaEntity.Locator, locatorId) as ILocator;
            return locator.Asset;
        }

        public static object CreateFilter(string filterName, MediaClient mediaClient, string sourceUrl, int markIn, int markOut)
        {
            IAsset asset = GetSourceAsset(mediaClient, sourceUrl);
            return mediaClient.CreateFilter(asset, filterName, markIn, markOut);
        }

        public static object SubmitJob(string authToken, MediaClient mediaClient, string sourceUrl, int markIn, int markOut)
        {
            IAsset asset = GetSourceAsset(mediaClient, sourceUrl);

            MediaAssetInput inputAsset = new MediaAssetInput();
            inputAsset.AssetId = asset.Id;
            inputAsset.MarkInSeconds = markIn;
            inputAsset.MarkOutSeconds = markOut;

            MediaAssetInput[] inputAssets = new MediaAssetInput[] { inputAsset };
            inputAssets = Workflow.GetInputAssets(mediaClient, inputAssets);

            MediaJobTask jobTask = new MediaJobTask();
            jobTask.ProcessorType = MediaProcessor.EncoderStandard;
            jobTask.ProcessorConfig = Constant.Media.ProcessorConfig.EncoderStandardDefaultPreset;

            MediaJob mediaJob = new MediaJob();
            mediaJob.Tasks = new MediaJobTask[] { jobTask };
            mediaJob.NodeType = ReservedUnitType.Premium;
            mediaJob = MediaClient.GetJob(mediaClient, mediaJob, inputAssets);

            IJobTemplate jobTemplate;
            IJob job = mediaClient.CreateJob(mediaJob, inputAssets, out jobTemplate);
            if (!string.IsNullOrEmpty(job.Id))
            {
                string storageAccount = mediaClient.DefaultStorageAccount.Name;
                Workflow.TrackJob(authToken, job, storageAccount, null);
            }
            return Workflow.GetJobResult(mediaClient, job, jobTemplate, null);
        }
    }
}
