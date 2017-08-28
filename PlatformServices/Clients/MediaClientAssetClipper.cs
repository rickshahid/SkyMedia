using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
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

            MediaJobInput jobInput = new MediaJobInput();
            jobInput.AssetId = asset.Id;
            jobInput.MarkInSeconds = markIn;
            jobInput.MarkOutSeconds = markOut;

            MediaJobInput[] jobInputs = new MediaJobInput[] { jobInput };
            jobInputs = Workflow.GetJobInputs(mediaClient, jobInputs);

            string settingKey = Constant.AppSettingKey.MediaClipperEncoderPreset;
            string processorConfig = AppSetting.GetValue(settingKey);

            MediaJobTask jobTask = new MediaJobTask();
            jobTask.MediaProcessor = MediaProcessor.EncoderStandard;
            jobTask.ProcessorConfig = processorConfig;

            MediaJob mediaJob = new MediaJob();
            mediaJob.Tasks = new MediaJobTask[] { jobTask };
            mediaJob.NodeType = ReservedUnitType.Premium;
            mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs);

            IJobTemplate jobTemplate;
            IJob job = mediaClient.CreateJob(mediaJob, jobInputs, out jobTemplate);
            if (job != null && !string.IsNullOrEmpty(job.Id))
            {
                string storageAccount = mediaClient.DefaultStorageAccount.Name;
                Workflow.TrackJob(authToken, job, storageAccount, null);
            }
            return Workflow.GetJobOutput(mediaClient, job, jobTemplate, null);
        }
    }
}