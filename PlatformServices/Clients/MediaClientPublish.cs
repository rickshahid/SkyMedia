using System.IO;

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {       
        public static MediaPublishNotification PublishJobOutput(EventGridEvent eventTrigger)
        {
            JObject eventData = JObject.FromObject(eventTrigger.Data);
            JObject jobData = (JObject)eventData["correlationData"];

            UserAccount userAccount = JsonConvert.DeserializeObject<UserAccount>(jobData["userAccount"].ToString());
            MediaPublishNotification publishNotification = new MediaPublishNotification
            {
                PhoneNumber = userAccount.MobilePhoneNumber,
                StatusMessage = eventTrigger.EventType
            };

            if (eventTrigger.EventType == "Microsoft.Media.JobFinished")
            {
                MediaAccount mediaAccount = JsonConvert.DeserializeObject<MediaAccount>(jobData["mediaAccount"].ToString());
                using (MediaClient mediaClient = new MediaClient(userAccount, mediaAccount))
                {
                    string transformName = eventTrigger.Subject.Split('/')[1];
                    string jobName = Path.GetFileName(eventTrigger.Subject);
                    Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);

                    foreach (JobOutputAsset jobOutput in job.Outputs)
                    {
                        Asset outputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, jobOutput.AssetName);
                        MediaAsset outputMediaAsset = new MediaAsset(mediaClient, outputAsset);
                        if (outputMediaAsset.Streamable)
                        {
                            string streamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly;
                            if (job.CorrelationData.ContainsKey("streamingPolicyName"))
                            {
                                streamingPolicyName = job.CorrelationData["streamingPolicyName"];
                            }

                            ContentProtection contentProtection = null;
                            if (job.CorrelationData.ContainsKey("contentProtection"))
                            {
                                string contentProtectionJson = job.CorrelationData["contentProtection"];
                                contentProtection = JsonConvert.DeserializeObject<ContentProtection>(contentProtectionJson);
                            }

                            StreamingLocator streamingLocator = mediaClient.CreateLocator(jobOutput.AssetName, jobOutput.AssetName, streamingPolicyName, contentProtection);
                            string streamingUrl = string.Concat("http:", mediaClient.GetPlayerUrl(streamingLocator));
                            publishNotification.StatusMessage = string.Concat(publishNotification.StatusMessage, "\n\n", streamingUrl);
                        }
                    }

                    if (job.CorrelationData.ContainsKey("archiveInput") && job.Input is JobInputAsset)
                    {
                        JobInputAsset jobInput = (JobInputAsset)job.Input;
                        Asset inputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, jobInput.AssetName);
                        StorageBlobClient blobClient = new StorageBlobClient(mediaAccount, inputAsset.StorageAccountName);
                        CloudBlockBlob[] blobs = blobClient.ListBlobContainer(inputAsset.Container, null);
                        foreach (CloudBlockBlob blob in blobs)
                        {
                            blob.SetStandardBlobTierAsync(StandardBlobTier.Archive);
                        }
                    }
                }
            }
            return publishNotification;
        }
    }
}