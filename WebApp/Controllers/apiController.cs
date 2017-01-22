using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.MediaServices.Client;

using AzureSkyMedia.Services;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/processors")]
        public IMediaProcessor[] GetMediaProcessors(string accountName, string accountKey)
        {
            MediaClient mediaClient = new MediaClient(accountName, accountKey);
            return mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
        }

        [HttpPost]
        [Route("/message")]
        public void SendSMSText(string messageText, string mobileNumber)
        {
            MessageClient.SendText(messageText, mobileNumber);
        }

        [HttpPost]
        [Route("/publish")]
        public void PublishJobOutput()
        {
            CloudQueueMessage queueMessage;
            MessageClient messageClient = new MessageClient();
            string settingKey = Constants.AppSettings.MediaJobNotificationStorageQueueName;
            string queueName = AppSetting.GetValue(settingKey);
            MediaJobEvent jobEvent = messageClient.GetMessage<MediaJobEvent>(queueName, out queueMessage);
            if (queueMessage != null)
            {
                string partitionKey = jobEvent.EventProperties.AccountName;
                string rowKey = jobEvent.EventProperties.JobId;
                if (!string.IsNullOrEmpty(partitionKey) && !string.IsNullOrEmpty(rowKey))
                {
                    EntityClient entityClient = new EntityClient();
                    string tableName = Constants.Storage.TableNames.JobPublish;
                    JobPublish jobPublish = entityClient.GetEntity<JobPublish>(tableName, partitionKey, rowKey);
                    if (jobPublish != null)
                    {
                        tableName = Constants.Storage.TableNames.JobPublishProtection;
                        ContentProtection contentProtection = entityClient.GetEntity<ContentProtection>(tableName, partitionKey, rowKey);
                        MediaClient mediaClient = new MediaClient(jobPublish.PartitionKey, jobPublish.MediaAccountKey);
                        IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobEvent.EventProperties.JobId) as IJob;
                        if (job != null)
                        {
                            if (jobEvent.EventProperties.NewState == JobState.Finished)
                            {
                                MediaClient.PublishJob(mediaClient, job, jobPublish, contentProtection);
                            }
                            string messageText = MediaClient.GetNotificationMessage(jobPublish.PartitionKey, job);
                            SendSMSText(messageText, jobPublish.MobileNumber);
                        }
                        if (contentProtection != null)
                        {
                            tableName = Constants.Storage.TableNames.JobPublishProtection;
                            entityClient.DeleteEntity(tableName, contentProtection);
                        }
                        tableName = Constants.Storage.TableNames.JobPublish;
                        entityClient.DeleteEntity(tableName, jobPublish);
                    }
                }
                messageClient.DeleteMessage(queueName, queueMessage);
            }
        }
    }
}
