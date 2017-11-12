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

            MediaJobInput jobInput = new MediaJobInput()
            {
                AssetId = asset.Id,
                MarkInSeconds = markIn,
                MarkOutSeconds = markOut
            };

            MediaJobInput[] jobInputs = new MediaJobInput[] { jobInput };
            jobInputs = Workflow.GetJobInputs(mediaClient, jobInputs);

            string settingKey = Constant.AppSettingKey.MediaClipperEncoderPreset;
            string processorConfig = AppSetting.GetValue(settingKey);

            MediaJobTask jobTask = new MediaJobTask()
            {
                MediaProcessor = MediaProcessor.EncoderStandard,
                ProcessorConfig = processorConfig
            };

            MediaJob mediaJob = new MediaJob()
            {
                Tasks = new MediaJobTask[] { jobTask },
                NodeType = ReservedUnitType.Premium
            };
            mediaJob = MediaClient.GetJob(authToken, mediaClient, mediaJob, jobInputs);

            IJob job = mediaClient.CreateJob(mediaJob, jobInputs, out IJobTemplate jobTemplate);
            if (job != null && !string.IsNullOrEmpty(job.Id))
            {
                Workflow.TrackJob(Constant.DirectoryIdB2C, authToken, job, null);
            }
            return Workflow.GetJobOutput(mediaClient, job, jobTemplate, null);
        }
    }
}