using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.ServiceBroker;
using SkyMedia.WebApp.Models;

namespace SkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/job/event")]
        public MediaJobEvent GetJobEvent()
        {
            CloudQueueMessage queueMessage;
            MessageClient messageClient = new MessageClient();
            string queueName = Constants.Storage.QueueName.JobStatus;
            MediaJobEvent jobEvent = messageClient.GetMessage<MediaJobEvent>(queueName, out queueMessage);
            if (queueMessage == null)
            {
                jobEvent = new MediaJobEvent();
            }
            else if (string.IsNullOrEmpty(jobEvent.Properties.JobId))
            {
                messageClient.DeleteMessage(queueName, queueMessage);
            }
            return jobEvent;
        }

        [HttpPost]
        [Route("/job/publish")]
        public PublishNotification PublishJob([FromBody] MediaJobPublish jobPublish)
        {
            PublishNotification notification = new PublishNotification();
            EntityClient entityClient = new EntityClient();
            string tableName = Constants.Storage.TableNames.AssetPublish;
            string partitionKey = jobPublish.AccountName;
            string rowKey = jobPublish.JobId;
            ContentPublish contentPublish = entityClient.GetEntity<ContentPublish>(tableName, partitionKey, rowKey);
            if (contentPublish != null)
            {
                tableName = Constants.Storage.TableNames.AssetProtection;
                ContentProtection contentProtection = entityClient.GetEntity<ContentProtection>(tableName, partitionKey, rowKey);
                MediaClient mediaClient = new MediaClient(contentPublish);
                IJob job = mediaClient.GetEntityById(EntityType.Job, jobPublish.JobId) as IJob;
                if (job != null)
                {
                    mediaClient.SetReservedUnits(job);
                    notification.MessageText = MediaClient.GetNotificationMessage(jobPublish.AccountName, job);
                    notification.MobileNumber = contentPublish.MobileNumber;
                    if (jobPublish.NewState == JobState.Finished)
                    {
                        MediaClient.PublishJob(mediaClient, job, contentPublish, contentProtection);
                    }
                }
                if (contentProtection != null)
                {
                    entityClient.DeleteEntity(tableName, contentProtection);
                }
                tableName = Constants.Storage.TableNames.AssetPublish;
                entityClient.DeleteEntity(tableName, contentPublish);
            }
            return notification;
        }
    }
}
