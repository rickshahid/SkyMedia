using System.IO;

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private void SetJobInputAssetArchive(JobInput jobInput, StandardBlobTier inputAssetStorageTier)
        {
            if (jobInput is JobInputAsset jobInputAsset && inputAssetStorageTier != StandardBlobTier.Unknown)
            {
                Asset inputAsset = GetEntity<Asset>(MediaEntity.Asset, jobInputAsset.AssetName);
                StorageBlobClient blobClient = new StorageBlobClient(this.MediaAccount, inputAsset.StorageAccountName);
                CloudBlockBlob[] blobs = blobClient.ListBlobContainer(inputAsset.Container, null);
                foreach (CloudBlockBlob blob in blobs)
                {
                    blob.SetStandardBlobTierAsync(inputAssetStorageTier);
                }
            }
        }

        public static MediaPublishNotification PublishJobOutput(EventGridEvent eventTrigger)
        {
            MediaPublishNotification publishNotification = new MediaPublishNotification();
            JObject eventData = JObject.FromObject(eventTrigger.Data);
            JObject jobData = (JObject)eventData["correlationData"];
            if (jobData.HasValues)
            {
                string mediaAccountData = jobData[Constant.Media.Job.CorrelationData.MediaAccount].ToString();
                string userAccountData = jobData[Constant.Media.Job.CorrelationData.UserAccount].ToString();
                MediaAccount mediaAccount = JsonConvert.DeserializeObject<MediaAccount>(mediaAccountData);
                UserAccount userAccount = JsonConvert.DeserializeObject<UserAccount>(userAccountData);
                using (MediaClient mediaClient = new MediaClient(mediaAccount, userAccount))
                {
                    string transformName = eventTrigger.Subject.Split('/')[1];
                    string jobName = Path.GetFileName(eventTrigger.Subject);
                    publishNotification = mediaClient.PublishJobOutput(transformName, jobName, eventTrigger.EventType);
                }
            }
            return publishNotification;
        }

        public MediaPublishNotification PublishJobOutput(string transformName, string jobName, string eventType)
        {
            MediaPublishNotification publishNotification = new MediaPublishNotification
            {
                PhoneNumber = this.UserAccount.MobilePhoneNumber,
                StatusMessage = string.Format(Constant.Message.JobPublishNotification, Constant.TextFormatter.FormatValue(transformName), jobName, eventType)
            };
            Job job = GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
            if (job != null)
            {
                switch (eventType)
                {
                    case Constant.Media.Job.EventType.Errored:
                        string jobError = null;
                        foreach (JobOutput jobOutput in job.Outputs)
                        {
                            if (!string.IsNullOrEmpty(jobError))
                            {
                                jobError = string.Concat(jobError, Constant.Message.NewLine);
                            }
                            jobError = string.Concat(jobError, jobOutput.Error.Message);
                        }
                        publishNotification.StatusMessage = string.Concat(publishNotification.StatusMessage, Constant.Message.NewLine, jobError);
                        break;

                    case Constant.Media.Job.EventType.Finished:
                        string jobOutputPublishData = job.CorrelationData[Constant.Media.Job.CorrelationData.OutputPublish].ToString();
                        MediaJobOutputPublish jobOutputPublish = JsonConvert.DeserializeObject<MediaJobOutputPublish>(jobOutputPublishData);
                        foreach (JobOutputAsset jobOutput in job.Outputs)
                        {
                            Asset outputAsset = GetEntity<Asset>(MediaEntity.Asset, jobOutput.AssetName);
                            StorageBlobClient blobClient = new StorageBlobClient(this.MediaAccount, outputAsset.StorageAccountName);
                            if (blobClient.ContainsFile(outputAsset.Container, null, null, Constant.Media.Stream.ManifestExtension))
                            {
                                StreamingLocator streamingLocator = GetStreamingLocator(jobOutput.AssetName, jobOutputPublish.StreamingPolicyName, jobOutputPublish.ContentProtection);
                                string streamingUrl = GetLocatorUrl(streamingLocator, null, true);
                                publishNotification.StatusMessage = string.Concat(publishNotification.StatusMessage, Constant.Message.NewLine, streamingUrl);
                            }
                        }
                        SetJobInputAssetArchive(job.Input, jobOutputPublish.InputAssetStorageTier);
                        break;
                }
            }
            return publishNotification;
        }

        public MediaPublishNotification UnpublishJobOutput(string transformName, string jobName)
        {
            MediaPublishNotification publishNotification = new MediaPublishNotification()
            {
                PhoneNumber = this.UserAccount.MobilePhoneNumber,
                StatusMessage = string.Format(Constant.Message.JobUnpublishNotification, jobName)
            };
            Job job = GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
            if (job != null)
            {
                foreach (JobOutputAsset jobOutput in job.Outputs)
                {
                    DeleteStreamingLocators(jobOutput.AssetName);
                }
            }
            return publishNotification;
        }
    }
}