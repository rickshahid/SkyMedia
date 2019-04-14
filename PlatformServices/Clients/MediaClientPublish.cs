using System.Text;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static string GetNotificationMessage(MediaAccount mediaAccount, string transformName, Job job)
        {
            StringBuilder message = new StringBuilder();
            message.Append("AMS Job Notification");
            message.Append("\n\nMedia Services Account\n");
            message.Append(mediaAccount.Name);
            message.Append("\n\nTransform Name\n");
            message.Append(transformName);
            message.Append("\n\nJob Name\n");
            message.Append(job.Name);
            message.Append("\n\nJob State\n");
            message.Append(job.State.ToString());
            message.Append("\n\nJob Priority\n");
            message.Append(job.Priority);
            message.Append("\n\nJob Created\n");
            message.Append(job.Created);
            message.Append("\n\nJob Last Modified\n");
            message.Append(job.LastModified);
            foreach (JobOutput jobOutput in job.Outputs)
            {
                message.Append("\n\nJob Output Label\n");
                message.Append(jobOutput.Label);
                message.Append("\n\nJob Output State\n");
                message.Append(jobOutput.State.ToString());
                message.Append("\n\nJob Output Progress\n");
                message.Append(jobOutput.Progress);
                if (jobOutput.Error != null)
                {
                    message.Append("\n\nOutput Error Category\n");
                    message.Append(jobOutput.Error.Category.ToString());
                    message.Append("\n\nOutput Error Code\n");
                    message.Append(jobOutput.Error.Code.ToString());
                    message.Append("\n\nOutput Error Message\n");
                    message.Append(jobOutput.Error.Message);
                    message.Append("\n\nOutput Error Retry\n");
                    message.Append(jobOutput.Error.Retry);
                    foreach (JobErrorDetail errorDetail in jobOutput.Error.Details)
                    {
                        message.Append("\n\nOutput Error Detail Code\n");
                        message.Append(errorDetail.Code.ToString());
                        message.Append("\n\nOutput Error Detail Message\n");
                        message.Append(errorDetail.Message);
                    }
                }
            }
            message.Append("\n\nStreaming Policy Name\n");
            message.Append(job.CorrelationData["streamingPolicyName"]);
            return message.ToString();
        }

        //private static MediaJobPublish GetJobPublish(Job job)
        //{
        //    string jobPublish = job.CorrelationData[Constant.Media.Job.OutputPublish];
        //    return JsonConvert.DeserializeObject<MediaJobPublish>(jobPublish);
        //}

        //public static JObject GetJobPublish(string jobData, string streamingPolicyName)
        //{
        //    //string mobilePhoneNumber = null;
        //    //if (!string.IsNullOrEmpty(authToken))
        //    //{
        //    //    User currentUser = new User(authToken);
        //    //    mobilePhoneNumber = currentUser.MobilePhoneNumber;
        //    //}
        //    MediaJobPublish jobPublish = new MediaJobPublish()
        //    {
        //        StreamingPolicyName = streamingPolicyName,
        //        ContentProtection = null,
        //        //UserNotification = new UserNotification()
        //        //{
        //        //    MobilePhoneNumber = mobilePhoneNumber
        //        //}
        //    };
        //    JObject jobPublishOutput = string.IsNullOrEmpty(jobData) ? new JObject() : JObject.Parse(jobData);
        //    jobPublishOutput[Constant.Media.Job.OutputPublish] = JObject.FromObject(jobPublish);
        //    return jobPublishOutput;
        //}

        public static StreamingLocator PublishJobOutput(MediaAccount mediaAccount, string transformName, string jobName)
        {
            StreamingLocator streamingLocator = null;
            using (MediaClient mediaClient = new MediaClient(mediaAccount))
            {
                Job job = mediaClient.GetEntity<Job>(MediaEntity.TransformJob, jobName, transformName);
                foreach (JobOutputAsset jobOutput in job.Outputs)
                {
                    Asset outputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, jobOutput.AssetName);
                    MediaAsset mediaAsset = new MediaAsset(mediaClient, outputAsset);
                    if (mediaAsset.Streamable)
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
                        streamingLocator = mediaClient.CreateLocator(jobOutput.AssetName, jobOutput.AssetName, streamingPolicyName, contentProtection);
                        if (job.CorrelationData.ContainsKey("archiveInput"))
                        {
                        }
                    }
                }

                //if (!string.IsNullOrEmpty(insightId))
                //{
                //    JObject insight = mediaClient.IndexerGetInsight(insightId);
                //    if (insight != null)
                //    {
                //        using (DatabaseClient databaseClient = new DatabaseClient(true))
                //        {
                //            string collectionId = Constant.Database.Collection.MediaContentInsight;
                //            databaseClient.UpsertDocument(collectionId, insight);
                //        }
                //    }
                //}
            }
            return streamingLocator;
        }
    }
}